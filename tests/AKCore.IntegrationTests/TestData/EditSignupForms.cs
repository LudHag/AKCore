namespace AKCore.IntegrationTests.TestData;

public static class EditSignupForms
{
    public static FormUrlEncodedContent Create(
        int eventId,
        string memberId,
        string type,
        bool instrument = false,
        bool car = false) =>
        new(new Dictionary<string, string>
        {
            ["eventId"] = eventId.ToString(),
            ["memberId"] = memberId,
            ["type"] = type,
            ["instrument"] = instrument.ToString().ToLowerInvariant(),
            ["car"] = car.ToString().ToLowerInvariant()
        });
}
