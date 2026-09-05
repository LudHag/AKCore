using System.Net;
using System.Text.Json;
using AKCore.DataModel;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests;

public class StatisticsControllerTests
{
    [Fact]
    public async Task FeatureUsage_RequiresSuperNintendoRole()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateMemberClient(factory);
        var response = await client.GetAsync("/Statistics/FeatureUsage");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task FeatureUsage_GroupsByTypeAndSumsAmounts()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();

        var now = DateTime.UtcNow;
        await factory.SeedAsync(db =>
        {
            db.UsageDatas.AddRange(
                new UsageData
                {
                    Type = AkFeatureUsageTypes.AlbumAIGeneration,
                    Amount = 2,
                    Created = now.AddHours(-2)
                },
                new UsageData
                {
                    Type = AkFeatureUsageTypes.AlbumAIGeneration,
                    Amount = 3,
                    Created = now.AddHours(-1)
                },
                new UsageData
                {
                    Type = AkFeatureUsageTypes.EventTranslation,
                    Amount = 1,
                    Created = now.AddHours(-3)
                });
            return Task.CompletedTask;
        });

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.GetAsync("/Statistics/FeatureUsage?range=day");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var items = json.RootElement.GetProperty("items").EnumerateArray().ToList();

        Assert.Equal(2, items.Count);

        var album = Assert.Single(items, i => i.GetProperty("type").GetString() == AkFeatureUsageTypes.AlbumAIGeneration);
        var albumTotal = album.GetProperty("items").EnumerateArray().Sum(i => i.GetProperty("amount").GetInt32());
        Assert.Equal(5, albumTotal);

        var eventTranslation = Assert.Single(items, i => i.GetProperty("type").GetString() == AkFeatureUsageTypes.EventTranslation);
        var eventTotal = eventTranslation.GetProperty("items").EnumerateArray().Sum(i => i.GetProperty("amount").GetInt32());
        Assert.Equal(1, eventTotal);
    }

    [Fact]
    public async Task FeatureUsage_RespectsRangeFilter()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();

        var now = DateTime.UtcNow;
        await factory.SeedAsync(db =>
        {
            db.UsageDatas.AddRange(
                new UsageData
                {
                    Type = AkFeatureUsageTypes.FlojtEvent,
                    Amount = 10,
                    Created = now.AddDays(-10)
                },
                new UsageData
                {
                    Type = AkFeatureUsageTypes.PageTranslation,
                    Amount = 4,
                    Created = now.AddHours(-2)
                });
            return Task.CompletedTask;
        });

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.GetAsync("/Statistics/FeatureUsage?range=day");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var items = json.RootElement.GetProperty("items").EnumerateArray().ToList();

        var pageTranslation = Assert.Single(items);
        Assert.Equal(AkFeatureUsageTypes.PageTranslation, pageTranslation.GetProperty("type").GetString());

        var total = pageTranslation.GetProperty("items").EnumerateArray().Sum(i => i.GetProperty("amount").GetInt32());
        Assert.Equal(4, total);
    }
}
