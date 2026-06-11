using System.Net;
using System.Text.RegularExpressions;
using AKCore.DataModel;
using AKCore.IntegrationTests.TestData;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests;

public class LogControllerTests
{
    [Fact]
    public async Task Index_RequiresSuperNintendoRole()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateMemberClient(factory);
        var response = await client.GetAsync("/Log");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Index_EmptyLog_ReturnsOkWithNoItems()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.GetAsync("/Log?p=0");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(0, CountLogItems(body));
        Assert.DoesNotContain("pagination", body);
    }

    [Fact]
    public async Task Index_FirstPage_ShowsUpToTwentyItems()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();
        await SeedLogsAsync(factory, 25);

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.GetAsync("/Log?p=0");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(20, CountLogItems(body));
        Assert.Contains("pagination", body);
    }

    [Fact]
    public async Task Index_SecondPage_ShowsRemainingItems()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();
        await SeedLogsAsync(factory, 25);

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.GetAsync("/Log?p=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(5, CountLogItems(body));
    }

    [Fact]
    public async Task Index_PageBeyondTotal_ReturnsEmptyItems()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();
        await SeedLogsAsync(factory, 3);

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.GetAsync("/Log?p=99");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(0, CountLogItems(body));
    }

    [Fact]
    public async Task Index_FilterByType_ReturnsMatchingRows()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();
        await factory.SeedAsync(db =>
        {
            db.Log.Add(new LogItem { Type = AkLogTypes.Menus, Comment = "Menu action", Modified = DateTime.UtcNow });
            db.Log.Add(new LogItem { Type = AkLogTypes.Events, Comment = "Event action", Modified = DateTime.UtcNow });
            return Task.CompletedTask;
        });

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.GetAsync("/Log?p=0&type=Menus");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(1, CountLogItems(body));
        Assert.Contains("Menu action", body);
        Assert.DoesNotContain("Event action", body);
    }

    private static Task SeedLogsAsync(CustomWebApplicationFactory factory, int count)
    {
        var baseTime = DateTime.UtcNow;
        return factory.SeedAsync(db =>
        {
            for (var i = 0; i < count; i++)
            {
                db.Log.Add(new LogItem
                {
                    Type = AkLogTypes.Events,
                    Comment = $"Log entry {i:D3}",
                    Modified = baseTime.AddMinutes(-i)
                });
            }
            return Task.CompletedTask;
        });
    }

    private static int CountLogItems(string html) =>
        Regex.Matches(html, @"<div class=""log-item row"">").Count;
}
