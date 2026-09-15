using AKCore.DataModel;
using AKCore.IntegrationTests.TestData;
using AKCore.Models.Api.V1.Auth;
using AKCore.Models.Api.V1.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AKCore.IntegrationTests.Api.V1;

public class EventDetailsControllerTests
{
    [Fact]
    public async Task Get_AnonymousRequest_ReturnsUnauthorized()
    {
        await using var factory = new CustomWebApplicationFactory();

        using var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.GetAsync("/api/v1/events/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_AuthenticatedNonMember_ReturnsForbidden()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedEditorAsync();

        using var client = await CreateMobileClientAsync(
            factory,
            TestUsers.EditorUserName);

        var response = await client.GetAsync("/api/v1/events/1");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Get_MissingEvent_ReturnsNotFound()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        using var client = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var response = await client.GetAsync("/api/v1/events/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_ValidEvent_ReturnsNormalMemberEventDetails()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var evt = new Event
        {
            Type = AkEventTypes.Spelning,
            Name = "Event detail test",
            Place = "Gasquesalen",
            Description = "Public description",
            DescriptionEng = "English public description",
            InternalDescription = "Member description",
            InternalDescriptionEng = "English member description",
            Fika = "Test fika",
            FikaCollection = "One,Two",
            Day = DateTime.UtcNow.Date.AddDays(2),
            HalanTime = new TimeSpan(17, 30, 0),
            ThereTime = new TimeSpan(18, 15, 0),
            StartsTime = new TimeSpan(19, 0, 0),
            PlayDuration = "60 min",
            Stand = "Stå",
            Secret = true,
            Disabled = false,
            SignUps = []
        };

        var eventId = await factory.SeedEventAndReturnIdAsync(evt);

        using var client = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var response = await client.GetAsync($"/api/v1/events/{eventId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content
            .ReadFromJsonAsync<EventDetailResponse>();

        Assert.NotNull(body);
        Assert.Equal(eventId, body.Id);
        Assert.Equal(AkEventTypes.Spelning, body.Type);
        Assert.Equal("Event detail test", body.Name);
        Assert.Equal("Gasquesalen", body.Place);
        Assert.Equal("Public description", body.Description);
        Assert.Equal("Member description", body.InternalDescription);
        Assert.Equal(
            evt.Day.ToString("yyyy-MM-dd"),
            body.Date);
        Assert.Equal("17:30", body.HalanTime);
        Assert.Equal("18:15", body.ThereTime);
        Assert.Equal("19:00", body.StartsTime);
        Assert.Equal("60 min", body.PlayDuration);
        Assert.Equal("Stå", body.Stand);
        Assert.Null(body.SignupState);
        Assert.Equal(0, body.Coming);
        Assert.Equal(0, body.NotComing);
        Assert.False(body.Disabled);
        Assert.True(body.RegistrationAvailable);

        Assert.NotNull(body.Registration);
        Assert.Null(body.Registration.Where);
        Assert.False(body.Registration.Car);
        Assert.True(body.Registration.Instrument);
        Assert.Equal("", body.Registration.Comment);
        Assert.Equal("Flöjt", body.Registration.SelectedInstrument);
        Assert.Equal(["Flöjt"], body.Registration.AvailableInstruments);

        Assert.Empty(body.Attendees);

        var json = await response.Content.ReadAsStringAsync();
        var lowerJson = json.ToLowerInvariant();

        Assert.DoesNotContain("\"secret\"", lowerJson);
        Assert.DoesNotContain("\"descriptioneng\"", lowerJson);
        Assert.DoesNotContain("\"internaldescriptioneng\"", lowerJson);
        Assert.DoesNotContain("\"fikacollection\"", lowerJson);
        Assert.DoesNotContain("\"isnintendo\"", lowerJson);
        Assert.DoesNotContain("\"members\"", lowerJson);
    }

    [Fact]
    public async Task Get_ExistingRegistration_ReturnsSelectionAndMemberRoster()
    {
        await using var factory = new CustomWebApplicationFactory();

        var memberId = await factory.SeedMemberAndReturnIdAsync();

        using (var scope = factory.Services.CreateScope())
        {
            var userManager =
                scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();

            var user = await userManager.FindByNameAsync(
                TestUsers.MemberUserName);

            Assert.NotNull(user);

            user.OtherInstruments = "Trumpet, Flöjt";

            var result = await userManager.UpdateAsync(user);

            Assert.True(result.Succeeded);
        }

        var evt = new Event
        {
            Type = AkEventTypes.Spelning,
            Name = "Registered event",
            Place = "Kårhuset",
            Description = "Description",
            InternalDescription = "Internal description",
            Day = DateTime.UtcNow.Date.AddDays(1),
            HalanTime = new TimeSpan(18, 0, 0),
            ThereTime = new TimeSpan(18, 30, 0),
            StartsTime = new TimeSpan(19, 0, 0),
            SignUps =
            [
                new SignUp
                {
                    Person = TestUsers.MemberUserName,
                    PersonId = memberId,
                    PersonName = "Test Member",
                    Where = AkSignupType.Direct,
                    Car = true,
                    Instrument = false,
                    InstrumentName = "Trumpet",
                    Comment = "Member comment",
                    SignupTime = DateTime.UtcNow
                },
                new SignUp
                {
                    Person = "other-member",
                    PersonId = "other-member-id",
                    PersonName = "Other Member",
                    Where = AkSignupType.Halan,
                    Car = false,
                    Instrument = true,
                    InstrumentName = "Flöjt",
                    Comment = "Other comment",
                    SignupTime = DateTime.UtcNow.AddMinutes(-10)
                },
                new SignUp
                {
                    Person = "absent-member",
                    PersonId = "absent-member-id",
                    PersonName = "Absent Member",
                    Where = AkSignupType.CantCome,
                    Car = false,
                    Instrument = true,
                    InstrumentName = "Klarinett",
                    Comment = "Cannot attend",
                    SignupTime = DateTime.UtcNow.AddMinutes(-20)
                }
            ]
        };

        var eventId = await factory.SeedEventAndReturnIdAsync(evt);

        using var client = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var response = await client.GetAsync($"/api/v1/events/{eventId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content
            .ReadFromJsonAsync<EventDetailResponse>();

        Assert.NotNull(body);

        Assert.Equal(AkSignupType.Direct, body.SignupState);
        Assert.Equal(2, body.Coming);
        Assert.Equal(1, body.NotComing);

        Assert.Equal(AkSignupType.Direct, body.Registration.Where);
        Assert.True(body.Registration.Car);
        Assert.False(body.Registration.Instrument);
        Assert.Equal("Member comment", body.Registration.Comment);
        Assert.Equal(
            "Trumpet",
            body.Registration.SelectedInstrument);
        Assert.Equal(
            ["Flöjt", "Trumpet"],
            body.Registration.AvailableInstruments);

        Assert.Equal(3, body.Attendees.Count);

        var currentMember = body.Attendees.Single(
            x => x.PersonName == "Test Member");

        Assert.Equal(AkSignupType.Direct, currentMember.Where);
        Assert.True(currentMember.Car);
        Assert.False(currentMember.Instrument);
        Assert.Equal("Trumpet", currentMember.InstrumentName);
        Assert.Equal("Member comment", currentMember.Comment);

        var otherMember = body.Attendees.Single(
            x => x.PersonName == "Other Member");

        Assert.Equal(AkSignupType.Halan, otherMember.Where);
        Assert.Equal("Flöjt", otherMember.InstrumentName);

        var absentMember = body.Attendees.Single(
            x => x.PersonName == "Absent Member");

        Assert.Equal(AkSignupType.CantCome, absentMember.Where);

        var json = await response.Content.ReadAsStringAsync();
        var lowerJson = json.ToLowerInvariant();

        Assert.DoesNotContain("\"personid\"", lowerJson);
        Assert.DoesNotContain("\"person\"", lowerJson);
        Assert.DoesNotContain("\"signuptime\"", lowerJson);
        Assert.DoesNotContain("\"otherinstruments\"", lowerJson);
    }

    [Fact]
    public async Task Get_YesterdaysEvent_RegistrationRemainsAvailable()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var evt = new Event
        {
            Type = AkEventTypes.Spelning,
            Name = "Yesterday event",
            Day = DateTime.UtcNow.Date.AddDays(-1),
            Disabled = false,
            SignUps = []
        };

        var eventId = await factory.SeedEventAndReturnIdAsync(evt);

        using var client = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var response = await client.GetAsync($"/api/v1/events/{eventId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content
            .ReadFromJsonAsync<EventDetailResponse>();

        Assert.NotNull(body);
        Assert.True(body.RegistrationAvailable);
    }

    [Fact]
    public async Task Get_OlderOrDisabledEvent_RegistrationIsUnavailable()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var oldEvent = new Event
        {
            Type = AkEventTypes.Spelning,
            Name = "Old event",
            Day = DateTime.UtcNow.Date.AddDays(-2),
            Disabled = false,
            SignUps = []
        };

        var disabledEvent = new Event
        {
            Type = AkEventTypes.Spelning,
            Name = "Disabled event",
            Day = DateTime.UtcNow.Date.AddDays(2),
            Disabled = true,
            SignUps = []
        };

        var oldEventId =
            await factory.SeedEventAndReturnIdAsync(oldEvent);

        var disabledEventId =
            await factory.SeedEventAndReturnIdAsync(disabledEvent);

        using var client = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var oldResponse =
            await client.GetAsync($"/api/v1/events/{oldEventId}");

        var disabledResponse =
            await client.GetAsync($"/api/v1/events/{disabledEventId}");

        Assert.Equal(HttpStatusCode.OK, oldResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, disabledResponse.StatusCode);

        var oldBody = await oldResponse.Content
            .ReadFromJsonAsync<EventDetailResponse>();

        var disabledBody = await disabledResponse.Content
            .ReadFromJsonAsync<EventDetailResponse>();

        Assert.NotNull(oldBody);
        Assert.NotNull(disabledBody);

        Assert.False(oldBody.RegistrationAvailable);
        Assert.False(disabledBody.RegistrationAvailable);
    }

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

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var login =
            await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(login);
        Assert.NotEmpty(login.AccessToken);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken);

        return client;
    }
}
