using System.Net;
using System.Text.Json;
using AKCore.DataModel;
using AKCore.IntegrationTests.TestData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests;

public class UpcomingControllerTests
{
    [Fact]
    public async Task UpcomingListData_ReturnsFuturePublicEvents()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAsync(TestEvents.PublicSpelning());

        var client = factory.CreateClientWithHttpsBaseAddress();
        var response = await client.GetAsync("/upcoming/UpcomingListData");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains(TestEvents.UpcomingListName, body);
    }

    [Fact]
    public async Task Ical_ReturnsCalendarWithFutureEvents()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAsync(TestEvents.IcalSpelning());

        var client = factory.CreateClientWithHttpsBaseAddress();
        var response = await client.GetAsync("/upcoming/akevents.ics");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/octet-stream", response.Content.Headers.ContentType?.MediaType);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("BEGIN:VCALENDAR", body);
        Assert.Contains(TestEvents.IcalEventName, body);
    }

    [Fact]
    public async Task SignUp_CreatesSignupForAuthenticatedMember()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();
        var eventId = await factory.SeedEventAndReturnIdAsync(TestEvents.SignupSpelning());

        var client = TestClients.CreateMemberClient(factory);
        var response = await client.PostAsync(
            $"/upcoming/Signup/{eventId}",
            SignupForms.Create());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.True(json.RootElement.GetProperty("success").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var evt = await db.Events.Include(e => e.SignUps).FirstAsync(e => e.Id == eventId);
        Assert.Contains(evt.SignUps, s => s.Where == AkSignupType.Halan && s.PersonId != null);
    }

    [Fact]
    public async Task SignUp_RequiresWhere()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();
        var eventId = await factory.SeedEventAndReturnIdAsync(TestEvents.SignupSpelning());

        var client = TestClients.CreateMemberClient(factory);
        var response = await client.PostAsync(
            $"/upcoming/Signup/{eventId}",
            new FormUrlEncodedContent(new Dictionary<string, string>()));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.False(json.RootElement.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task SignUp_RequiresMemberRole()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();
        var eventId = await factory.SeedEventAndReturnIdAsync(TestEvents.SignupSpelning());

        var client = TestClients.CreateAnonymousClient(factory);
        var response = await client.PostAsync(
            $"/upcoming/Signup/{eventId}",
            SignupForms.Create());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SignUp_ReflectedInUpcomingList()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();
        var eventId = await factory.SeedEventAndReturnIdAsync(TestEvents.SignupSpelning());

        var client = TestClients.CreateMemberClient(factory);
        var signupResponse = await client.PostAsync(
            $"/upcoming/Signup/{eventId}",
            SignupForms.Create(where: AkSignupType.Halan));

        Assert.Equal(HttpStatusCode.OK, signupResponse.StatusCode);

        var signupBody = await signupResponse.Content.ReadAsStringAsync();
        using var signupJson = JsonDocument.Parse(signupBody);
        Assert.True(signupJson.RootElement.GetProperty("success").GetBoolean());

        var listResponse = await client.GetAsync("/upcoming/UpcomingListData");

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var listBody = await listResponse.Content.ReadAsStringAsync();
        using var listJson = JsonDocument.Parse(listBody);

        Assert.True(listJson.RootElement.GetProperty("loggedIn").GetBoolean());
        Assert.True(listJson.RootElement.GetProperty("member").GetBoolean());
        Assert.Contains(TestEvents.SignupEventName, listBody);
        Assert.Equal(AkSignupType.Halan, FindSignupStateForEvent(listJson.RootElement, eventId));
    }

    private static string? FindSignupStateForEvent(JsonElement root, int eventId)
    {
        if (!root.TryGetProperty("years", out var years))
        {
            return null;
        }

        foreach (var yearProp in years.EnumerateObject())
        {
            var months = yearProp.Value.GetProperty("months");
            foreach (var monthProp in months.EnumerateObject())
            {
                foreach (var evt in monthProp.Value.EnumerateArray())
                {
                    if (evt.TryGetProperty("id", out var id) && id.GetInt32() == eventId)
                    {
                        return evt.TryGetProperty("signupState", out var state) ? state.GetString() : null;
                    }
                }
            }
        }

        return null;
    }
}
