using AKCore.DataModel;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests;

public class MenuEditControllerTests
{
    [Fact]
    public async Task AddTopMenu_PersistsMenuAndReturnsSuccess()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = TestClients.CreateAdminClient(factory);

        var response = await client.PostAsync(
            "/MenuEdit/AddTopMenu",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["name"] = "Testmeny",
            }));

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"success\":true", json);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        Assert.Contains(db.Menus, m => m.Name == "Testmeny");
    }

    [Fact]
    public async Task EditMenu_UpdatesNameAndReturnsSuccess()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAsync(db =>
        {
            db.Menus.Add(new Menu { Name = "Original", PosIndex = 0 });
            return Task.CompletedTask;
        });

        var client = TestClients.CreateAdminClient(factory);

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AKContext>();
            var menuId = db.Menus.Single().Id;

            var response = await client.PostAsync(
                "/MenuEdit/EditMenu",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["menuId"] = menuId.ToString(),
                    ["text"] = "Uppdaterad",
                    ["textEng"] = "Updated",
                }));

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            Assert.Contains("\"success\":true", json);
        }

        using var verifyScope = factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AKContext>();
        Assert.Equal("Uppdaterad", verifyDb.Menus.Single().Name);
    }
}
