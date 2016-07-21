using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace AKCore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Title"] = "Home";

            var model = new PageRenderModel();


            return View(model);
        }

        public ActionResult Page(string slug)
        {
            using (var db = new AKContext())
            {
                Page page;
                if (string.IsNullOrWhiteSpace(slug))
                {
                    page = db.Pages.FirstOrDefault(x => x.Path == "/");
                }
                else
                {
                    page = db.Pages.FirstOrDefault(x => x.Path == ("/" + slug));
                }
                if (page == null)
                {
                    return View("Error");
                }
                var model = new PageRenderModel()
                {
                    PageInfo = page
                };
                return View("Index", model);
            }
        }
    }
}