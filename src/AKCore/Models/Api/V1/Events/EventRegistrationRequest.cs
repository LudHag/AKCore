namespace AKCore.Models.Api.V1.Events;

public class EventRegistrationRequest
{
    public string Where { get; set; } = "";

    public bool Car { get; set; }

    public bool Instrument { get; set; } = true;

    public string Comment { get; set; }

    public string SelectedInstrument { get; set; }
}
