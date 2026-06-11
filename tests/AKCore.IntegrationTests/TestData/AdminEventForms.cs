using AKCore.DataModel;

namespace AKCore.IntegrationTests.TestData;

public static class AdminEventForms
{
    public static FormUrlEncodedContent CreateEvenemang(
        string name,
        int daysFromNow = 7,
        string place = "Hålan") =>
        new(new Dictionary<string, string>
        {
            ["Type"] = AkEventTypes.Evenemang,
            ["Name"] = name,
            ["Day"] = DateTime.UtcNow.Date.AddDays(daysFromNow).ToString("yyyy-MM-dd"),
            ["Place"] = place
        });

    public static FormUrlEncodedContent UpdateEvenemang(
        int id,
        string name,
        int daysFromNow = 7,
        string place = "Hålan") =>
        new(new Dictionary<string, string>
        {
            ["Id"] = id.ToString(),
            ["Type"] = AkEventTypes.Evenemang,
            ["Name"] = name,
            ["Day"] = DateTime.UtcNow.Date.AddDays(daysFromNow).ToString("yyyy-MM-dd"),
            ["Place"] = place
        });
}
