using System.Collections.Generic;
using AKCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AKCore.DataModel;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AKCore.Controllers
{
    [Route("Edit")]
    [Authorize(Roles= "SuperNintendo,Editor")]
    public class EditController : Controller
    {
        [Authorize(Roles = "SuperNintendo,Editor")]
        public ActionResult Index()
        {
            ViewBag.Title = "Editera sidor";
            using (var db = new AKContext())
            {
                var pages = db.Pages.ToList();
                var model = new EditPagesModel
                {
                    Pages = pages
                };
                return View(model);
            }
        }

        [HttpPost]
        [Route("CreatePage")]
        [Authorize(Roles = "SuperNintendo")]
        public ActionResult CreatePage(string name, string slug, string loggedIn)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(slug))
            {
                return Json(new {success = false, message = "Alla fällt måste vara ifyllda"});
            }

            using (var db = new AKContext())
            {
                if (slug.Length>1 && slug[0] != '/')
                {
                    slug = "/" + slug;
                }
                if (db.Pages.Any(x => x.Slug == slug))
                {
                    return Json(new {success = false, message = "Sidlänk måste vara unik"});
                }
                var page = new Page
                {
                    Name = name,
                    Slug = slug,
                    LoggedIn = loggedIn=="on"
                };
                db.Pages.Add(page);
                db.SaveChanges();
                var id = db.Pages.First(x => x.Slug == slug).Id;
                return Json(new {success = true, redirect = "/Edit/Page/" + id});
            }
        }

        [Route("Page/{id:int}")]
        [Authorize(Roles = "SuperNintendo,Editor")]
        public ActionResult Page(string id)
        {
            var pId = 0;
            int.TryParse(id, out pId);
            if (pId == 0)
            {
                return Redirect("/Edit");
            }
            using (var db = new AKContext())
            {
                var page = db.Pages.FirstOrDefault(x => x.Id == pId);
                if (page == null)
                {
                    return Redirect("/Edit");
                }
                var model = new PageEditModel
                {
                    Name = page.Name,
                    Slug = page.Slug,
                    WidgetsJson = page.WidgetsJson,
                    LoggedIn = page.LoggedIn,
                    Widgets = page.WidgetsJson != null ? JsonConvert.DeserializeObject<List<Widget>>(page.WidgetsJson) : new List<Widget>()
                };
                ViewBag.Title = "Editera " + page.Name;
                return View("EditPage", model);
            }
        }
        [HttpPost]
        [Route("Page/{id:int}")]
        [Authorize(Roles = "SuperNintendo,Editor")]
        public ActionResult Page(PageEditModel model,string id)
        {
            var pId = 0;
            int.TryParse(id, out pId);
            if (pId == 0)
            {
                return Json(new { success = false, message="Could not parse id" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Felaktigt ifyllda fällt" });
            }

            using (var db = new AKContext())
            {
                var page = db.Pages.FirstOrDefault(x => x.Id == pId);
                if (page == null)
                {
                    return Json(new { success = false, message = "Could not find page with id id" });
                }
                page.Name = model.Name;
                page.Slug = model.Slug;
                page.WidgetsJson = model.WidgetsJson;
                page.LoggedIn = model.LoggedIn;
                db.SaveChanges();

                return Json(new { success = true, message = "Uppdaterade sidan framgångsrikt" });
            }
        }

        [Route("RemovePage/{id:int}")]
        [Authorize(Roles = "SuperNintendo")]
        public ActionResult RemovePage(string id)
        {
            var pId = 0;
            int.TryParse(id, out pId);
            if (pId == 0)
            {
                return Redirect("/Edit");
            }
            using (var db = new AKContext())
            {
                var page = db.Pages.FirstOrDefault(x => x.Id == pId);
                if (page == null)
                {
                    return Redirect("/Edit");
                }
                db.Pages.Remove(page);
                db.SaveChanges();

                return Redirect("/Edit");
            }
        }
    }
}