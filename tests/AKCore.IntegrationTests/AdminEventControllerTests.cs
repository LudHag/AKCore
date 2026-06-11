using System.Net;
using System.Text.Json;
using AKCore.DataModel;
using AKCore.IntegrationTests.TestData;
using Microsoft.EntityFrameworkCore;
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
    public async Task Edit_CreatesNewEventAndLogRow()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();

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
        await LogAssertions.AssertLogExistsAsync(db, AkLogTypes.Events, "Händelse med namn " + TestEvents.CreateEventName + " skapas");
    }

    [Fact]
    public async Task Edit_UpdatesEventAndLogRow()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();
        var eventId = await factory.SeedEventAndReturnIdAsync(TestEvents.EvenemangForGet());
        const string updatedName = "Updated Event Name";

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync(
            "/AdminEvent/Edit",
            AdminEventForms.UpdateEvenemang(eventId, updatedName));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.True(json.RootElement.GetProperty("success").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var evt = await db.Events.SingleAsync(e => e.Id == eventId);
        Assert.Equal(updatedName, evt.Name);
        await LogAssertions.AssertLogExistsAsync(db, AkLogTypes.Events, "Händelse med id " + eventId + " redigeras");
    }

    [Fact]
    public async Task Edit_UpdateMissingEvent_FailsWithoutLog()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync(
            "/AdminEvent/Edit",
            AdminEventForms.UpdateEvenemang(99999, "Missing"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.False(json.RootElement.GetProperty("success").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        Assert.Empty(db.Log);
    }

    [Fact]
    public async Task Remove_RemovesEventAndLogRow()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();
        var eventId = await factory.SeedEventAndReturnIdAsync(TestEvents.EvenemangForGet());

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync($"/AdminEvent/Remove/{eventId}", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.True(json.RootElement.GetProperty("success").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        Assert.DoesNotContain(db.Events, e => e.Id == eventId);
        await LogAssertions.AssertLogExistsAsync(db, AkLogTypes.Events, "Händelse med id " + eventId + " tas bort");
    }

    [Fact]
    public async Task Remove_MissingEvent_FailsWithoutLog()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAdminAsync();

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.PostAsync("/AdminEvent/Remove/99999", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.False(json.RootElement.GetProperty("success").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        Assert.Empty(db.Log);
    }

    [Fact]
    public async Task EventData_PageZero_NormalizesToPageOne()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAsync(TestEvents.UpcomingEvenemang());

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.GetAsync("/AdminEvent/EventData?old=false&page=0");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.Equal(1, json.RootElement.GetProperty("currentPage").GetInt32());
    }

    [Fact]
    public async Task EventData_SecondPage_ReturnsCorrectSlice()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAsync(db =>
        {
            for (var i = 0; i < 21; i++)
            {
                db.Events.Add(TestEvents.Create($"Paged Event {i:D2}", daysFromNow: 10 + i));
            }
            return Task.CompletedTask;
        });

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.GetAsync("/AdminEvent/EventData?old=false&page=2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.Equal(2, json.RootElement.GetProperty("currentPage").GetInt32());
        Assert.Equal(2, json.RootElement.GetProperty("totalPages").GetInt32());
        Assert.Single(json.RootElement.GetProperty("events").EnumerateArray());
    }

    [Fact]
    public async Task EventData_PageBeyondTotal_ReturnsEmptyEvents()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAsync(TestEvents.UpcomingEvenemang());

        var client = TestClients.CreateAdminClient(factory);
        var response = await client.GetAsync("/AdminEvent/EventData?old=false&page=99");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.Equal(99, json.RootElement.GetProperty("currentPage").GetInt32());
        Assert.Empty(json.RootElement.GetProperty("events").EnumerateArray());
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
