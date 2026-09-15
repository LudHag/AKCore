using AKCore.DataModel;
using AKCore.IntegrationTests.TestData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AKCore.IntegrationTests.Api.V1;

public class MobileDevicesControllerTests
{
    [Fact]
    public async Task Put_Anonymous_ReturnsUnauthorized()
    {
        await using var factory = new CustomWebApplicationFactory(
            useTestAuthentication: false);

        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.PutAsJsonAsync(
            "/api/v1/devices/test-installation",
            new
            {
                pushToken = "test-token",
                platform = "android"
            });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task Put_NewDevice_PersistsRegistration()
    {
        await using var factory = new CustomWebApplicationFactory(
            useTestAuthentication: false);
        await factory.SeedMemberAsync();

        var client = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var response = await client.PutAsJsonAsync(
            "/api/v1/devices/test-installation",
            new
            {
                pushToken = "test-token",
                platform = "android"
            });

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();

        var device = await db.MobileDevices.SingleAsync();

        Assert.Equal(
            "test-installation",
            device.InstallationId);

        Assert.Equal(
            "test-token",
            device.PushToken);

        Assert.Equal(
            "fcm",
            device.Provider);

        Assert.Equal(
            "android",
            device.Platform);

        var user = await db.Users.SingleAsync(
            x => x.UserName == TestUsers.MemberUserName);

        Assert.Equal(
            user.Id,
            device.UserId);

        Assert.Equal(
            device.CreatedAt,
            device.UpdatedAt);
    }

    [Fact]
    public async Task Put_ExistingInstallation_UpdatesTokenWithoutDuplicate()
    {
        await using var factory = new CustomWebApplicationFactory(
            useTestAuthentication: false);
        await factory.SeedMemberAsync();

        var client = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var firstResponse = await client.PutAsJsonAsync(
            "/api/v1/devices/test-installation",
            new
            {
                pushToken = "old-token",
                platform = "android"
            });

        Assert.Equal(
            HttpStatusCode.NoContent,
            firstResponse.StatusCode);

        var secondResponse = await client.PutAsJsonAsync(
            "/api/v1/devices/test-installation",
            new
            {
                pushToken = "new-token",
                platform = "android"
            });

        Assert.Equal(
            HttpStatusCode.NoContent,
            secondResponse.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();

        var device = Assert.Single(
            await db.MobileDevices.ToListAsync());

        Assert.Equal(
            "test-installation",
            device.InstallationId);

        Assert.Equal(
            "new-token",
            device.PushToken);
    }

    [Fact]
    public async Task Put_ExistingToken_MovesRegistrationWithoutDuplicate()
    {
        await using var factory = new CustomWebApplicationFactory(
            useTestAuthentication: false);
        await factory.SeedMemberAsync();

        var client = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var firstResponse = await client.PutAsJsonAsync(
            "/api/v1/devices/old-installation",
            new
            {
                pushToken = "test-token",
                platform = "android"
            });

        Assert.Equal(
            HttpStatusCode.NoContent,
            firstResponse.StatusCode);

        var secondResponse = await client.PutAsJsonAsync(
            "/api/v1/devices/new-installation",
            new
            {
                pushToken = "test-token",
                platform = "android"
            });

        Assert.Equal(
            HttpStatusCode.NoContent,
            secondResponse.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();

        var device = Assert.Single(
            await db.MobileDevices.ToListAsync());

        Assert.Equal(
            "new-installation",
            device.InstallationId);

        Assert.Equal(
            "test-token",
            device.PushToken);
    }

    [Fact]
    public async Task Put_InvalidPlatform_ReturnsBadRequest()
    {
        await using var factory = new CustomWebApplicationFactory(
            useTestAuthentication: false);
        await factory.SeedMemberAsync();

        var client = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var response = await client.PutAsJsonAsync(
            "/api/v1/devices/test-installation",
            new
            {
                pushToken = "test-token",
                platform = "ios"
            });

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();

        Assert.Empty(await db.MobileDevices.ToListAsync());
    }

    [Fact]
    public async Task Put_OtherUsersInstallation_ReassignsDeviceWithoutDuplicate()
    {
        await using var factory = new CustomWebApplicationFactory(
            useTestAuthentication: false);

        const string otherUserName = "other-member";

        await factory.SeedMemberAsync();
        var otherUserId = await factory.SeedMemberAndReturnIdAsync(
            otherUserName);

        var firstClient = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var firstResponse = await firstClient.PutAsJsonAsync(
            "/api/v1/devices/test-installation",
            new
            {
                pushToken = "first-token",
                platform = "android"
            });

        Assert.Equal(
            HttpStatusCode.NoContent,
            firstResponse.StatusCode);

        var otherClient = await CreateMobileClientAsync(
            factory,
            otherUserName);

        var secondResponse = await otherClient.PutAsJsonAsync(
            "/api/v1/devices/test-installation",
            new
            {
                pushToken = "second-token",
                platform = "android"
            });

        Assert.Equal(
            HttpStatusCode.NoContent,
            secondResponse.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();

        var device = Assert.Single(
            await db.MobileDevices.ToListAsync());

        Assert.Equal(
            "test-installation",
            device.InstallationId);

        Assert.Equal(
            "second-token",
            device.PushToken);

        Assert.Equal(
            otherUserId,
            device.UserId);
    }

    [Fact]
    public async Task Delete_OtherUsersDevice_DoesNotRemoveRegistration()
    {
        await using var factory = new CustomWebApplicationFactory(
            useTestAuthentication: false);

        const string otherUserName = "other-member";

        var ownerUserId = await factory.SeedMemberAndReturnIdAsync();
        await factory.SeedMemberAsync(otherUserName);

        var ownerClient = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var putResponse = await ownerClient.PutAsJsonAsync(
            "/api/v1/devices/test-installation",
            new
            {
                pushToken = "test-token",
                platform = "android"
            });

        Assert.Equal(
            HttpStatusCode.NoContent,
            putResponse.StatusCode);

        var otherClient = await CreateMobileClientAsync(
            factory,
            otherUserName);

        var response = await otherClient.DeleteAsync(
            "/api/v1/devices/test-installation");

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();

        var device = Assert.Single(
            await db.MobileDevices.ToListAsync());

        Assert.Equal(
            "test-installation",
            device.InstallationId);

        Assert.Equal(
            ownerUserId,
            device.UserId);
    }

    [Fact]
    public async Task Delete_OwnDevice_RemovesRegistration()
    {
        await using var factory = new CustomWebApplicationFactory(
            useTestAuthentication: false);
        await factory.SeedMemberAsync();

        var client = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var putResponse = await client.PutAsJsonAsync(
            "/api/v1/devices/test-installation",
            new
            {
                pushToken = "test-token",
                platform = "android"
            });

        Assert.Equal(
            HttpStatusCode.NoContent,
            putResponse.StatusCode);

        var response = await client.DeleteAsync(
            "/api/v1/devices/test-installation");

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();

        Assert.Empty(await db.MobileDevices.ToListAsync());
    }

    [Fact]
    public async Task Delete_UnknownDevice_RemainsSuccessful()
    {
        await using var factory = new CustomWebApplicationFactory(
            useTestAuthentication: false);
        await factory.SeedMemberAsync();

        var client = await CreateMobileClientAsync(
            factory,
            TestUsers.MemberUserName);

        var response = await client.DeleteAsync(
            "/api/v1/devices/not-registered");

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);
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

    private sealed class LoginResponse
    {
        public bool Authenticated { get; set; }

        public string AccessToken { get; set; } = "";

        public string RefreshToken { get; set; } = "";
    }
}
