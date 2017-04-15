using System.Linq;
using System.Threading.Tasks;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Identity;
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
                    .Where(x => loggedIn || !x.LoggedIn)
                    .Where(x => x.Link==null || loggedIn || !x.Link.LoggedIn)
                    .Where(x => x.Link == null || !loggedIn || !x.Link.LoggedOut)
                    .Where(x => x.Link == null ||  !x.Link.BalettOnly || (loggedIn && User.IsInRole(AkRoles.Balett)));
                var modelMenus = menus.Select(m => new ModelMenu(m, loggedIn)).ToList();
                var upcomming = new ModelMenu(loggedIn ? "På gång" : "Spelningar", "/Upcomming", true) {Id = 10003};
                modelMenus.Add(upcomming);
                if (loggedIn)
                {
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    var roles = await _userManager.GetRolesAsync(user);
                    var nintendo = roles.Contains("SuperNintendo");
                    var editor = roles.Contains("Editor");
          
                    if (nintendo || editor)
                    {
                        var signUpMenus = new ModelMenu("Anmälningar", "", true) { Id = 10004};
                        signUpMenus.Children.Add(new ModelMenu("Intresseanmälningar", "/Signup/Recruits", true) { Id = 10007 });
                        modelMenus.Add(signUpMenus);

                        var adminMenu = new ModelMenu("Admin", "", true) {Id = 10015};
                        adminMenu.Children.Add(new ModelMenu("Ändra sidor", "/Edit", true) {Id = 10017});
                        modelMenus.Add(adminMenu);
                        if (nintendo)
                        {
                            adminMenu.Children.Add(new ModelMenu("Ändra menyer", "/MenuEdit", true));
                            adminMenu.Children.Add(new ModelMenu("Ändra händelser", "/AdminEvent", true));
                            adminMenu.Children.Add(new ModelMenu("Användare", "/User", true));
                        }
                        adminMenu.Children.Add(new ModelMenu("Ändra Skivor", "/AlbumEdit", true));
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