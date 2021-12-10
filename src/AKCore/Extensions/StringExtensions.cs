using AKCore.DataModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AKCore.Extensions
{
    public static class StringExtensions
    {
        public static IList<Widget> GetWidgetsFromString(this string widgetJson)
        {
            var widgetList = widgetJson != null ? JsonConvert.DeserializeObject<List<Widget>>(widgetJson) : new List<Widget>();
            foreach (var (widget, id) in widgetList.Select((value, i) => (value, i)))
            {
                widget.Id = id;
            }

            return widgetList;
        }

    }
}
