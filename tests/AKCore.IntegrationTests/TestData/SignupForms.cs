using AKCore.DataModel;

namespace AKCore.IntegrationTests.TestData;

public static class SignupForms
{
    public static FormUrlEncodedContent Create(
        string where = AkSignupType.Halan,
        bool car = false,
        bool instrument = false,
        string? comment = null,
        string? selectedInstrument = null) =>
        new(new Dictionary<string, string?>
        {
            ["Where"] = where,
            ["Car"] = car.ToString().ToLowerInvariant(),
            ["Instrument"] = instrument.ToString().ToLowerInvariant(),
            ["Comment"] = comment,
            ["SelectedInstrument"] = selectedInstrument
        }.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value!));
}
