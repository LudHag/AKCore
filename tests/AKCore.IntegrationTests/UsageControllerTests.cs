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
    public async Task Track_RequiresAuthentication()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.PostAsync(
            $"/Usage/Track?type={AkFeatureUsageTypes.AlbumAIGeneration}",
            null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Track_RejectsMissingType()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateMemberClient(factory);
        var response = await client.PostAsync("/Usage/Track", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Track_RejectsInvalidType()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateMemberClient(factory);
        var response = await client.PostAsync("/Usage/Track?type=UnknownFeature", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Track_AcceptsValidTypes()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateMemberClient(factory);

        var albumResponse = await client.PostAsync(
            $"/Usage/Track?type={AkFeatureUsageTypes.AlbumAIGeneration}",
            null);
        var eventResponse = await client.PostAsync(
            $"/Usage/Track?type={AkFeatureUsageTypes.EventTranslation}",
            null);

        Assert.Equal(HttpStatusCode.OK, albumResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, eventResponse.StatusCode);

        using var albumJson = JsonDocument.Parse(await albumResponse.Content.ReadAsStringAsync());
        using var eventJson = JsonDocument.Parse(await eventResponse.Content.ReadAsStringAsync());
        Assert.True(albumJson.RootElement.GetProperty("success").GetBoolean());
        Assert.True(eventJson.RootElement.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task Track_FlushesAggregatedAmountToDatabase()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateMemberClient(factory);

        await client.PostAsync($"/Usage/Track?type={AkFeatureUsageTypes.AlbumAIGeneration}", null);
        await client.PostAsync($"/Usage/Track?type={AkFeatureUsageTypes.AlbumAIGeneration}", null);
        await client.PostAsync($"/Usage/Track?type={AkFeatureUsageTypes.EventTranslation}", null);

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
