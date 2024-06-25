using AKCore.DataModel;
using System.Collections.Generic;
using System.Linq;

namespace AKCore.Models;

public record StructuredDataEvent(string Name, string Description, string StartsTime, string Place, bool LastElement);

public class StructuredDataModel
{
    public IEnumerable<StructuredDataEvent> UpcomingGigs { get; set; }
}

