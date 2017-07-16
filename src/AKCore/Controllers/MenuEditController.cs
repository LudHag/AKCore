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
        private readonly IHostingEnvironment _hostingEnv;

        public MenuEditController(IHostingEnvironment env)
        {
            _hostingEnv = env;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Redigera Menyer";
            using (var db = new AKContext(_hostingEnv))
            {
                var pages = db.Pages.OrderBy(x=>x.Name).ToList();
                var menus = db.Menus.Include(x=>x.Children).OrderBy(x=>x.PosIndex).ToList();
                var modelMenus = menus.Select(m => new ModelMenu(m,true)).ToList();

                var model = new MenuEditModel
                {
                    Pages = pages,
                    Menus = modelMenus
                };
                return View(model);
            }
        }

        [Route("MenuList")]
        public ActionResult MenuList()
        {
            using (var db = new AKContext(_hostingEnv))
            {
                var pages = db.Pages.OrderBy(x => x.Name).ToList();
                var menus = db.Menus.Include(x=>x.Children).OrderBy(x => x.PosIndex).ToList();
                var modelMenus = menus.Select(m => new ModelMenu(m,true)).ToList();
                var model = new MenuEditModel
                {
                    Pages = pages,
                    Menus = modelMenus
                };
                return View("_MenuList", model);
            }
        }

        [HttpPost]
        [Route("AddTopMenu")]
        public ActionResult AddTopMenu(string name, string pageId, string loggedIn)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Json(new {success = false, message = "Menyn måste ha ett namn"});
            }

            using (var db = new AKContext(_hostingEnv))
            {
                var m = new Menu
                {
                    Name = name,
                    LoggedIn = loggedIn!=null
                };
                var highIndex=db.Menus.OrderByDescending(z => z.PosIndex).FirstOrDefault();
                if (highIndex != null)
                {
                    m.PosIndex = highIndex.PosIndex + 1;
                }
                if (!string.IsNullOrWhiteSpace(pageId))
                {
                    var id = 0;
                    if (int.TryParse(pageId, out id))
                    {
                        var page = db.Pages.FirstOrDefault(x => x.Id == id);
                        if (page != null)
                        {
                            m.Link = page;
                        }
                    }
                }
                db.Menus.Add(m);
                db.SaveChanges();
                return Json(new {success = true});
            }
        }

        [HttpPost]
        [Route("EditMenu")]
        public ActionResult EditMenu(string parentId, string menuId, string text, string pageId, string loggedIn)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Json(new {success = false, message = "Menyn måste ha ett namn"});
            }


            using (var db = new AKContext(_hostingEnv))
            {
                if (parentId != "true")
                {
                    if (!int.TryParse(menuId, out int id))
                    {
                        return Json(new { success = false, message = "Felaktigt menyid" });
                    }
                    var menu = db.Menus.FirstOrDefault(x => x.Id == id);
                    if (menu == null)
                    {
                        return Json(new {success = false, message = "Meny finns ej"});
                    }
                    menu.Name = text;
                    if (int.TryParse(pageId, out int pId))
                    {
                        var page = db.Pages.FirstOrDefault(x => x.Id == pId);
                        menu.Link = page;
                    }
                    else
                    {
                        menu.Link = null;
                    }
                    menu.LoggedIn = loggedIn != null;
                }
                else
                {
                    if (!int.TryParse(menuId, out int id))
                    {
                        return Json(new { success = false, message = "Felaktigt menyid" });
                    }
                    var menu = db.SubMenus.FirstOrDefault(x => x.Id == id);
                    if (menu == null)
                    {
                        return Json(new {success = false, message = "Submeny finns ej"});
                    }
                    menu.Name = text;
                    if (int.TryParse(pageId, out int pId))
                    {
                        var page = db.Pages.FirstOrDefault(x => x.Id == pId);
                        menu.Link = page;
                    }
                    else
                    {
                        menu.Link = null;
                    }
                }
                db.SaveChanges();
                return Json(new {success = true});
            }
        }

        [HttpPost]
        [Route("AddSubMenu")]
        public ActionResult AddSubMenu(string parentId, string pageId, string text)
        {
            if (string.IsNullOrWhiteSpace(parentId) || string.IsNullOrWhiteSpace(pageId) || string.IsNullOrWhiteSpace(text))
            {
                return Json(new {success = false, message = "Otillräcklig input"});
            }

            using (var db = new AKContext(_hostingEnv))
            {
                if (int.TryParse(parentId, out int pid))
                {
                    var parent = db.Menus.Include(z => z.Children).FirstOrDefault(x => x.Id == pid);
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
                        var page = db.Pages.FirstOrDefault(x => x.Id == id);
                        if (page != null)
                        {
                            m.Link = page;
                        }
                    }
                    parent.Children.Add(m);

                    db.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new {success = false, message = "Felaktigt format på föräldrameny"});
            }
        }

        [HttpPost]
        [Route("RemoveTopMenu")]
        public ActionResult RemoveTopMenu(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new {success = false, message = "Inget id inskickat"});
            }
            using (var db = new AKContext(_hostingEnv))
            {
                if (!int.TryParse(id, out int i))
                {
                    return Json(new { success = false, message = "Ej numeriskt id" });
                }
                var menu = db.Menus.Include(x=>x.Children).FirstOrDefault(x => x.Id == i);
                if (menu == null)
                {
                    return Json(new {success = false, message = "Meny finns ej"});
                }
                while (menu.Children.Count > 0)
                {
                    db.SubMenus.Remove(menu.Children.First());
                }

                db.Menus.Remove(menu);
                db.SaveChanges();
                return Json(new {success = true});
            }
        }

        [HttpPost]
        [Route("MoveLeft")]
        public ActionResult MoveLeft(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new {success = false, message = "Inget id inskickat"});
            }
            using (var db = new AKContext(_hostingEnv))
            {
                if (!int.TryParse(id, out int i))
                {
                    return Json(new { success = false, message = "Ej numeriskt id" });
                }
                var menu = db.Menus.FirstOrDefault(x => x.Id == i);
                if (menu == null)
                {
                    return Json(new { success = false, message = "Meny finns ej" });
                }
                var menu2 = db.Menus.Where(x => x.PosIndex < menu.PosIndex).OrderByDescending(x => x.PosIndex).FirstOrDefault();
                if (menu2!=null)
                {
                    var tempPos = menu.PosIndex;
                    menu.PosIndex = menu2.PosIndex;
                    menu2.PosIndex = tempPos;
                }

                db.SaveChanges();
                return Json(new {success = true});
            }
        }

        [HttpPost]
        [Route("MoveRight")]
        public ActionResult MoveRight(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new { success = false, message = "Inget id inskickat" });
            }
            using (var db = new AKContext(_hostingEnv))
            {
                if (!int.TryParse(id, out int i))
                {
                    return Json(new { success = false, message = "Ej numeriskt id" });
                }
                var menu = db.Menus.FirstOrDefault(x => x.Id == i);
                if (menu == null)
                {
                    return Json(new { success = false, message = "Meny finns ej" });
                }
                var menu2 = db.Menus.Where(x => x.PosIndex > menu.PosIndex).OrderBy(x => x.PosIndex).FirstOrDefault();
                if (menu2 != null)
                {
                    var tempPos = menu.PosIndex;
                    menu.PosIndex = menu2.PosIndex;
                    menu2.PosIndex = tempPos;
                }

                db.SaveChanges();
                return Json(new { success = true });
            }
        }

        [HttpPost]
        [Route("MoveUp")]
        public ActionResult MoveUp(string id,string parent)
        {
            if (string.IsNullOrWhiteSpace(id)||string.IsNullOrWhiteSpace(parent))
            {
                return Json(new { success = false, message = "Inget id eller topmeny inskickat" });
            }
            using (var db = new AKContext(_hostingEnv))
            {
                if (!int.TryParse(id, out int i))
                {
                    return Json(new { success = false, message = "Ej numeriskt id" });
                }
                var menu = db.SubMenus.FirstOrDefault(x => x.Id == i);
                if (menu == null)
                {
                    return Json(new { success = false, message = "Meny finns ej" });
                }
                var tId = 0;
                if (!int.TryParse(parent, out tId))
                {
                    return Json(new { success = false, message = "Ej numeriskt id" });
                }
                var topmenu = db.Menus.Include(x=>x.Children).FirstOrDefault(x => x.Id == tId);
                if (topmenu != null)
                {
                    var menu2 = topmenu.Children.Where(x => x.SubPosIndex < menu.SubPosIndex).OrderByDescending(x => x.SubPosIndex).FirstOrDefault();

                    if (menu2 != null)
                    {
                        var tempPos = menu.SubPosIndex;
                        menu.SubPosIndex = menu2.SubPosIndex;
                        menu2.SubPosIndex = tempPos;
                    }
                    
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Topmeny finns ej" });
                }

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
            using (var db = new AKContext(_hostingEnv))
            {
                if (!int.TryParse(id, out int i))
                {
                    return Json(new { success = false, message = "Ej numeriskt id" });
                }
                var menu = db.SubMenus.FirstOrDefault(x => x.Id == i);
                if (menu == null)
                {
                    return Json(new { success = false, message = "Meny finns ej" });
                }
                if (!int.TryParse(parent, out int tId))
                {
                    return Json(new { success = false, message = "Ej numeriskt id" });
                }
                var topmenu = db.Menus.Include(x => x.Children).FirstOrDefault(x => x.Id == tId);
                if (topmenu != null)
                {
                    var menu2 = topmenu.Children.Where(x => x.SubPosIndex > menu.SubPosIndex).OrderBy(x => x.SubPosIndex).FirstOrDefault();

                    if (menu2 != null)
                    {
                        var tempPos = menu.SubPosIndex;
                        menu.SubPosIndex = menu2.SubPosIndex;
                        menu2.SubPosIndex = tempPos;
                    }

                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Topmeny finns ej" });
                }

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
            using (var db = new AKContext(_hostingEnv))
            {
                if (!int.TryParse(id, out int i))
                {
                    return Json(new { success = false, message = "Ej numeriskt id" });
                }
                var menu = db.SubMenus.FirstOrDefault(x => x.Id == i);
                if (menu == null)
                {
                    return Json(new {success = false, message = "Meny finns ej"});
                }

                db.SubMenus.Remove(menu);
                db.SaveChanges();
                return Json(new {success = true});
            }
        }
    }
}