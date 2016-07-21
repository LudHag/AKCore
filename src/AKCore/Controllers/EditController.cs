using AKCore.Helpers;
using AKCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AKCore.DataModel;
using Microsoft.AspNetCore.Authorization;

namespace AKCore.Controllers
{
    [Route("Edit")]
    [Authorize(Roles="SuperNintendo")]
    public class EditController : Controller
    {
        //[MyAuthorize(Roles = "SuperNintendo,Nintendo,Styrelse")]
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
        //[MyAuthorize(Roles = "SuperNintendo,Nintendo,Styrelse")]
        public ActionResult CreatePage(string name, string slug)
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
                    Authority = 0,
                    Name = name,
                    Slug = slug,
                    Path = slug
                };
                db.Pages.Add(page);
                db.SaveChanges();
                var id = db.Pages.First(x => x.Slug == slug).Id;
                return Json(new {success = true, redirect = "/Edit/Page/" + id});
            }
        }

        //[MyAuthorize(Roles = "SuperNintendo,Nintendo,Styrelse")]
        [Route("Page/{id:int}")]
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
                int parent = 0;
                if (page.Parent != null)
                {
                    parent = page.Parent.Id;
                }
                var model = new PageEditModel
                {
                    Name = page.Name,
                    Parent = parent,
                    Slug = page.Slug,
                    Content = page.Content
                };
                ViewBag.Title = "Editera " + page.Name;
                return View("EditPage", model);
            }
        }
        //[MyAuthorize(Roles = "SuperNintendo,Nintendo,Styrelse")]
        [HttpPost]
        [Route("Page/{id:int}")]
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
                page.Path = model.Slug;
                page.Content = model.Content;
                db.SaveChanges();

                return Json(new { success = true, message = "Uppdaterade sidan framgångsrikt" });
            }
        }

        //[MyAuthorize(Roles = "SuperNintendo,Nintendo,Styrelse")]
        [Route("RemovePage/{id:int}")]
        public ActionResult RemovePage(string id)
        {
            int pId = 0;
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