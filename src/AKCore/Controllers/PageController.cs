using System.Collections.Generic;
using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AKCore.Controllers
{
    public class PageController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Title"] = "Home";
            var model = new PageRenderModel();

            return View(model);
        }

        public ActionResult Page(string slug)
        {
            var loggedIn = User.Identity.IsAuthenticated;
            using (var db = new AKContext())
            {
                Page page;
                if (string.IsNullOrWhiteSpace(slug))
                {
                    page = db.Pages.FirstOrDefault(x => x.Slug == "/");
                }
                else
                {
                    page = db.Pages.FirstOrDefault(x => x.Slug == ("/" + slug));
                }
                if (page == null)
                {
                    return View("Error");
                }
                if (page.LoggedIn && !loggedIn)
                {
                    return Redirect("/");
                }
                if (page.LoggedOut && loggedIn)
                {
                    return Redirect("/");
                }
                ViewData["Title"] = page.Name;
                var model = new PageRenderModel()
                {
                    Widgets = page.WidgetsJson != null ? JsonConvert.DeserializeObject<List<Widget>>(page.WidgetsJson) : new List<Widget>()
                };
                return View("Index", model);
            }
        }
    }
}