using System.Linq;
using System.Threading.Tasks;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AKCore.Controllers
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly UserManager<AkUser> _userManager;

        public HeaderViewComponent(UserManager<AkUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var loggedIn = User.Identity.IsAuthenticated;

            using (var db = new AKContext())
            {
                var menus = db.Menus.OrderBy(x => x.PosIndex)
                    .Include(b => b.Link)
                    .Include(x => x.Children)
                    .ThenInclude(x => x.Link)
                    .ToList()
                    .Where(x=>loggedIn || !x.Link.LoggedIn);
                var modelMenus = menus.Select(m => new ModelMenu(m, loggedIn)).ToList();
                if (loggedIn)
                {
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    var roles = await _userManager.GetRolesAsync(user);
                    var nintendo = roles.Contains("SuperNintendo");
                    var editor = roles.Contains("Editor");
                    var memberMenu = new ModelMenu("Adressregister", "/MemberList", true) {Id = 10000 };
                    var postList = new ModelMenu("Kamerersposter", "/MemberList/PostList", true);
                    memberMenu.Children.Add(postList);
                    modelMenus.Add(memberMenu);
                    if (nintendo || editor)
                    {
                        var adminMenu = new ModelMenu("Ändra sidor", "/Edit", true) {Id = 10001 };
                        modelMenus.Add(adminMenu);
                        if (nintendo)
                        {
                            adminMenu.Children.Add(new ModelMenu("Lägg till spelningar", "/AdminEvent", true));
                            adminMenu.Children.Add(new ModelMenu("Ändra menyer", "/MenuEdit", true));
                            adminMenu.Children.Add(new ModelMenu("Användare", "/User", true));
                        }
                        adminMenu.Children.Add(new ModelMenu("Ladda upp filer", "/Media", true));
                    }
                }

                var model = new HeaderModel
                {
                    Menus = modelMenus,
                    LoggedIn = loggedIn,
                    UserName = User.Identity.Name
                };
                return View(model);
            }
        }
    }
}