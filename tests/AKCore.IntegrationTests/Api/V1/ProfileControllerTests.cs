using System.Net;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AKCore.IntegrationTests.TestData;
using AKCore.Models.Api.V1.Profile;
using AKCore.DataModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests.Api.V1;

public class ProfileControllerTests
{
    [Fact]
    public async Task Get_WithoutBearerToken_ReturnsUnauthorized()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.GetAsync("/api/v1/profile");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_AuthenticatedMember_ReturnsOwnProfile()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = await CreateMobileClientAsync(factory);

        var response = await client.GetAsync("/api/v1/profile");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var profile = await response.Content
            .ReadFromJsonAsync<ProfileResponse>();

        Assert.NotNull(profile);
        Assert.Equal(TestUsers.MemberUserName, profile.UserName);
        Assert.Equal("Test", profile.FirstName);
        Assert.Equal("Member", profile.LastName);
        Assert.Equal("Flöjt", profile.Instrument);
        Assert.Contains("Medlem", profile.Roles);
        Assert.Empty(profile.Posts);
    }

    [Fact]
    public async Task Get_DoesNotExposeInternalAccountFields()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = await CreateMobileClientAsync(factory);

        var response = await client.GetAsync("/api/v1/profile");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();

        using var json = System.Text.Json.JsonDocument.Parse(body);

        Assert.False(json.RootElement.TryGetProperty("id", out _));
        Assert.False(json.RootElement.TryGetProperty("password", out _));
        Assert.False(json.RootElement.TryGetProperty("passwordHash", out _));
        Assert.False(json.RootElement.TryGetProperty("securityStamp", out _));
        Assert.False(json.RootElement.TryGetProperty("mobileSessions", out _));
    }


    [Fact]
    public async Task GetStatistics_WithoutBearerToken_ReturnsUnauthorized()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.GetAsync(
            "/api/v1/profile/statistics");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetStatistics_Default_PreservesWebsiteDateSemantics()
    {
        await using var factory = new CustomWebApplicationFactory();
        var userId = await factory.SeedMemberAndReturnIdAsync();

        await SeedStatisticsEventsAsync(factory, userId);

        var client = await CreateMobileClientAsync(factory);
        var response = await client.GetAsync(
            "/api/v1/profile/statistics");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var statistics = await response.Content
            .ReadFromJsonAsync<ProfileStatisticsResponse>();

        Assert.NotNull(statistics);
        Assert.Equal(3, statistics.TotalGigs);
        Assert.Equal(1, statistics.Halan);
        Assert.Equal(0, statistics.Direct);
        Assert.Equal(1, statistics.CantCome);
        Assert.Equal(1, statistics.Car);
        Assert.Equal(1, statistics.Instrument);
        Assert.Equal(1, statistics.Comment);
    }

    [Fact]
    public async Task GetStatistics_CustomPeriod_UsesEventDates()
    {
        await using var factory = new CustomWebApplicationFactory();
        var userId = await factory.SeedMemberAndReturnIdAsync();

        await SeedStatisticsEventsAsync(factory, userId);

        var client = await CreateMobileClientAsync(factory);

        var today = DateTime.UtcNow.Date;
        var start = today.AddDays(-30)
            .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var end = today.AddDays(-1)
            .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        var response = await client.GetAsync(
            $"/api/v1/profile/statistics?startDate={start}&endDate={end}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var statistics = await response.Content
            .ReadFromJsonAsync<ProfileStatisticsResponse>();

        Assert.NotNull(statistics);
        Assert.Equal(2, statistics.TotalGigs);
        Assert.Equal(1, statistics.Halan);
        Assert.Equal(1, statistics.Direct);
        Assert.Equal(0, statistics.CantCome);
        Assert.Equal(1, statistics.Car);
        Assert.Equal(1, statistics.Instrument);
        Assert.Equal(1, statistics.Comment);
    }

    [Theory]
    [InlineData("?startDate=2026-01-01")]
    [InlineData("?endDate=2026-06-30")]
    [InlineData("?startDate=invalid&endDate=2026-06-30")]
    [InlineData("?startDate=2026-07-01&endDate=2026-06-30")]
    [InlineData("?startDate=2026-01-01&endDate=9999-12-31")]
    public async Task GetStatistics_InvalidPeriod_ReturnsBadRequest(
        string query)
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = await CreateMobileClientAsync(factory);

        var response = await client.GetAsync(
            "/api/v1/profile/statistics" + query);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Fact]
    public async Task GetStatistics_CustomPeriod_ExcludesCurrentDay()
    {
        await using var factory = new CustomWebApplicationFactory();
        var userId = await factory.SeedMemberAndReturnIdAsync();

        var today = DateTime.UtcNow.Date;

        await factory.SeedAsync(new Event
        {
            Type = AkEventTypes.Spelning,
            Name = "Today's event",
            Day = today,
            SignUps =
            [
                new SignUp
                {
                    Person = TestUsers.MemberUserName,
                    PersonId = userId,
                    Where = AkSignupType.Direct,
                    SignupTime = DateTime.UtcNow.AddDays(-1)
                }
            ]
        });

        var client = await CreateMobileClientAsync(factory);

        var date = today.ToString(
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture);

        var response = await client.GetAsync(
            $"/api/v1/profile/statistics?startDate={date}&endDate={date}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var statistics = await response.Content
            .ReadFromJsonAsync<ProfileStatisticsResponse>();

        Assert.NotNull(statistics);
        Assert.Equal(0, statistics.TotalGigs);
        Assert.Equal(0, statistics.Direct);
    }

    private static async Task SeedStatisticsEventsAsync(
        CustomWebApplicationFactory factory,
        string userId)
    {
        var today = DateTime.UtcNow.Date;
        var now = DateTime.UtcNow;

        await factory.SeedAsync(new Event
        {
            Type = AkEventTypes.Spelning,
            Name = "Recent Hålan event",
            Day = today.AddDays(-10),
            SignUps =
            [
                new SignUp
                {
                    Person = TestUsers.MemberUserName,
                    PersonId = userId,
                    Where = AkSignupType.Halan,
                    Car = true,
                    Instrument = true,
                    Comment = "Test comment",
                    SignupTime = now.AddDays(-15)
                },
                new SignUp
                {
                    Person = "Other member",
                    PersonId = "other-member-id",
                    Where = AkSignupType.Direct,
                    SignupTime = now.AddDays(-15)
                }
            ]
        });

        await factory.SeedAsync(new Event
        {
            Type = AkEventTypes.Spelning,
            Name = "Early registration event",
            Day = today.AddDays(-20),
            SignUps =
            [
                new SignUp
                {
                    Person = TestUsers.MemberUserName,
                    PersonId = userId,
                    Where = AkSignupType.Direct,
                    SignupTime = now.AddMonths(-13)
                }
            ]
        });

        await factory.SeedAsync(new Event
        {
            Type = AkEventTypes.Spelning,
            Name = "Outside requested period",
            Day = today.AddDays(-45),
            SignUps =
            [
                new SignUp
                {
                    Person = TestUsers.MemberUserName,
                    PersonId = userId,
                    Where = AkSignupType.CantCome,
                    SignupTime = now.AddDays(-50)
                }
            ]
        });
    }


    [Fact]
    public async Task UpdateProfile_WithoutBearerToken_ReturnsUnauthorized()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.PutAsJsonAsync(
            "/api/v1/profile",
            new { firstName = "Unauthorized" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProfile_ValidFields_UpdatesOwnAccount()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = await CreateMobileClientAsync(factory);

        var response = await client.PutAsJsonAsync(
            "/api/v1/profile",
            new
            {
                firstName = "Updated",
                lastName = "Member",
                email = "updated@example.com",
                phone = "0701234567",
                instrument = "Klarinett",
                otherInstruments = new[] { "Flöjt", "Trumpet" }
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var profile = await response.Content
            .ReadFromJsonAsync<ProfileResponse>();

        Assert.NotNull(profile);
        Assert.Equal("Updated", profile.FirstName);
        Assert.Equal("updated@example.com", profile.Email);
        Assert.Equal("Klarinett", profile.Instrument);
        Assert.Equal(
            ["Flöjt", "Trumpet"],
            profile.OtherInstruments);

        using var scope = factory.Services.CreateScope();
        var userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();

        var user = await userManager.FindByNameAsync(
            TestUsers.MemberUserName);

        Assert.NotNull(user);
        Assert.Equal("Updated", user.FirstName);
        Assert.Equal("Klarinett", user.Instrument);
        Assert.Equal("Flöjt,Trumpet", user.OtherInstruments);
    }

    [Fact]
    public async Task UpdateProfile_DuplicateInstrument_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = await CreateMobileClientAsync(factory);

        var response = await client.PutAsJsonAsync(
            "/api/v1/profile",
            new
            {
                firstName = "Changed",
                lastName = "Member",
                instrument = "Flöjt",
                otherInstruments = new[] { "Flöjt" }
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();

        var user = await userManager.FindByNameAsync(
            TestUsers.MemberUserName);

        Assert.NotNull(user);
        Assert.Equal("Test", user.FirstName);
    }

    [Fact]
    public async Task UpdateProfile_CannotChangeProtectedFieldsOrAnotherAccount()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();
        await factory.SeedMemberAsync(
            userName: "another-member",
            instrument: "Trumpet");

        using (var scope = factory.Services.CreateScope())
        {
            var userManager =
                scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();

            var user = await userManager.FindByNameAsync(
                TestUsers.MemberUserName);

            Assert.NotNull(user);

            user.Medal = "Gold";
            user.GivenMedal = "Silver";
            user.SlavPoster = "[\"Ordförande\"]";

            Assert.True((await userManager.UpdateAsync(user)).Succeeded);
        }

        var client = await CreateMobileClientAsync(factory);

        var response = await client.PutAsJsonAsync(
            "/api/v1/profile",
            new
            {
                id = "another-member",
                userName = "another-member",
                firstName = "Updated",
                lastName = "Member",
                email = "member@example.com",
                instrument = "Flöjt",
                otherInstruments = Array.Empty<string>(),
                medal = "Unauthorized",
                givenMedal = "Unauthorized",
                posts = new[] { "Unauthorized" },
                roles = new[] { AkRoles.SuperNintendo }
            });

        Assert.True(
            response.StatusCode == HttpStatusCode.OK,
            $"HTTP {(int)response.StatusCode}: " +
            await response.Content.ReadAsStringAsync());

        using var verifyScope = factory.Services.CreateScope();
        var verifyManager =
            verifyScope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();

        var member = await verifyManager.FindByNameAsync(
            TestUsers.MemberUserName);

        var other = await verifyManager.FindByNameAsync(
            "another-member");

        Assert.NotNull(member);
        Assert.NotNull(other);

        Assert.Equal("Updated", member.FirstName);
        Assert.Equal("Gold", member.Medal);
        Assert.Equal("Silver", member.GivenMedal);
        Assert.Equal("[\"Ordförande\"]", member.SlavPoster);

        Assert.Equal("Test", other.FirstName);
        Assert.Equal("Trumpet", other.Instrument);

        var roles = await verifyManager.GetRolesAsync(member);
        Assert.DoesNotContain(AkRoles.SuperNintendo, roles);
    }

    [Fact]
    public async Task UpdateProfile_InvalidEmail_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = await CreateMobileClientAsync(factory);

        var response = await client.PutAsJsonAsync(
            "/api/v1/profile",
            new
            {
                firstName = "Updated",
                lastName = "Member",
                email = "not-an-email",
                instrument = "Flöjt",
                otherInstruments = Array.Empty<string>()
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Fact]
    public async Task ChangeUsername_WithoutBearerToken_ReturnsUnauthorized()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.PutAsJsonAsync(
            "/api/v1/profile/username",
            new
            {
                newUsername = "renamed-member",
                currentPassword = TestUsers.DefaultPassword
            });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ChangeUsername_WrongPassword_PreservesUsername()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = await CreateMobileClientAsync(factory);

        var response = await client.PutAsJsonAsync(
            "/api/v1/profile/username",
            new
            {
                newUsername = "renamed-member",
                currentPassword = "incorrect-password"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();

        Assert.NotNull(await userManager.FindByNameAsync(
            TestUsers.MemberUserName));

        Assert.Null(await userManager.FindByNameAsync(
            "renamed-member"));
    }

    [Fact]
    public async Task ChangeUsername_DuplicateUsername_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();
        await factory.SeedMemberAsync(
            userName: "existing-member");

        var client = await CreateMobileClientAsync(factory);

        var response = await client.PutAsJsonAsync(
            "/api/v1/profile/username",
            new
            {
                newUsername = "existing-member",
                currentPassword = TestUsers.DefaultPassword
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();

        Assert.NotNull(await userManager.FindByNameAsync(
            TestUsers.MemberUserName));

        Assert.NotNull(await userManager.FindByNameAsync(
            "existing-member"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ChangeUsername_BlankUsername_ReturnsBadRequest(
        string newUsername)
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = await CreateMobileClientAsync(factory);

        var response = await client.PutAsJsonAsync(
            "/api/v1/profile/username",
            new
            {
                newUsername,
                currentPassword = TestUsers.DefaultPassword
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ChangeUsername_ValidChange_PreservesMobileAuthentication()
    {
        await using var factory = new CustomWebApplicationFactory();
        var originalUserId = await factory.SeedMemberAndReturnIdAsync();

        var client = TestClients.CreateAnonymousClient(factory);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                username = TestUsers.MemberUserName,
                password = TestUsers.DefaultPassword
            });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        using var loginJson = System.Text.Json.JsonDocument.Parse(
            await loginResponse.Content.ReadAsStringAsync());

        var originalAccessToken = loginJson.RootElement
            .GetProperty("accessToken").GetString();

        var originalRefreshToken = loginJson.RootElement
            .GetProperty("refreshToken").GetString();

        Assert.False(string.IsNullOrWhiteSpace(originalAccessToken));
        Assert.False(string.IsNullOrWhiteSpace(originalRefreshToken));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                originalAccessToken);

        var updateResponse = await client.PutAsJsonAsync(
            "/api/v1/profile/username",
            new
            {
                newUsername = "renamed-member",
                currentPassword = TestUsers.DefaultPassword
            });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var profileResponse = await client.GetAsync(
            "/api/v1/profile");

        Assert.Equal(HttpStatusCode.OK, profileResponse.StatusCode);

        var profile = await profileResponse.Content
            .ReadFromJsonAsync<ProfileResponse>();

        Assert.NotNull(profile);
        Assert.Equal("renamed-member", profile.UserName);

        using (var scope = factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<AkUser>>();

            Assert.Null(await userManager.FindByNameAsync(
                TestUsers.MemberUserName));

            var renamedUser = await userManager.FindByNameAsync(
                "renamed-member");

            Assert.NotNull(renamedUser);
            Assert.Equal(originalUserId, renamedUser.Id);
        }

        var anonymousClient = TestClients.CreateAnonymousClient(factory);

        var refreshResponse = await anonymousClient.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new
            {
                refreshToken = originalRefreshToken
            });

        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);

        using var refreshJson = System.Text.Json.JsonDocument.Parse(
            await refreshResponse.Content.ReadAsStringAsync());

        var refreshedAccessToken = refreshJson.RootElement
            .GetProperty("accessToken").GetString();

        Assert.False(string.IsNullOrWhiteSpace(refreshedAccessToken));

        var jwt = new System.IdentityModel.Tokens.Jwt
            .JwtSecurityTokenHandler()
            .ReadJwtToken(refreshedAccessToken);

        Assert.Equal(
            originalUserId,
            jwt.Claims.Single(x => x.Type == "sub").Value);

        Assert.Equal(
            "renamed-member",
            jwt.Claims.Single(x => x.Type == "unique_name").Value);

        var oldLogin = await anonymousClient.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                username = TestUsers.MemberUserName,
                password = TestUsers.DefaultPassword
            });

        Assert.Equal(HttpStatusCode.Unauthorized, oldLogin.StatusCode);

        var newLogin = await anonymousClient.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                username = "renamed-member",
                password = TestUsers.DefaultPassword
            });

        Assert.Equal(HttpStatusCode.OK, newLogin.StatusCode);
    }


    [Fact]
    public async Task ChangePassword_WithoutBearerToken_ReturnsUnauthorized()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.PutAsJsonAsync(
            "/api/v1/profile/password",
            new
            {
                currentPassword = TestUsers.DefaultPassword,
                newPassword = "NewPassword2!"
            });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_WrongCurrentPassword_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = await CreateMobileClientAsync(factory);

        var response = await client.PutAsJsonAsync(
            "/api/v1/profile/password",
            new
            {
                currentPassword = "incorrect-password",
                newPassword = "NewPassword2!"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();

        var user = await userManager.FindByNameAsync(
            TestUsers.MemberUserName);

        Assert.NotNull(user);
        Assert.True(await userManager.CheckPasswordAsync(
            user,
            TestUsers.DefaultPassword));
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    public async Task ChangePassword_InvalidNewPassword_ReturnsBadRequest(
        string newPassword)
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = await CreateMobileClientAsync(factory);

        var response = await client.PutAsJsonAsync(
            "/api/v1/profile/password",
            new
            {
                currentPassword = TestUsers.DefaultPassword,
                newPassword
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();

        var user = await userManager.FindByNameAsync(
            TestUsers.MemberUserName);

        Assert.NotNull(user);
        Assert.True(await userManager.CheckPasswordAsync(
            user,
            TestUsers.DefaultPassword));
    }

    [Fact]
    public async Task ChangePassword_Success_RevokesOnlyOwnRefreshSessions()
    {
        await using var factory = new CustomWebApplicationFactory();
        var memberId = await factory.SeedMemberAndReturnIdAsync();

        var otherId = await factory.SeedMemberAndReturnIdAsync(
            userName: "another-member");

        var client = TestClients.CreateAnonymousClient(factory);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                username = TestUsers.MemberUserName,
                password = TestUsers.DefaultPassword
            });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        using var loginJson = System.Text.Json.JsonDocument.Parse(
            await loginResponse.Content.ReadAsStringAsync());

        var accessToken = loginJson.RootElement
            .GetProperty("accessToken").GetString();

        var refreshToken = loginJson.RootElement
            .GetProperty("refreshToken").GetString();

        Assert.False(string.IsNullOrWhiteSpace(accessToken));
        Assert.False(string.IsNullOrWhiteSpace(refreshToken));

        await factory.SeedAsync(db =>
        {
            db.MobileSessions.Add(new MobileSession
            {
                UserId = otherId,
                RefreshTokenHash = new string('a', 64),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            });

            return Task.CompletedTask;
        });

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.PutAsJsonAsync(
            "/api/v1/profile/password",
            new
            {
                currentPassword = TestUsers.DefaultPassword,
                newPassword = "NewPassword2!"
            });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<AKContext>();

            Assert.Empty(db.MobileSessions.Where(
                x => x.UserId == memberId));

            Assert.Single(db.MobileSessions.Where(
                x => x.UserId == otherId));

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<AkUser>>();

            var user = await userManager.FindByIdAsync(memberId);

            Assert.NotNull(user);
            Assert.True(await userManager.CheckPasswordAsync(
                user,
                "NewPassword2!"));

            Assert.False(await userManager.CheckPasswordAsync(
                user,
                TestUsers.DefaultPassword));
        }

        var anonymousClient = TestClients.CreateAnonymousClient(factory);

        var revokedRefresh = await anonymousClient.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new
            {
                refreshToken
            });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            revokedRefresh.StatusCode);

        var oldLogin = await anonymousClient.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                username = TestUsers.MemberUserName,
                password = TestUsers.DefaultPassword
            });

        Assert.Equal(HttpStatusCode.Unauthorized, oldLogin.StatusCode);

        var newLogin = await anonymousClient.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                username = TestUsers.MemberUserName,
                password = "NewPassword2!"
            });

        Assert.Equal(HttpStatusCode.OK, newLogin.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_ExistingAccessToken_IsRevoked()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = await CreateMobileClientAsync(factory);

        var changeResponse = await client.PutAsJsonAsync(
            "/api/v1/profile/password",
            new
            {
                currentPassword = TestUsers.DefaultPassword,
                newPassword = "NewPassword2!"
            });

        Assert.Equal(
            HttpStatusCode.NoContent,
            changeResponse.StatusCode);

        var profileResponse = await client.GetAsync(
            "/api/v1/profile");

        Assert.Equal(HttpStatusCode.Unauthorized, profileResponse.StatusCode);
    }


    [Fact]
    public async Task ChangePassword_OnWebsite_RevokesMobileAccessToken()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var mobileClient = await CreateMobileClientAsync(factory);

        var before = await mobileClient.GetAsync("/api/v1/profile");
        Assert.Equal(HttpStatusCode.OK, before.StatusCode);

        var websiteClient = TestClients.CreateMemberClient(factory);

        var change = await websiteClient.PostAsync(
            "/Profile/ChangePassword",
            new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    ["password"] = "NewPassword2!"
                }));

        Assert.Equal(HttpStatusCode.OK, change.StatusCode);

        var after = await mobileClient.GetAsync("/api/v1/profile");

        Assert.Equal(HttpStatusCode.Unauthorized, after.StatusCode);
    }

    private static async Task<HttpClient> CreateMobileClientAsync(
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

        var login = await loginResponse.Content
            .ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(login);
        Assert.NotEmpty(login.AccessToken);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken);

        return client;
    }

    [Fact]
    public async Task Get_MemberWithAdditionalProfileFields_ReturnsMappedValues()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();
        await factory.EnsureRoleExistsAsync(AkRoles.Balett);

        using (var scope = factory.Services.CreateScope())
        {
            var userManager =
                scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();

            var user = await userManager.FindByNameAsync(
                TestUsers.MemberUserName);

            Assert.NotNull(user);

            user.Email = "member@example.com";
            user.Phone = "0701234567";
            user.OtherInstruments = "Trumpet, Klarinett";
            user.SlavPoster = "[\"Ordförande\",\"Sekreterare\"]";
            user.Medal = "Guld";
            user.GivenMedal = "Silver";

            Assert.True((await userManager.UpdateAsync(user)).Succeeded);
            Assert.True((await userManager.AddToRoleAsync(
                user, AkRoles.Balett)).Succeeded);
        }

        var client = await CreateMobileClientAsync(factory);
        var response = await client.GetAsync("/api/v1/profile");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var profile = await response.Content
            .ReadFromJsonAsync<ProfileResponse>();

        Assert.NotNull(profile);
        Assert.Equal("member@example.com", profile.Email);
        Assert.Equal("0701234567", profile.Phone);
        Assert.Equal(["Trumpet", " Klarinett"], profile.OtherInstruments);
        Assert.Contains(AkRoles.Medlem, profile.Roles);
        Assert.Contains(AkRoles.Balett, profile.Roles);
        Assert.Equal(["Ordförande", "Sekreterare"], profile.Posts);
        Assert.Equal("Guld", profile.Medal);
        Assert.Equal("Silver", profile.GivenMedal);
    }

    [Fact]
    public async Task Get_LegacyNullPositions_ReturnsEmptyPositions()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        using (var scope = factory.Services.CreateScope())
        {
            var userManager =
                scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();

            var user = await userManager.FindByNameAsync(
                TestUsers.MemberUserName);

            Assert.NotNull(user);

            user.SlavPoster = "[null]";

            Assert.True((await userManager.UpdateAsync(user)).Succeeded);
        }

        var client = await CreateMobileClientAsync(factory);
        var response = await client.GetAsync("/api/v1/profile");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var profile = await response.Content
            .ReadFromJsonAsync<ProfileResponse>();

        Assert.NotNull(profile);
        Assert.Empty(profile.Posts);
    }

    [Fact]
    public async Task Get_DoesNotReturnAnotherMembersProfile()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        await factory.SeedMemberAsync(
            userName: "another-member",
            instrument: "Trumpet");

        var client = await CreateMobileClientAsync(factory);

        var response = await client.GetAsync("/api/v1/profile");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var profile = await response.Content
            .ReadFromJsonAsync<ProfileResponse>();

        Assert.NotNull(profile);
        Assert.Equal(TestUsers.MemberUserName, profile.UserName);
        Assert.Equal("Flöjt", profile.Instrument);
    }

    private sealed class LoginResponse
    {
        public string AccessToken { get; set; } = "";
    }
}
