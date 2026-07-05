using System.Net;
using System.Text;
using System.Text.Json;
using AKCore.DataModel;
using AKCore.IntegrationTests.TestData;
using AKCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests;

public class UserControllerTests
{
    [Fact]
    public async Task CreateUser_CreatesUserAndLogRow()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();

        const string newUserName = "newmember";
        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync("/User/CreateUser", UserForms.CreateUser(newUserName));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.True(json.RootElement.GetProperty("success").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var user = await userManager.FindByNameAsync(newUserName);
        Assert.NotNull(user);
        await LogAssertions.AssertLogExistsAsync(db, AkLogTypes.User, "Användare med namn " + user!.GetName() + " skapad");
    }

    [Fact]
    public async Task CreateUser_DuplicateUserName_FailsWithoutLog()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync(
            "/User/CreateUser",
            UserForms.CreateUser(TestUsers.MemberUserName));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.False(json.RootElement.GetProperty("success").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        Assert.Empty(db.Log);
    }

    [Fact]
    public async Task EditUser_UpdatesUserAndLogRow()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();
        await factory.SeedMemberAsync();
        var memberId = await factory.SeedMemberAndReturnIdAsync();

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync(
            "/User/EditUser",
            UserForms.EditUser(memberId, TestUsers.MemberUserName, "Changed", "Member"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.True(json.RootElement.GetProperty("success").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var user = await db.Users.SingleAsync(u => u.Id == memberId);
        Assert.Equal("Changed", user.FirstName);
        await LogAssertions.AssertLogExistsAsync(db, AkLogTypes.User, "Användare med namn Changed Member redigeras");
    }

    [Fact]
    public async Task EditUser_MissingUser_FailsWithoutLog()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync(
            "/User/EditUser",
            UserForms.EditUser("missing-id", "ghost"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.False(json.RootElement.GetProperty("success").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        Assert.Empty(db.Log);
    }

    [Fact]
    public async Task AddRole_AddsRoleAndLogRow()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();
        await factory.SeedMemberAsync();
        await factory.EnsureRoleExistsAsync(AkRoles.Editor);

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync(
            "/User/AddRole",
            UserForms.AddRole(TestUsers.MemberUserName, AkRoles.Editor));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.True(json.RootElement.GetProperty("success").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var user = await userManager.FindByNameAsync(TestUsers.MemberUserName);
        var roles = await userManager.GetRolesAsync(user!);
        Assert.Contains(AkRoles.Editor, roles);
        await LogAssertions.AssertLogExistsAsync(
            db,
            AkLogTypes.User,
            TestUsers.MemberUserName + " får roll " + AkRoles.Editor + " tillagd");
    }

    [Fact]
    public async Task RemoveRole_RemovesRoleAndLogRow()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();
        await factory.SeedMemberAsync();
        await factory.EnsureRoleExistsAsync(AkRoles.Editor);

        var setupClient = TestClients.CreateAdminClient(factory);
        await setupClient.PostAsync("/User/AddRole", UserForms.AddRole(TestUsers.MemberUserName, AkRoles.Editor));

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync(
            "/User/RemoveRole",
            UserForms.RemoveRole(TestUsers.MemberUserName, AkRoles.Editor));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.True(json.RootElement.GetProperty("success").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var user = await userManager.FindByNameAsync(TestUsers.MemberUserName);
        var roles = await userManager.GetRolesAsync(user!);
        Assert.DoesNotContain(AkRoles.Editor, roles);
        await LogAssertions.AssertLogExistsAsync(
            db,
            AkLogTypes.User,
            TestUsers.MemberUserName + " får roll " + AkRoles.Editor + " borttagen");
    }

    [Fact]
    public async Task ChangePassword_ResetsPasswordAndLogRow()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync(
            "/User/ChangePassword",
            UserForms.ChangePassword(TestUsers.MemberUserName, "NewPassword2!"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.True(json.RootElement.GetProperty("success").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var user = await userManager.FindByNameAsync(TestUsers.MemberUserName);
        Assert.True(await userManager.CheckPasswordAsync(user!, "NewPassword2!"));
        await LogAssertions.AssertLogExistsAsync(
            db,
            AkLogTypes.User,
            TestUsers.MemberUserName + " får lösenord ändrat");
    }
}
