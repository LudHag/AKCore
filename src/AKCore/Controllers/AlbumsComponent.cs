using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AKCore.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AKCore.Controllers
{
    public class AlbumsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(Widget widget)
        {
            using (var db = new AKContext())
            {
                var model = widget.Albums.Select(a => db.Albums.Include(x=>x.Tracks).FirstOrDefault(x => x.Id == a)).Where(album => album != null).ToList();
                return View(model);
            }
        }
    }
}