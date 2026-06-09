using System.Net;
using System.Text.Json;
using AKCore.DataModel;
using AKCore.IntegrationTests.TestData;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests;

public class AdminEventControllerTests
{
    [Fact]
    public async Task EventData_ReturnsUpcomingEvents()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAsync(TestEvents.UpcomingEvenemang());

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.GetAsync("/AdminEvent/EventData?old=false&page=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);

        Assert.Equal(1, json.RootElement.GetProperty("currentPage").GetInt32());
        Assert.Contains(TestEvents.AdminListName, body);
    }

    [Fact]
    public async Task GetEvent_ReturnsSeededEvent()
    {
        await using var factory = new CustomWebApplicationFactory();
        var eventId = await factory.SeedEventAndReturnIdAsync(TestEvents.EvenemangForGet());

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.GetAsync($"/AdminEvent/GetEvent/{eventId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);

        Assert.True(json.RootElement.GetProperty("success").GetBoolean());
        Assert.Contains(TestEvents.GetEventName, json.RootElement.GetProperty("e").GetString()!);
    }

    [Fact]
    public async Task Edit_CreatesNewEvent()
    {
        await using var factory = new CustomWebApplicationFactory();

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync(
            "/AdminEvent/Edit",
            AdminEventForms.CreateEvenemang(TestEvents.CreateEventName));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.True(json.RootElement.GetProperty("success").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        Assert.Contains(db.Events, e => e.Name == TestEvents.CreateEventName);
    }

    [Fact]
    public async Task CreatedInAdmin_AppearsInUpcomingList()
    {
        await using var factory = new CustomWebApplicationFactory();
        var adminClient = TestClients.CreateAdminClient(factory);

        var createResponse = await adminClient.PostAsync(
            "/AdminEvent/Edit",
            AdminEventForms.CreateEvenemang(TestEvents.AdminCreatedUpcomingName));

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        var createBody = await createResponse.Content.ReadAsStringAsync();
        using var createJson = JsonDocument.Parse(createBody);
        Assert.True(createJson.RootElement.GetProperty("success").GetBoolean());

        var upcomingClient = TestClients.CreateAnonymousClient(factory);
        var listResponse = await upcomingClient.GetAsync("/upcoming/UpcomingListData");

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var listBody = await listResponse.Content.ReadAsStringAsync();
        Assert.Contains(TestEvents.AdminCreatedUpcomingName, listBody);
    }
}
