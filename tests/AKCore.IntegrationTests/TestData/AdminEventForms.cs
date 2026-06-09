using AKCore.DataModel;

namespace AKCore.IntegrationTests.TestData;

public static class AdminEventForms
{
    public static FormUrlEncodedContent CreateEvenemang(
        string name,
        string day = "2030-01-01",
        string place = "Hålan") =>
        new(new Dictionary<string, string>
        {
            ["Type"] = AkEventTypes.Evenemang,
            ["Name"] = name,
            ["Day"] = day,
            ["Place"] = place
        });
}
