using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AKCore.DataModel;
using AKCore.IntegrationTests.TestData;
using AKCore.Models.Api.V1.Me;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests.Api.V1;

public class MeControllerTests
{
    [Fact]
    public async Task Get_WithoutBearerToken_ReturnsUnauthorized()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.GetAsync("/api/v1/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_AuthenticatedMember_ReturnsMinimumMemberInformation()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = await CreateMobileClientAsync(factory);

        var response = await client.GetAsync("/api/v1/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<MeResponse>();

        Assert.NotNull(body);
        Assert.Equal("Test Member", body.DisplayName);
        Assert.True(body.IsMember);
        Assert.False(body.IsBallet);
        Assert.Equal(["Flöjt"], body.AvailableInstruments);
    }

    [Fact]
    public async Task Get_BalletInstrumentWithoutBalletRole_ReturnsIsBalletFalse()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync(
            instrument: AkInstruments.Balett);

        var client = await CreateMobileClientAsync(factory);

        var response = await client.GetAsync("/api/v1/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<MeResponse>();

        Assert.NotNull(body);
        Assert.True(body.IsMember);
        Assert.False(body.IsBallet);
        Assert.Equal(
            [AkInstruments.Balett],
            body.AvailableInstruments);
    }

    [Fact]
    public async Task Get_BalletRole_ReturnsIsBalletTrue()
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

            var result = await userManager.AddToRoleAsync(
                user,
                AkRoles.Balett);

            Assert.True(result.Succeeded);
        }

        var client = await CreateMobileClientAsync(factory);

        var response = await client.GetAsync("/api/v1/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<MeResponse>();

        Assert.NotNull(body);
        Assert.True(body.IsMember);
        Assert.True(body.IsBallet);
        Assert.Equal(
            ["Flöjt"],
            body.AvailableInstruments);
    }

    [Fact]
    public async Task Get_MemberWithOtherInstruments_ReturnsExistingInstrumentSemantics()
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

            user.OtherInstruments = "Trumpet, Klarinett,Flöjt";

            var result = await userManager.UpdateAsync(user);

            Assert.True(result.Succeeded);
        }

        var client = await CreateMobileClientAsync(factory);

        var response = await client.GetAsync("/api/v1/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<MeResponse>();

        Assert.NotNull(body);
        Assert.Equal(
            ["Flöjt", "Trumpet", "Klarinett"],
            body.AvailableInstruments);
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

    private sealed class LoginResponse
    {
        public string AccessToken { get; set; } = "";
    }
}
