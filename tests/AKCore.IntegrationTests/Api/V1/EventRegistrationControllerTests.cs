using AKCore.DataModel;
using AKCore.IntegrationTests.TestData;
using AKCore.Models.Api.V1.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AKCore.IntegrationTests.Api.V1;

public class EventRegistrationControllerTests
{
    [Fact]
    public async Task Put_AnonymousRequest_ReturnsUnauthorized()
    {
        await using var factory = new CustomWebApplicationFactory();

        var eventId = await factory.SeedEventAndReturnIdAsync(
            TestEvents.SignupSpelning());

        using var client = TestClients.CreateAnonymousClient(factory);

        var response = await PutRegistrationAsync(
            client,
            eventId,
            AkSignupType.Halan);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Put_AuthenticatedNonMember_ReturnsForbidden()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedEditorAsync();

        var eventId = await factory.SeedEventAndReturnIdAsync(
            TestEvents.SignupSpelning());

        using var client = await CreateMobileClientAsync(
            factory,
            TestUsers.EditorUserName);

        var response = await PutRegistrationAsync(
            client,
            eventId,
            AkSignupType.Halan);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Put_MissingEvent_ReturnsNotFound()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        using var client = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var response = await PutRegistrationAsync(
            client,
            999999,
            AkSignupType.Halan);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_NewRegistration_CreatesSignup()
    {
        await using var factory = new CustomWebApplicationFactory();

        var memberId =
            await factory.SeedMemberAndReturnIdAsync();

        var eventId = await factory.SeedEventAndReturnIdAsync(
            TestEvents.SignupSpelning());

        using var client = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var response = await PutRegistrationAsync(
            client,
            eventId,
            AkSignupType.Direct,
            car: true,
            instrument: false,
            comment: "Kommer direkt",
            selectedInstrument: "Flöjt");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();

        var signups = await db.SignUps
            .Where(x =>
                x.Event.Id == eventId &&
                x.PersonId == memberId)
            .ToListAsync();

        var signup = Assert.Single(signups);

        Assert.Equal(AkSignupType.Direct, signup.Where);
        Assert.True(signup.Car);
        Assert.False(signup.Instrument);
        Assert.Equal("Kommer direkt", signup.Comment);
        Assert.Equal("Flöjt", signup.InstrumentName);
    }

    [Fact]
    public async Task Put_WebsiteRegistration_UpdatesSameSignupWithoutDuplicate()
    {
        await using var factory = new CustomWebApplicationFactory();

        var memberId =
            await factory.SeedMemberAndReturnIdAsync();

        var eventId = await factory.SeedEventAndReturnIdAsync(
            TestEvents.SignupSpelning());

        using (var websiteClient =
            TestClients.CreateMemberClient(factory))
        {
            var websiteResponse = await websiteClient.PostAsync(
                $"/upcoming/Signup/{eventId}",
                SignupForms.Create(
                    where: AkSignupType.Halan,
                    car: false,
                    instrument: true,
                    comment: "Website"));

            Assert.Equal(
                HttpStatusCode.OK,
                websiteResponse.StatusCode);
        }

        using var mobileClient = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var mobileResponse = await PutRegistrationAsync(
            mobileClient,
            eventId,
            AkSignupType.Direct,
            car: true,
            instrument: false,
            comment: "Mobile",
            selectedInstrument: "Flöjt");

        Assert.Equal(
            HttpStatusCode.NoContent,
            mobileResponse.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();

        var signups = await db.SignUps
            .Where(x =>
                x.Event.Id == eventId &&
                x.PersonId == memberId)
            .ToListAsync();

        var signup = Assert.Single(signups);

        Assert.Equal(AkSignupType.Direct, signup.Where);
        Assert.True(signup.Car);
        Assert.False(signup.Instrument);
        Assert.Equal("Mobile", signup.Comment);
        Assert.Equal("Flöjt", signup.InstrumentName);
    }

    [Fact]
    public async Task Put_MissingWhere_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var eventId = await factory.SeedEventAndReturnIdAsync(
            TestEvents.SignupSpelning());

        using var client = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var response = await PutRegistrationAsync(
            client,
            eventId,
            "");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_InvalidWhere_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var eventId = await factory.SeedEventAndReturnIdAsync(
            TestEvents.SignupSpelning());

        using var client = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var response = await PutRegistrationAsync(
            client,
            eventId,
            "Something else");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_DisabledEvent_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var evt = TestEvents.SignupSpelning();
        evt.Disabled = true;

        var eventId =
            await factory.SeedEventAndReturnIdAsync(evt);

        using var client = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var response = await PutRegistrationAsync(
            client,
            eventId,
            AkSignupType.Halan);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();

        Assert.Empty(
            await db.SignUps
                .Where(x => x.Event.Id == eventId)
                .ToListAsync());
    }

    [Fact]
    public async Task Put_OlderEvent_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var evt = TestEvents.SignupSpelning();
        evt.Day = DateTime.UtcNow.Date.AddDays(-2);

        var eventId =
            await factory.SeedEventAndReturnIdAsync(evt);

        using var client = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var response = await PutRegistrationAsync(
            client,
            eventId,
            AkSignupType.Halan);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();

        Assert.Empty(
            await db.SignUps
                .Where(x => x.Event.Id == eventId)
                .ToListAsync());
    }

    [Fact]
    public async Task Put_YesterdaysEvent_RemainsAvailable()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var evt = TestEvents.SignupSpelning();
        evt.Day = DateTime.UtcNow.Date.AddDays(-1);

        var eventId =
            await factory.SeedEventAndReturnIdAsync(evt);

        using var client = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var response = await PutRegistrationAsync(
            client,
            eventId,
            AkSignupType.Halan);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private static Task<HttpResponseMessage> PutRegistrationAsync(
        HttpClient client,
        int eventId,
        string where,
        bool car = false,
        bool instrument = true,
        string? comment = null,
        string? selectedInstrument = null) =>
        client.PutAsJsonAsync(
            $"/api/v1/events/{eventId}/registration",
            new
            {
                where,
                car,
                instrument,
                comment,
                selectedInstrument
            });

    private static async Task<HttpClient> CreateMobileClientAsync(
        CustomWebApplicationFactory factory,
        string userName)
    {
        var client = TestClients.CreateAnonymousClient(factory);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                username = userName,
                password = TestUsers.DefaultPassword
            });

        Assert.Equal(
            HttpStatusCode.OK,
            loginResponse.StatusCode);

        var login = await loginResponse.Content
            .ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(login);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken);

        return client;
    }
}
