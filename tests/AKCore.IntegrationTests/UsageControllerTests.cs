using System.Net;
using System.Text.Json;
using AKCore.DataModel;
using AKCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests;

public class UsageControllerTests
{
    [Fact]
    public async Task Record_AllowsAnonymous()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.PostAsync(
            $"/Usage/Record?type={AkFeatureUsageTypes.FlojtEvent}",
            null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Record_RejectsMissingType()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateMemberClient(factory);
        var response = await client.PostAsync("/Usage/Record", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Record_RejectsInvalidType()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateMemberClient(factory);
        var response = await client.PostAsync("/Usage/Record?type=UnknownFeature", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(AkFeatureUsageTypes.AlbumAIGeneration)]
    [InlineData(AkFeatureUsageTypes.EventTranslation)]
    [InlineData(AkFeatureUsageTypes.PageTranslation)]
    [InlineData(AkFeatureUsageTypes.FlojtEvent)]
    public async Task Record_AcceptsValidTypes(string type)
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateMemberClient(factory);

        var response = await client.PostAsync($"/Usage/Record?type={type}", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(json.RootElement.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task Record_FlushesAggregatedAmountToDatabase()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateMemberClient(factory);

        await client.PostAsync($"/Usage/Record?type={AkFeatureUsageTypes.AlbumAIGeneration}", null);
        await client.PostAsync($"/Usage/Record?type={AkFeatureUsageTypes.AlbumAIGeneration}", null);
        await client.PostAsync($"/Usage/Record?type={AkFeatureUsageTypes.EventTranslation}", null);

        var collector = factory.Services.GetRequiredService<UsageCollector>();
        await collector.FlushAsync();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var rows = await db.UsageDatas.OrderBy(x => x.Type).ToListAsync();

        Assert.Equal(2, rows.Count);

        var albumRow = Assert.Single(rows, r => r.Type == AkFeatureUsageTypes.AlbumAIGeneration);
        Assert.Equal(2, albumRow.Amount);

        var eventRow = Assert.Single(rows, r => r.Type == AkFeatureUsageTypes.EventTranslation);
        Assert.Equal(1, eventRow.Amount);
    }
}
