using System.Collections.Generic;

namespace AKCore.Models.Api.V1.Events;

public class EventDetailResponse
{
    public int Id { get; set; }

    public string Type { get; set; } = "";

    public string Name { get; set; } = "";

    public string Place { get; set; } = "";

    public string Description { get; set; } = "";

    public string InternalDescription { get; set; } = "";

    public string Date { get; set; } = "";

    public string HalanTime { get; set; } = "";

    public string ThereTime { get; set; } = "";

    public string StartsTime { get; set; } = "";

    public string PlayDuration { get; set; } = "";

    public string Stand { get; set; } = "";

    public string SignupState { get; set; }

    public int Coming { get; set; }

    public int NotComing { get; set; }

    public bool Disabled { get; set; }

    public bool RegistrationAvailable { get; set; }

    public EventRegistrationSelectionResponse Registration { get; set; } = new();

    public IList<EventAttendeeResponse> Attendees { get; set; } = [];
}

public class EventRegistrationSelectionResponse
{
    public string Where { get; set; }

    public bool Car { get; set; }

    public bool Instrument { get; set; } = true;

    public string Comment { get; set; } = "";

    public string SelectedInstrument { get; set; }

    public IList<string> AvailableInstruments { get; set; } = [];
}

public class EventAttendeeResponse
{
    public string PersonName { get; set; } = "";

    public string Where { get; set; }

    public bool Car { get; set; }

    public bool Instrument { get; set; }

    public string InstrumentName { get; set; }

    public string Comment { get; set; } = "";
}
