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
            return revisions?.Select(x => x.Map());
        }

        public static RevisionViewModel Map(this Revision revision)
        {
            return new RevisionViewModel
            {
                Id = revision.Id,
                Name = revision.Name,
                Slug = revision.Slug,
                Widgets = revision.WidgetsJson.GetWidgetsFromString(),
                MetaDescription = revision.MetaDescription ?? "",
                LoggedIn = revision.LoggedIn,
                LoggedOut = revision.LoggedOut,
                BalettOnly = revision.BalettOnly,
                Modified = revision.Modified.ToString("yy-MM-dd HH:mm"),
                ModifiedBy = revision.ModifiedBy.GetName()

            };
        }

    }
}
