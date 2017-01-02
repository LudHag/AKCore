using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AKCore.DataModel;
using Microsoft.AspNetCore.Mvc;

namespace AKCore.Controllers
{
    public class AlbumsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IList<int> albumInts)
        {
            using (var db = new AKContext())
            {
                var model = albumInts.Select(a => db.Albums.FirstOrDefault(x => x.Id == a)).Where(album => album != null).ToList();
                return View(model);
            }
        }
    }
}