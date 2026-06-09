using AKCore.DataModel;
using AKCore.IntegrationTests.TestData;

namespace AKCore.IntegrationTests;

public static class TestClients
{
    public static HttpClient CreateAdminClient(CustomWebApplicationFactory factory)
    {
        var client = factory.CreateClientWithHttpsBaseAddress();
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserHeader, "admin");
        client.DefaultRequestHeaders.Add(TestAuthHandler.RolesHeader, AkRoles.SuperNintendo);
        return client;
    }

    public static HttpClient CreateMemberClient(CustomWebApplicationFactory factory)
    {
        var client = factory.CreateClientWithHttpsBaseAddress();
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserHeader, TestUsers.MemberUserName);
        client.DefaultRequestHeaders.Add(TestAuthHandler.RolesHeader, AkRoles.Medlem);
        return client;
    }

    public static HttpClient CreateAnonymousClient(CustomWebApplicationFactory factory) =>
        factory.CreateClientWithHttpsBaseAddress();
}
