using System.Collections.Generic;
using System.Linq;
using AKCore.DataModel;

namespace AKCore.Models
{
    public class UpcommingModel
    {
        public IEnumerable<IGrouping<int,Event>> Events { get; set; }
    }
}