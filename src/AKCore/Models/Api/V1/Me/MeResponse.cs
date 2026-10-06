using System.Collections.Generic;

namespace AKCore.Models.Api.V1.Me;

public class MeResponse
{
    public string DisplayName { get; set; } = "";
    public bool IsMember { get; set; }
    public bool IsBallet { get; set; }
    public IList<string> AvailableInstruments { get; set; } = [];
}
