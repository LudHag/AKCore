using AKCore.DataModel;
using System.Collections.Generic;
using System.Linq;

namespace AKCore.Models
{
    public class MemberListModel
    {
        public IList<IGrouping<string, AkUser>> Users { get; set; }
        public string SearchPhrase { get; set; }
        public string Instrument { get; set; }
    }
}