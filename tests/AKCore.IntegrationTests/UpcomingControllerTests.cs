using AKCore.IntegrationTests.TestData;

namespace AKCore.IntegrationTests;

public class UpcomingControllerTests
{
    [Fact]
    public async Task UpcomingListData_ReturnsFuturePublicEvents()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAsync(TestEvents.PublicSpelning());

        var client = factory.CreateClientWithHttpsBaseAddress();
        var response = await client.GetAsync("/upcoming/UpcomingListData");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains(TestEvents.UpcomingListName, body);
    }

    [Fact]
    public async Task Ical_ReturnsCalendarWithFutureEvents()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedAsync(TestEvents.IcalSpelning());

        var client = factory.CreateClientWithHttpsBaseAddress();
        var response = await client.GetAsync("/upcoming/akevents.ics");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/octet-stream", response.Content.Headers.ContentType?.MediaType);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("BEGIN:VCALENDAR", body);
        Assert.Contains(TestEvents.IcalEventName, body);
    }
}
