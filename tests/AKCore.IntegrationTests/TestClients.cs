using AKCore.DataModel;

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

    public static HttpClient CreateAnonymousClient(CustomWebApplicationFactory factory) =>
        factory.CreateClientWithHttpsBaseAddress();
}
