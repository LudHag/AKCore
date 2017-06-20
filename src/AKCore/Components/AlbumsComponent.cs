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
        private readonly IHostingEnvironment _hostingEnv;

        public AlbumsViewComponent(IHostingEnvironment env)
        {
            _hostingEnv = env;
        }

        public IViewComponentResult Invoke(Widget widget)
        {
            using (var db = new AKContext(_hostingEnv))
            {
                var model = widget.Albums.Select(a => db.Albums.Include(x=>x.Tracks).FirstOrDefault(x => x.Id == a)).Where(album => album != null).ToList();
                return View(model);
            }
        }
    }
}