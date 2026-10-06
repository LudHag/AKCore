using AKCore.DataModel;
using AKCore.IntegrationTests.TestData;
using AKCore.Models.Api.V1.Auth;
using AKCore.Models.Api.V1.Calendar;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace AKCore.IntegrationTests.Api.V1;

public class CalendarControllerTests
{
    [Fact]
    public async Task Get_AnonymousRequest_ReturnsUnauthorized()
    {
        await using var factory = new CustomWebApplicationFactory();

        using var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.GetAsync("/api/v1/calendar");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_AuthenticatedMember_ReturnsWebsiteCalendarSemantics()
    {
        await using var factory = new CustomWebApplicationFactory();

        var userId = await factory.SeedMemberAndReturnIdAsync();

        var firstEvent = new Event
        {
            Type = AkEventTypes.KarRep,
            Name = "First event",
            Place = "Kårhuset",
            Description = "Public description",
            InternalDescription = "Member description",
            Day = DateTime.UtcNow.Date.AddDays(1),
            HalanTime = new TimeSpan(8, 0, 0),
            ThereTime = new TimeSpan(8, 30, 0),
            StartsTime = default,
            PlayDuration = "90 min",
            Stand = "",
            Secret = true,
            Disabled = true,
            SignUps =
            [
                new SignUp
                {
                    Person = "Member",
                    PersonId = userId,
                    Where = AkSignupType.Halan,
                    SignupTime = DateTime.UtcNow
                },
                new SignUp
                {
                    Person = "Other member",
                    PersonId = "other-member",
                    Where = AkSignupType.Direct,
                    SignupTime = DateTime.UtcNow
                },
                new SignUp
                {
                    Person = "Absent member",
                    PersonId = "absent-member",
                    Where = AkSignupType.CantCome,
                    SignupTime = DateTime.UtcNow
                }
            ]
        };

        var secondEvent = new Event
        {
            Type = AkEventTypes.Spelning,
            Name = "Second event",
            Place = "Gasquesalen",
            Description = "Second description",
            Day = DateTime.UtcNow.Date.AddDays(1),
            HalanTime = new TimeSpan(7, 0, 0),
            ThereTime = new TimeSpan(8, 45, 0),
            StartsTime = new TimeSpan(9, 0, 0),
            PlayDuration = "15 min",
            Stand = "Gå",
            Secret = false,
            Disabled = false,
            SignUps = []
        };

        var pastEvent = new Event
        {
            Type = AkEventTypes.Spelning,
            Name = "Past event",
            Day = DateTime.UtcNow.Date.AddDays(-1),
            SignUps = []
        };

        await factory.SeedAsync(firstEvent);
        await factory.SeedAsync(secondEvent);
        await factory.SeedAsync(pastEvent);

        using var client = await CreateMobileMemberClientAsync(factory);

        var response = await client.GetAsync("/api/v1/calendar");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();

        var body = JsonSerializer.Deserialize<CalendarResponse>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        Assert.NotNull(body);
        Assert.Equal(2, body.Events.Count);

        var first = body.Events[0];

        Assert.Equal(firstEvent.Id, first.Id);
        Assert.Equal(AkEventTypes.KarRep, first.Type);
        Assert.Equal("First event", first.Name);
        Assert.Equal("Kårhuset", first.Place);
        Assert.Equal("Public description", first.Description);
        Assert.Equal("Member description", first.InternalDescription);
        Assert.Equal(
            firstEvent.Day.ToString("yyyy-MM-dd"),
            first.Date);
        Assert.Equal("08:00", first.HalanTime);
        Assert.Equal("08:30", first.ThereTime);
        Assert.Equal("00:00", first.StartsTime);
        Assert.Equal("90 min", first.PlayDuration);
        Assert.Equal(AkSignupType.Halan, first.SignupState);
        Assert.Equal(2, first.Coming);
        Assert.Equal(1, first.NotComing);
        Assert.True(first.Disabled);

        Assert.Equal(secondEvent.Id, body.Events[1].Id);

        Assert.DoesNotContain("Past event", json);

        var lowerJson = json.ToLowerInvariant();

        Assert.DoesNotContain("\"secret\"", lowerJson);
        Assert.DoesNotContain("\"signups\"", lowerJson);
        Assert.DoesNotContain("\"descriptioneng\"", lowerJson);
        Assert.DoesNotContain("\"internaldescriptioneng\"", lowerJson);
        Assert.DoesNotContain("\"fikacollection\"", lowerJson);
    }

    private static async Task<HttpClient> CreateMobileMemberClientAsync(
        CustomWebApplicationFactory factory)
    {
        var client = TestClients.CreateAnonymousClient(factory);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                username = TestUsers.MemberUserName,
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
