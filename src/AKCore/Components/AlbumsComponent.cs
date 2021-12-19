using AKCore.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            var model = widget.Albums.Select(a => _db.Albums.Include(x => x.Tracks).FirstOrDefault(x => x.Id == a)).Where(album => album != null).ToList();
            return View(model);
        }
    }
}