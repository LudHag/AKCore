using AKCore.DataModel;
using AKCore.Extensions;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Controllers
{
    [Route("Edit")]
    [Authorize(Roles = "SuperNintendo,Editor")]
    public class EditController : Controller
    {
        private readonly AKContext _db;
        private readonly UserManager<AkUser> _userManager;

        public EditController(AKContext db, UserManager<AkUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [Authorize(Roles = "SuperNintendo,Editor")]
        public ActionResult Index()
        {
            ViewBag.Title = "Redigera sidor";
            var pages = _db.Pages.OrderBy(x => x.Name).ToList();
            var model = new EditPagesModel
            {
                Pages = pages
            };
            return View(model);
        }

        [HttpPost]
        [Route("CreatePage")]
        [Authorize(Roles = "SuperNintendo")]
        public async Task<ActionResult> CreatePage(string name, string slug, string loggedIn)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(slug))
            {
                return Json(new { success = false, message = "Alla fällt måste vara ifyllda" });
            }

            if (slug.Length > 1 && slug[0] != '/')
            {
                slug = "/" + slug;
            }
            if (_db.Pages.Any(x => x.Slug == slug))
            {
                return Json(new { success = false, message = "Sidlänk måste vara unik" });
            }
            var page = new Page
            {
                Name = name,
                Slug = slug,
                LoggedIn = loggedIn == "on",
                LastModified = DateTime.Now.ConvertToSwedishTime(),
            };
            _db.Pages.Add(page);

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            _db.Log.Add(new LogItem()
            {
                Type = AkLogTypes.Page,
                Modified = DateTime.Now.ConvertToSwedishTime(),
                ModifiedBy = user,
                Comment = "Sida skapad med namn " + name
            });

            _db.SaveChanges();
            var id = _db.Pages.First(x => x.Slug == slug).Id;
            return Json(new { success = true, redirect = "/Edit/Page/" + id });
        }

        [Route("Page/{id:int}")]
        [Authorize(Roles = "SuperNintendo,Editor")]
        public ActionResult Page(int id)
        {
            var page = _db.Pages.Include(x => x.Revisions).ThenInclude(x => x.ModifiedBy).FirstOrDefault(x => x.Id == id);
            if (page == null)
            {
                return Redirect("/Edit");
            }

            ViewBag.Title = "Redigera " + page.Name;
            return View("EditPage");
        }

        private PageEditModel GetPageModel(int id)
        {
            var page = _db.Pages.Include(x => x.Revisions).ThenInclude(x => x.ModifiedBy).FirstOrDefault(x => x.Id == id);
            if (page == null)
            {
                return null;
            }
            return new PageEditModel
            {
                Name = page.Name,
                Slug = page.Slug,
                PageId = page.Id,
                LastModified = page.LastModified,
                LoggedIn = page.LoggedIn,
                LoggedOut = page.LoggedOut,
                BalettOnly = page.BalettOnly,
                Albums = _db.Albums.ToList(),
                Widgets = page.WidgetsJson.GetWidgetsFromString(),
                Revisions = page.Revisions?.SkipLast(1).Map()
            };
        }

        [HttpGet("Page/{id:int}/model")]
        [Authorize(Roles = "SuperNintendo,Editor")]
        public ActionResult PageEditModel(int id)
        {
            var model = GetPageModel(id);
            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }

        [HttpPost("Page/{id:int}")]
        [Authorize(Roles = "SuperNintendo,Editor")]
        public async Task<ActionResult> Page([FromBody] PageEditModel model, int id)
        {
            if (id == 0)
            {
                return Json(new { success = false, message = "Could not parse id" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Felaktigt ifyllda fällt" });
            }

            var page = _db.Pages.Include(x => x.Revisions).ThenInclude(x => x.ModifiedBy).FirstOrDefault(x => x.Id == id);
            if (page == null)
            {
                return Json(new { success = false, message = "Could not find page with id id" });
            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (page.Revisions == null)
            {
                page.Revisions = new List<Revision>();
            }
            else if (page.Revisions.Count > 5)
            {
                var oldestRevision = page.Revisions.OrderBy(x => x.Modified).FirstOrDefault();
                if (oldestRevision != null) _db.Revisions.Remove(oldestRevision);
            }

            page.Revisions.Add(new Revision()
            {
                BalettOnly = model.BalettOnly,
                LoggedIn = model.LoggedIn,
                LoggedOut = model.LoggedOut,
                Modified = DateTime.Now.ConvertToSwedishTime(),
                ModifiedBy = user,
                Name = model.Name,
                Slug = model.Slug,
                WidgetsJson = JsonConvert.SerializeObject(model.Widgets)
            });

            page.Name = model.Name;
            page.Slug = model.Slug;
            page.WidgetsJson = JsonConvert.SerializeObject(model.Widgets);
            page.LoggedIn = model.LoggedIn;
            page.LoggedOut = model.LoggedOut;
            page.BalettOnly = model.BalettOnly;
            page.LastModified = DateTime.Now.ConvertToSwedishTime();

            _db.Log.Add(new LogItem()
            {
                Type = AkLogTypes.Page,
                Modified = DateTime.Now.ConvertToSwedishTime(),
                ModifiedBy = user,
                Comment = "Sida med namn " + model.Name + " uppdaterad"
            });

            _db.SaveChanges();
            var newModel = GetPageModel(id);
            return Json(new { success = true, message = "Uppdaterade sidan framgångsrikt", newModel });
        }

        [Route("RemovePage/{id:int}")]
        [Authorize(Roles = "SuperNintendo")]
        public async Task<ActionResult> RemovePage(string id)
        {
            if (!int.TryParse(id, out var pId))
            {
                return Redirect("/Edit");
            }
            var page = _db.Pages.Include(x => x.Revisions).FirstOrDefault(x => x.Id == pId);

            if (page?.Revisions != null)
            {
                foreach (var rev in page.Revisions)
                {
                    _db.Revisions.Remove(rev);
                }
            }

            var topmenus = _db.Menus.Where(x => x.Link == page).ToList();
            foreach (var m in topmenus)
            {
                m.Link = null;
            }
            var subMenus = _db.SubMenus.Where(x => x.Link == page).ToList();
            foreach (var m in subMenus)
            {
                _db.SubMenus.Remove(m);
            }

            if (page == null)
            {
                return Redirect("/Edit");
            }
            var pageName = page.Name;

            _db.Pages.Remove(page);

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            _db.Log.Add(new LogItem()
            {
                Type = AkLogTypes.Page,
                Modified = DateTime.Now.ConvertToSwedishTime(),
                ModifiedBy = user,
                Comment = "Sida med namn " + pageName + " borttagen"
            });

            _db.SaveChanges();

            return Redirect("/Edit");
        }
    }
}