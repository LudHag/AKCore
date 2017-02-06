using System.Collections.Generic;
using System.Linq;
using AKCore.DataModel;

namespace AKCore.Models
{
    public class MemberListModel
    {
        public IList<IGrouping<string,AkUser>> Users { get; set; }
        public string SearchPhrase { get; set; }
        public string Instrument { get; set; }
    }
}