using AKCore.DataModel;
using AKCore.Models;
using System.Collections.Generic;
using System.Linq;

namespace AKCore.Extensions
{
    public static class RevisionExtensions
    {
        public static IEnumerable<RevisionViewModel> Map(this IEnumerable<Revision> revisions)
        {
            return revisions?.Select(x => new RevisionViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Slug = x.Slug,
                Widgets = x.WidgetsJson.GetWidgetsFromString(),
                LoggedIn = x.LoggedIn,
                LoggedOut = x.LoggedOut,
                BalettOnly = x.BalettOnly,
                Modified = x.Modified.ToString("yy-MM-dd HH:mm"),
                ModifiedBy = x.ModifiedBy.GetName()

            });
        }

    }
}
