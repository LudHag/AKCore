using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AKCore.DataModel;

namespace AKCore.Models
{
    public class PostModel
    {
        public IDictionary<string, IList<AkUser>> Users { get; set; }
    }
}