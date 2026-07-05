using AKCore.DataModel;
using AKCore.IntegrationTests.TestData;

namespace AKCore.IntegrationTests;

public static class TestClients
{
    public static HttpClient CreateAdminClient(CustomWebApplicationFactory factory) =>
        CreateAuthenticatedClient(factory, TestUsers.AdminUserName, AkRoles.SuperNintendo);

    public static HttpClient CreateEditorClient(CustomWebApplicationFactory factory) =>
        CreateAuthenticatedClient(factory, TestUsers.EditorUserName, AkRoles.Editor);

    public static HttpClient CreateMemberClient(CustomWebApplicationFactory factory) =>
        CreateMemberClient(factory, TestUsers.MemberUserName);

    public static HttpClient CreateMemberClient(CustomWebApplicationFactory factory, string userName) =>
        CreateAuthenticatedClient(factory, userName, AkRoles.Medlem);

    public static HttpClient CreateAnonymousClient(CustomWebApplicationFactory factory) =>
        factory.CreateClientWithHttpsBaseAddress();

    public static HttpClient CreateAuthenticatedClient(
        CustomWebApplicationFactory factory,
        string userName,
        string roles)
    {
        var client = factory.CreateClientWithHttpsBaseAddress();
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserHeader, userName);
        client.DefaultRequestHeaders.Add(TestAuthHandler.RolesHeader, roles);
        return client;
    }
}
