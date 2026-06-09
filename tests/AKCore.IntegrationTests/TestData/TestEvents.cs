using AKCore.DataModel;

namespace AKCore.IntegrationTests.TestData;

public static class TestEvents
{
    public const string UpcomingListName = "Test Spelning";
    public const string IcalEventName = "Ical Test Event";
    public const string AdminListName = "Admin List Event";
    public const string GetEventName = "Get Event Test";
    public const string CreateEventName = "New Created Event";
    public const string AdminCreatedUpcomingName = "Admin Created Upcoming Event";

    public static Event Create(
        string name,
        string type = AkEventTypes.Spelning,
        string place = "Testplats",
        int daysFromNow = 7,
        bool secret = false) =>
        new()
        {
            Type = type,
            Name = name,
            Place = place,
            Day = DateTime.UtcNow.Date.AddDays(daysFromNow),
            Secret = secret
        };

    public static Event PublicSpelning() =>
        Create(UpcomingListName, daysFromNow: 7);

    public static Event IcalSpelning() =>
        Create(IcalEventName, place: "Konserthuset", daysFromNow: 14);

    public static Event UpcomingEvenemang() =>
        Create(AdminListName, AkEventTypes.Evenemang, "Hålan", daysFromNow: 10);

    public static Event EvenemangForGet() =>
        Create(GetEventName, AkEventTypes.Evenemang, "Hålan", daysFromNow: 5);
}
