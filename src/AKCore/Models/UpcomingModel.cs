using System.Collections.Generic;
using System.Linq;
using AKCore.DataModel;

namespace AKCore.Models
{
    public class UpcomingModel
    {
        public IEnumerable<IGrouping<int,Event>> Events { get; set; }
        public bool LoggedIn { get; set; }
        public bool Medlem { get; set; }
        public string UserId { get; set; }
        public string ICalLink { get; set; }
    }
}