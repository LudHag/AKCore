using AKCore.DataModel;
using System.Collections.Generic;

namespace AKCore.Models
{
    public class PostModel
    {
        public IDictionary<string, IList<AkUser>> Users { get; set; }
    }
}