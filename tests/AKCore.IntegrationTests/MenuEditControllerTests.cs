using System.Net;
using System.Text.Json;
using AKCore.DataModel;
using AKCore.IntegrationTests.TestData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests;

public class MenuEditControllerTests
{
    [Fact]
    public async Task AddTopMenu_CreatesMenuAndLogRow()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync("/MenuEdit/AddTopMenu", MenuEditForms.AddTopMenu(TestMenus.TopMenuName));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.True(json.RootElement.GetProperty("success").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        Assert.Contains(db.Menus, m => m.Name == TestMenus.TopMenuName);
        await LogAssertions.AssertLogExistsAsync(db, AkLogTypes.Menus, "Toppmeny med namn " + TestMenus.TopMenuName + " skapas");
    }

    [Fact]
    public async Task AddTopMenu_EmptyName_DoesNotMutateOrLog()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync("/MenuEdit/AddTopMenu", MenuEditForms.AddTopMenu(""));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.False(json.RootElement.GetProperty("success").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        Assert.Empty(db.Menus);
        Assert.Empty(db.Log);
    }

    [Fact]
    public async Task EditMenu_UpdatesMenuAndLogRow()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();
        await factory.SeedAsync(db =>
        {
            db.Menus.Add(TestMenus.TopMenu());
            return Task.CompletedTask;
        });

        using var seedScope = factory.Services.CreateScope();
        var menuId = seedScope.ServiceProvider.GetRequiredService<AKContext>().Menus.Single().Id;

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync(
            "/MenuEdit/EditMenu",
            MenuEditForms.EditTopMenu(menuId, TestMenus.EditedTopMenuName));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.True(json.RootElement.GetProperty("success").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var menu = await db.Menus.SingleAsync(m => m.Id == menuId);
        Assert.Equal(TestMenus.EditedTopMenuName, menu.Name);
        await LogAssertions.AssertLogExistsAsync(db, AkLogTypes.Menus, "Meny med id " + menuId + " redigeras");
    }

    [Fact]
    public async Task RemoveTopMenu_RemovesMenuAndChildrenAndLogRow()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();
        var menu = TestMenus.TopMenu();
        menu.Children = [TestMenus.SubMenu()];
        await factory.SeedAsync(db =>
        {
            db.Menus.Add(menu);
            return Task.CompletedTask;
        });

        using var seedScope = factory.Services.CreateScope();
        var dbSeed = seedScope.ServiceProvider.GetRequiredService<AKContext>();
        var menuId = dbSeed.Menus.Include(m => m.Children).Single().Id;

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync("/MenuEdit/RemoveTopMenu", MenuEditForms.RemoveTopMenu(menuId));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        Assert.Empty(db.Menus);
        Assert.Empty(db.SubMenus);
        await LogAssertions.AssertLogExistsAsync(db, AkLogTypes.Menus, TestMenus.TopMenuName + " tas bort");
    }

    [Fact]
    public async Task RemoveSubMenu_RemovesSubMenuAndLogRow()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();
        var menu = TestMenus.TopMenu();
        menu.Children = [TestMenus.SubMenu()];
        await factory.SeedAsync(db =>
        {
            db.Menus.Add(menu);
            return Task.CompletedTask;
        });

        using var seedScope = factory.Services.CreateScope();
        var subMenuId = seedScope.ServiceProvider.GetRequiredService<AKContext>()
            .SubMenus.Single().Id;

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync("/MenuEdit/RemoveSubMenu", MenuEditForms.RemoveSubMenu(subMenuId));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        Assert.Empty(db.SubMenus);
        Assert.Single(db.Menus);
        await LogAssertions.AssertLogExistsAsync(db, AkLogTypes.Menus, TestMenus.SubMenuName + " tas bort");
    }

    [Fact]
    public async Task MoveLeft_SwapsPositionAndCreatesLogRow()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();
        await factory.SeedAsync(db =>
        {
            db.Menus.Add(TestMenus.TopMenu("First", 0));
            db.Menus.Add(TestMenus.TopMenu("Second", 1));
            return Task.CompletedTask;
        });

        using var seedScope = factory.Services.CreateScope();
        var menus = seedScope.ServiceProvider.GetRequiredService<AKContext>().Menus.OrderBy(m => m.PosIndex).ToList();
        var secondMenuId = menus[1].Id;

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync("/MenuEdit/MoveLeft", MenuEditForms.MoveLeft(secondMenuId));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var updated = db.Menus.OrderBy(m => m.PosIndex).ToList();
        Assert.Equal("Second", updated[0].Name);
        Assert.Equal("First", updated[1].Name);
        await LogAssertions.AssertLogExistsAsync(db, AkLogTypes.Menus, "Meny med id " + secondMenuId + " flyttas");
    }

    [Fact]
    public async Task MoveRight_SwapsPositionAndCreatesLogRow()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();
        await factory.SeedAsync(db =>
        {
            db.Menus.Add(TestMenus.TopMenu("First", 0));
            db.Menus.Add(TestMenus.TopMenu("Second", 1));
            return Task.CompletedTask;
        });

        using var seedScope = factory.Services.CreateScope();
        var firstMenuId = seedScope.ServiceProvider.GetRequiredService<AKContext>().Menus.OrderBy(m => m.PosIndex).First().Id;

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync("/MenuEdit/MoveRight", MenuEditForms.MoveRight(firstMenuId));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var updated = db.Menus.OrderBy(m => m.PosIndex).ToList();
        Assert.Equal("Second", updated[0].Name);
        Assert.Equal("First", updated[1].Name);
        await LogAssertions.AssertLogExistsAsync(db, AkLogTypes.Menus, "Meny med id " + firstMenuId + " flyttas");
    }
}
