using System.Collections.Generic;
using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.EntityFrameworkCore;

namespace AKCore.Helpers
{
    public static class MenuHelper
    {
        public static List<ModelMenu> GetMenus()
        {
            using (var db = new AKContext())
            {
                var menus = db.Menus.OrderBy(x => x.PosIndex)
                    .Include(b => b.Link)
                    .Include(x => x.Children)
                    .ThenInclude(x=>x.Link)
                    .ToList();
                var modelMenus = menus.Select(m => new ModelMenu(m)).ToList();
                return modelMenus;
            }
        }
    }
}