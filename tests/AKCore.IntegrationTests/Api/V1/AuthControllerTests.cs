using System.Net;
using System.Net.Http.Json;
using AKCore.IntegrationTests.TestData;
using AKCore.DataModel;
using AKCore.Models.Api.V1.Auth;
using AKCore.Services.Api.V1.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests.Api.V1;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                username = TestUsers.MemberUserName,
                password = TestUsers.DefaultPassword
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(body);
        Assert.True(body.Authenticated);
        Assert.NotEmpty(body.AccessToken);
        Assert.NotEmpty(body.RefreshToken);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                username = TestUsers.MemberUserName,
                password = "WrongPassword"
            });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_UnknownUser_ReturnsUnauthorized()
    {
        await using var factory = new CustomWebApplicationFactory();

        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                username = "does-not-exist",
                password = TestUsers.DefaultPassword
            });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_MissingPassword_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory();

        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                username = TestUsers.MemberUserName
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_PersistsHashedRefreshSession()
    {
        await using var factory = new CustomWebApplicationFactory();
        var userId = await factory.SeedMemberAndReturnIdAsync();

        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                username = TestUsers.MemberUserName,
                password = TestUsers.DefaultPassword
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(body);
        Assert.NotEmpty(body.RefreshToken);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();

        var session = Assert.Single(db.MobileSessions);

        Assert.Equal(userId, session.UserId);
        Assert.NotEqual(body.RefreshToken, session.RefreshTokenHash);
        Assert.Equal(64, session.RefreshTokenHash.Length);
        Assert.Null(session.RevokedAt);
        Assert.True(session.ExpiresAt > session.CreatedAt);
    }

    [Fact]
    public async Task Refresh_ValidToken_RotatesRefreshSession()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateAnonymousClient(factory);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                username = TestUsers.MemberUserName,
                password = TestUsers.DefaultPassword
            });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginBody =
            await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(loginBody);

        var refreshResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new
            {
                refreshToken = loginBody.RefreshToken
            });

        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);

        var refreshBody =
            await refreshResponse.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(refreshBody);
        Assert.True(refreshBody.Authenticated);
        Assert.NotEmpty(refreshBody.AccessToken);
        Assert.NotEmpty(refreshBody.RefreshToken);
        Assert.NotEqual(
            loginBody.RefreshToken,
            refreshBody.RefreshToken);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();

        var sessions = await db.MobileSessions
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        Assert.Equal(2, sessions.Count);
        Assert.NotNull(sessions[0].RevokedAt);
        Assert.Null(sessions[1].RevokedAt);
    }

    [Fact]
    public async Task Refresh_ReusedToken_ReturnsUnauthorized()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateAnonymousClient(factory);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                username = TestUsers.MemberUserName,
                password = TestUsers.DefaultPassword
            });

        var loginBody =
            await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(loginBody);

        var firstRefresh = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new
            {
                refreshToken = loginBody.RefreshToken
            });

        Assert.Equal(HttpStatusCode.OK, firstRefresh.StatusCode);

        var reusedRefresh = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new
            {
                refreshToken = loginBody.RefreshToken
            });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            reusedRefresh.StatusCode);
    }

    [Fact]
    public async Task Refresh_UnknownToken_ReturnsUnauthorized()
    {
        await using var factory = new CustomWebApplicationFactory();

        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new
            {
                refreshToken = "not-a-real-refresh-token"
            });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task Refresh_ExpiredToken_ReturnsUnauthorized()
    {
        await using var factory = new CustomWebApplicationFactory();

        var userId = await factory.SeedMemberAndReturnIdAsync();

        const string refreshToken = "expired-refresh-token";

        await factory.SeedAsync(db =>
        {
            db.MobileSessions.Add(new MobileSession
            {
                UserId = userId,
                RefreshTokenHash =
                    new MobileTokenService(
                        Microsoft.Extensions.Options.Options.Create(
                            new MobileAuthOptions()))
                    .HashRefreshToken(refreshToken),
                CreatedAt = DateTime.UtcNow.AddDays(-31),
                ExpiresAt = DateTime.UtcNow.AddDays(-1)
            });

            return Task.CompletedTask;
        });

        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new
            {
                refreshToken
            });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task Logout_ValidRefreshToken_RevokesSession()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateAnonymousClient(factory);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                username = TestUsers.MemberUserName,
                password = TestUsers.DefaultPassword
            });

        var loginBody =
            await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(loginBody);

        var logoutResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/logout",
            new
            {
                refreshToken = loginBody.RefreshToken
            });

        Assert.Equal(
            HttpStatusCode.NoContent,
            logoutResponse.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();

        var session = await db.MobileSessions.SingleAsync();

        Assert.NotNull(session.RevokedAt);
    }

    [Fact]
    public async Task Logout_RevokedToken_RemainsSuccessful()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateAnonymousClient(factory);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                username = TestUsers.MemberUserName,
                password = TestUsers.DefaultPassword
            });

        var loginBody =
            await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(loginBody);

        var firstLogout = await client.PostAsJsonAsync(
            "/api/v1/auth/logout",
            new
            {
                refreshToken = loginBody.RefreshToken
            });

        Assert.Equal(
            HttpStatusCode.NoContent,
            firstLogout.StatusCode);

        var secondLogout = await client.PostAsJsonAsync(
            "/api/v1/auth/logout",
            new
            {
                refreshToken = loginBody.RefreshToken
            });

        Assert.Equal(
            HttpStatusCode.NoContent,
            secondLogout.StatusCode);
    }

    [Fact]
    public async Task Logout_UnknownToken_ReturnsNoContent()
    {
        await using var factory = new CustomWebApplicationFactory();

        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.PostAsJsonAsync(
            "/api/v1/auth/logout",
            new
            {
                refreshToken = "not-a-real-refresh-token"
            });

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);
    }

}
