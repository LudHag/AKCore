using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AKCore.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace AKCore.Components
{
    public class AlbumsViewComponent : ViewComponent
    {
        private readonly AKContext _db;

        public AlbumsViewComponent(AKContext db)
        {
            _db = db;
        }

        public IViewComponentResult Invoke(Widget widget)
        {
            var model = widget.Albums.Select(a => _db.Albums.Include(x=>x.Tracks).FirstOrDefault(x => x.Id == a)).Where(album => album != null).OrderBy(x => x.Name).ToList();
            return View(model);
        }
    }
}