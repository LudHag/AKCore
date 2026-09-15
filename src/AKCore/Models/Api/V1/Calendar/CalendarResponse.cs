using System.Collections.Generic;

namespace AKCore.Models.Api.V1.Calendar;

public class CalendarResponse
{
    public IList<CalendarEventResponse> Events { get; set; } = [];
}

public class CalendarEventResponse
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
}
