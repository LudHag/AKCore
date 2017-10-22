using System;
using AKCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AKCore.DataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace AKCore.Controllers
{
    [Route("MenuEdit")]
    [Authorize(Roles = "SuperNintendo")]
    public class MenuEditController : Controller
    {
        private readonly AKContext _db;

        public MenuEditController(AKContext db)
        {
            _db = db;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Redigera Menyer";
            var pages = _db.Pages.OrderBy(x=>x.Name).ToList();
            var menus = _db.Menus.Include(x=>x.Children).OrderBy(x=>x.PosIndex).ToList();
            var modelMenus = menus.Select(m => new ModelMenu(m,true)).ToList();

            var model = new MenuEditModel
            {
                Pages = pages,
                Menus = modelMenus
            };
            return View(model);
        }

        [Route("MenuList")]
        public ActionResult MenuList()
        {
            var pages = _db.Pages.OrderBy(x => x.Name).ToList();
            var menus = _db.Menus.Include(x=>x.Children).OrderBy(x => x.PosIndex).ToList();
            var modelMenus = menus.Select(m => new ModelMenu(m,true)).ToList();
            var model = new MenuEditModel
            {
                Pages = pages,
                Menus = modelMenus
            };
            return View("_MenuList", model);
        }

        [HttpPost]
        [Route("AddTopMenu")]
        public ActionResult AddTopMenu(string name, string pageId, string loggedIn, string balett)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Json(new {success = false, message = "Menyn måste ha ett namn"});
            }

            var m = new Menu
            {
                Name = name,
                LoggedIn = loggedIn!=null,
                Balett = balett!=null
            };
            var highIndex=_db.Menus.OrderByDescending(z => z.PosIndex).FirstOrDefault();
            if (highIndex != null)
            {
                m.PosIndex = highIndex.PosIndex + 1;
            }
            if (!string.IsNullOrWhiteSpace(pageId))
            {
                if (int.TryParse(pageId, out int id))
                {
                    var page = _db.Pages.FirstOrDefault(x => x.Id == id);
                    if (page != null)
                    {
                        m.Link = page;
                    }
                }
            }
            _db.Menus.Add(m);
            _db.SaveChanges();
            return Json(new {success = true});
        }

        [HttpPost]
        [Route("EditMenu")]
        public ActionResult EditMenu(string parentId, string menuId, string text, string pageId, string loggedIn, string balett)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Json(new {success = false, message = "Menyn måste ha ett namn"});
            }

            if (parentId != "true")
            {
                if (!int.TryParse(menuId, out int id))
                {
                    return Json(new { success = false, message = "Felaktigt menyid" });
                }
                var menu = _db.Menus.FirstOrDefault(x => x.Id == id);
                if (menu == null)
                {
                    return Json(new {success = false, message = "Meny finns ej"});
                }
                menu.Name = text;
                if (int.TryParse(pageId, out int pId))
                {
                    var page = _db.Pages.FirstOrDefault(x => x.Id == pId);
                    menu.Link = page;
                }
                else
                {
                    menu.Link = null;
                }
                menu.LoggedIn = loggedIn != null;
                menu.Balett = balett != null;
            }
            else
            {
                if (!int.TryParse(menuId, out var id))
                {
                    return Json(new { success = false, message = "Felaktigt menyid" });
                }
                var menu = _db.SubMenus.FirstOrDefault(x => x.Id == id);
                if (menu == null)
                {
                    return Json(new {success = false, message = "Submeny finns ej"});
                }
                menu.Name = text;
                if (int.TryParse(pageId, out int pId))
                {
                    var page = _db.Pages.FirstOrDefault(x => x.Id == pId);
                    menu.Link = page;
                }
                else
                {
                    menu.Link = null;
                }
            }
           
            var res = _db.SaveChanges();
            return Json(new {success = res>0 });
        }

        [HttpPost]
        [Route("AddSubMenu")]
        public ActionResult AddSubMenu(string parentId, string pageId, string text)
        {
            if (string.IsNullOrWhiteSpace(parentId) || string.IsNullOrWhiteSpace(pageId) || string.IsNullOrWhiteSpace(text))
            {
                return Json(new {success = false, message = "Otillräcklig input"});
            }

            if (int.TryParse(parentId, out int pid))
            {
                var parent = _db.Menus.Include(z => z.Children).FirstOrDefault(x => x.Id == pid);
                if (parent == null)
                {
                    return Json(new { success = false, message = "Föräldrameny finns ej" });
                }
                var m = new SubMenu
                {
                    Name = text
                };
                var highIndex = parent.Children.OrderByDescending(z => z.SubPosIndex).FirstOrDefault();
                if (highIndex != null)
                {
                    m.SubPosIndex = highIndex.SubPosIndex + 1;
                }

                if (int.TryParse(pageId, out int id))
                {
                    var page = _db.Pages.FirstOrDefault(x => x.Id == id);
                    if (page != null)
                    {
                        m.Link = page;
                    }
                }
                parent.Children.Add(m);

                _db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new {success = false, message = "Felaktigt format på föräldrameny"});
        }

        [HttpPost]
        [Route("RemoveTopMenu")]
        public ActionResult RemoveTopMenu(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new {success = false, message = "Inget id inskickat"});
            }
            if (!int.TryParse(id, out int i))
            {
                return Json(new { success = false, message = "Ej numeriskt id" });
            }
            var menu = _db.Menus.Include(x=>x.Children).FirstOrDefault(x => x.Id == i);
            if (menu == null)
            {
                return Json(new {success = false, message = "Meny finns ej"});
            }
            while (menu.Children.Count > 0)
            {
                _db.SubMenus.Remove(menu.Children.First());
            }

            _db.Menus.Remove(menu);
            _db.SaveChanges();
            return Json(new {success = true});
        }

        [HttpPost]
        [Route("MoveLeft")]
        public ActionResult MoveLeft(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new {success = false, message = "Inget id inskickat"});
            }
            if (!int.TryParse(id, out int i))
            {
                return Json(new { success = false, message = "Ej numeriskt id" });
            }
            var menu = _db.Menus.FirstOrDefault(x => x.Id == i);
            if (menu == null)
            {
                return Json(new { success = false, message = "Meny finns ej" });
            }
            var menu2 = _db.Menus.Where(x => x.PosIndex < menu.PosIndex).OrderByDescending(x => x.PosIndex).FirstOrDefault();
            if (menu2!=null)
            {
                var tempPos = menu.PosIndex;
                menu.PosIndex = menu2.PosIndex;
                menu2.PosIndex = tempPos;
            }

            _db.SaveChanges();
            return Json(new {success = true});
        }

        [HttpPost]
        [Route("MoveRight")]
        public ActionResult MoveRight(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new { success = false, message = "Inget id inskickat" });
            }
            if (!int.TryParse(id, out int i))
            {
                return Json(new { success = false, message = "Ej numeriskt id" });
            }
            var menu = _db.Menus.FirstOrDefault(x => x.Id == i);
            if (menu == null)
            {
                return Json(new { success = false, message = "Meny finns ej" });
            }
            var menu2 = _db.Menus.Where(x => x.PosIndex > menu.PosIndex).OrderBy(x => x.PosIndex).FirstOrDefault();
            if (menu2 != null)
            {
                var tempPos = menu.PosIndex;
                menu.PosIndex = menu2.PosIndex;
                menu2.PosIndex = tempPos;
            }

            _db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        [Route("MoveUp")]
        public ActionResult MoveUp(string id,string parent)
        {
            if (string.IsNullOrWhiteSpace(id)||string.IsNullOrWhiteSpace(parent))
            {
                return Json(new { success = false, message = "Inget id eller topmeny inskickat" });
            }
            if (!int.TryParse(id, out int i))
            {
                return Json(new { success = false, message = "Ej numeriskt id" });
            }
            var menu = _db.SubMenus.FirstOrDefault(x => x.Id == i);
            if (menu == null)
            {
                return Json(new { success = false, message = "Meny finns ej" });
            }
            var tId = 0;
            if (!int.TryParse(parent, out tId))
            {
                return Json(new { success = false, message = "Ej numeriskt id" });
            }
            var topmenu = _db.Menus.Include(x=>x.Children).FirstOrDefault(x => x.Id == tId);
            if (topmenu != null)
            {
                var menu2 = topmenu.Children.Where(x => x.SubPosIndex < menu.SubPosIndex).OrderByDescending(x => x.SubPosIndex).FirstOrDefault();

                if (menu2 != null)
                {
                    var tempPos = menu.SubPosIndex;
                    menu.SubPosIndex = menu2.SubPosIndex;
                    menu2.SubPosIndex = tempPos;
                }
                    
                _db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Topmeny finns ej" });
            }
        }

        [HttpPost]
        [Route("MoveDown")]
        public ActionResult MoveDown(string id, string parent)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(parent))
            {
                return Json(new { success = false, message = "Inget id eller topmeny inskickat" });
            }
            if (!int.TryParse(id, out int i))
            {
                return Json(new { success = false, message = "Ej numeriskt id" });
            }
            var menu = _db.SubMenus.FirstOrDefault(x => x.Id == i);
            if (menu == null)
            {
                return Json(new { success = false, message = "Meny finns ej" });
            }
            if (!int.TryParse(parent, out int tId))
            {
                return Json(new { success = false, message = "Ej numeriskt id" });
            }
            var topmenu = _db.Menus.Include(x => x.Children).FirstOrDefault(x => x.Id == tId);
            if (topmenu != null)
            {
                var menu2 = topmenu.Children.Where(x => x.SubPosIndex > menu.SubPosIndex).OrderBy(x => x.SubPosIndex).FirstOrDefault();

                if (menu2 != null)
                {
                    var tempPos = menu.SubPosIndex;
                    menu.SubPosIndex = menu2.SubPosIndex;
                    menu2.SubPosIndex = tempPos;
                }

                _db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Topmeny finns ej" });
            }
        }

        [HttpPost]
        [Route("RemoveSubMenu")]
        public ActionResult RemoveSubMenu(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new {success = false, message = "Inget id inskickat"});
            }
            if (!int.TryParse(id, out int i))
            {
                return Json(new { success = false, message = "Ej numeriskt id" });
            }
            var menu = _db.SubMenus.FirstOrDefault(x => x.Id == i);
            if (menu == null)
            {
                return Json(new {success = false, message = "Meny finns ej"});
            }

            _db.SubMenus.Remove(menu);
            _db.SaveChanges();
            return Json(new {success = true});
        }
    }
}