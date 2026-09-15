using System.Net;
using System.Net.Http.Json;
using AKCore.IntegrationTests.TestData;

namespace AKCore.IntegrationTests;

public class AccountControllerTests
{
    [Fact]
    public async Task Login_ValidCredentials_PreservesWebsiteCookieLoginBehavior()
    {
        await using var factory =
            new CustomWebApplicationFactory(
                useTestAuthentication: false);
        await factory.SeedMemberAsync();

        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.PostAsync(
            "/Account/Login",
            new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    ["Username"] = TestUsers.MemberUserName,
                    ["Password"] = TestUsers.DefaultPassword
                }));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body =
            await response.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(body);
        Assert.True(body.Success);

        Assert.True(
            response.Headers.TryGetValues("Set-Cookie", out var cookies));

        Assert.Contains(
            cookies,
            cookie => cookie.Contains(
                ".AspNetCore.Identity.Application"));
    }

    [Fact]
    public async Task Login_WrongPassword_PreservesWebsiteFailureBehavior()
    {
        await using var factory =
            new CustomWebApplicationFactory(
                useTestAuthentication: false);
        await factory.SeedMemberAsync();

        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.PostAsync(
            "/Account/Login",
            new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    ["Username"] = TestUsers.MemberUserName,
                    ["Password"] = "WrongPassword"
                }));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body =
            await response.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(body);
        Assert.False(body.Success);
        Assert.Equal(
            "Inloggning misslyckades",
            body.Message);
    }

    private sealed class LoginResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
