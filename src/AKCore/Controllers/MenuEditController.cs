using AKCore.DataModel;
using AKCore.Extensions;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Controllers
{
    [Route("MenuEdit")]
    [Authorize(Roles = "SuperNintendo")]
    public class MenuEditController : Controller
    {
        private readonly AKContext _db;
        private readonly UserManager<AkUser> _userManager;

        public MenuEditController(AKContext db, UserManager<AkUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Redigera Menyer";
            return View();
        }

        [Route("MenuListData")]
        public ActionResult MenuListData()
        {
            var pages = _db.Pages.OrderBy(x => x.Name).ToList();
            var menus = _db.Menus.Include(x => x.Children).OrderBy(x => x.PosIndex).ToList();
            var modelMenus = menus.Select(m => new ModelMenu(m, true)).ToList();
            var model = new MenuEditModel
            {
                Pages = pages,
                Menus = modelMenus
            };
            return Json(model);
        }

        [HttpPost]
        [Route("AddTopMenu")]
        public async Task<ActionResult> AddTopMenu(string name, string pageId, string loggedIn, string balett)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Json(new { success = false, message = "Menyn måste ha ett namn" });
            }

            var m = new Menu
            {
                Name = name,
                LoggedIn = loggedIn != null,
                Balett = balett != null
            };
            var highIndex = _db.Menus.OrderByDescending(z => z.PosIndex).FirstOrDefault();
            if (highIndex != null)
            {
                m.PosIndex = highIndex.PosIndex + 1;
            }
            if (!string.IsNullOrWhiteSpace(pageId))
            {
                if (int.TryParse(pageId, out var id))
                {
                    var page = _db.Pages.FirstOrDefault(x => x.Id == id);
                    if (page != null)
                    {
                        m.Link = page;
                    }
                }
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            _db.Log.Add(new LogItem()
            {
                Type = AkLogTypes.Menus,
                Modified = DateTime.Now.ConvertToSwedishTime(),
                ModifiedBy = user,
                Comment = "Toppmeny med namn " + m.Name + " skapas"
            });

            _db.Menus.Add(m);
            _db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        [Route("EditMenu")]
        public async Task<ActionResult> EditMenu(string parentId, string menuId, string text, string textEng, string pageId, string loggedIn, string balett)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Json(new { success = false, message = "Menyn måste ha ett namn" });
            }

            if (string.IsNullOrWhiteSpace(parentId))
            {
                if (!int.TryParse(menuId, out var id))
                {
                    return Json(new { success = false, message = "Felaktigt menyid" });
                }
                var menu = _db.Menus.Include(x => x.Link).FirstOrDefault(x => x.Id == id);
                if (menu == null)
                {
                    return Json(new { success = false, message = "Meny finns ej" });
                }
                menu.Name = text;
                menu.NameEng = textEng;
                if (int.TryParse(pageId, out var pId))
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
                    return Json(new { success = false, message = "Submeny finns ej" });
                }
                menu.Name = text;
                menu.NameEng = textEng;
                if (int.TryParse(pageId, out var pId))
                {
                    var page = _db.Pages.FirstOrDefault(x => x.Id == pId);
                    menu.Link = page;
                }
                else
                {
                    menu.Link = null;
                }
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            _db.Log.Add(new LogItem()
            {
                Type = AkLogTypes.Menus,
                Modified = DateTime.Now.ConvertToSwedishTime(),
                ModifiedBy = user,
                Comment = "Meny med id " + menuId + " redigeras"
            });

            var res = _db.SaveChanges();
            return res > 0 ? Json(new { success = true }) : Json(new { success = false, message = "Inga ändringar gjorda" });

        }

        [HttpPost]
        [Route("AddSubMenu")]
        public async Task<ActionResult> AddSubMenu(string parentId, string pageId, string text)
        {
            if (string.IsNullOrWhiteSpace(parentId) || string.IsNullOrWhiteSpace(pageId) || string.IsNullOrWhiteSpace(text))
            {
                return Json(new { success = false, message = "Otillräcklig input" });
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

                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                _db.Log.Add(new LogItem()
                {
                    Type = AkLogTypes.Menus,
                    Modified = DateTime.Now.ConvertToSwedishTime(),
                    ModifiedBy = user,
                    Comment = "Submeny med namn " + text + " läggs till"
                });

                _db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Felaktigt format på föräldrameny" });
        }

        [HttpPost]
        [Route("RemoveTopMenu")]
        public async Task<ActionResult> RemoveTopMenu(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new { success = false, message = "Inget id inskickat" });
            }
            if (!int.TryParse(id, out var i))
            {
                return Json(new { success = false, message = "Ej numeriskt id" });
            }
            var menu = _db.Menus.Include(x => x.Children).FirstOrDefault(x => x.Id == i);
            if (menu == null)
            {
                return Json(new { success = false, message = "Meny finns ej" });
            }
            while (menu.Children.Count > 0)
            {
                _db.SubMenus.Remove(menu.Children.First());
            }

            var menuName = menu.Name;
            _db.Menus.Remove(menu);

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            _db.Log.Add(new LogItem()
            {
                Type = AkLogTypes.Menus,
                Modified = DateTime.Now.ConvertToSwedishTime(),
                ModifiedBy = user,
                Comment = "Submeny med namn " + menuName + " tas bort"
            });
            _db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        [Route("MoveLeft")]
        public async Task<ActionResult> MoveLeft(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new { success = false, message = "Inget id inskickat" });
            }
            if (!int.TryParse(id, out var i))
            {
                return Json(new { success = false, message = "Ej numeriskt id" });
            }
            var menu = _db.Menus.FirstOrDefault(x => x.Id == i);
            if (menu == null)
            {
                return Json(new { success = false, message = "Meny finns ej" });
            }
            var menu2 = _db.Menus.Where(x => x.PosIndex < menu.PosIndex).OrderByDescending(x => x.PosIndex).FirstOrDefault();
            if (menu2 != null)
            {
                var tempPos = menu.PosIndex;
                menu.PosIndex = menu2.PosIndex;
                menu2.PosIndex = tempPos;
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            _db.Log.Add(new LogItem()
            {
                Type = AkLogTypes.Menus,
                Modified = DateTime.Now.ConvertToSwedishTime(),
                ModifiedBy = user,
                Comment = "Meny med id " + id + " flyttas"
            });

            _db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        [Route("MoveRight")]
        public async Task<ActionResult> MoveRight(string id)
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

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            _db.Log.Add(new LogItem()
            {
                Type = AkLogTypes.Menus,
                Modified = DateTime.Now.ConvertToSwedishTime(),
                ModifiedBy = user,
                Comment = "Meny med id " + id + " flyttas"
            });

            _db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        [Route("MoveUp")]
        public async Task<ActionResult> MoveUp(string id, string parent)
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
            if (!int.TryParse(parent, out var tId))
            {
                return Json(new { success = false, message = "Ej numeriskt id" });
            }
            var topmenu = _db.Menus.Include(x => x.Children).FirstOrDefault(x => x.Id == tId);
            if (topmenu == null) return Json(new { success = false, message = "Topmeny finns ej" });
            {
                var menu2 = topmenu.Children.Where(x => x.SubPosIndex < menu.SubPosIndex).OrderByDescending(x => x.SubPosIndex).FirstOrDefault();

                if (menu2 != null)
                {
                    var tempPos = menu.SubPosIndex;
                    menu.SubPosIndex = menu2.SubPosIndex;
                    menu2.SubPosIndex = tempPos;
                }

                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                _db.Log.Add(new LogItem()
                {
                    Type = AkLogTypes.Menus,
                    Modified = DateTime.Now.ConvertToSwedishTime(),
                    ModifiedBy = user,
                    Comment = "Meny med id " + id + " flyttas"
                });

                _db.SaveChanges();
                return Json(new { success = true });
            }

        }

        [HttpPost]
        [Route("MoveDown")]
        public async Task<ActionResult> MoveDown(string id, string parent)
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
            if (!int.TryParse(parent, out var tId))
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
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                _db.Log.Add(new LogItem()
                {
                    Type = AkLogTypes.Menus,
                    Modified = DateTime.Now.ConvertToSwedishTime(),
                    ModifiedBy = user,
                    Comment = "Meny med id " + id + " flyttas"
                });
                _db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Topmeny finns ej" });
        }

        [HttpPost]
        [Route("RemoveSubMenu")]
        public async Task<ActionResult> RemoveSubMenu(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new { success = false, message = "Inget id inskickat" });
            }
            if (!int.TryParse(id, out var i))
            {
                return Json(new { success = false, message = "Ej numeriskt id" });
            }
            var menu = _db.SubMenus.FirstOrDefault(x => x.Id == i);
            if (menu == null)
            {
                return Json(new { success = false, message = "Meny finns ej" });
            }

            var menuName = menu.Name;
            _db.SubMenus.Remove(menu);

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            _db.Log.Add(new LogItem()
            {
                Type = AkLogTypes.Menus,
                Modified = DateTime.Now.ConvertToSwedishTime(),
                ModifiedBy = user,
                Comment = "Submeny med namn " + menuName + " flyttas"
            });

            _db.SaveChanges();
            return Json(new { success = true });
        }
    }
}