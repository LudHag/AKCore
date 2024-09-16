using AKCore.DataModel;
using AKCore.Models;
using AKCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Components;

public class HeaderViewComponent : ViewComponent
{
    private readonly UserManager<AkUser> _userManager;
    private readonly AKContext _db;
    private readonly TranslationsService _translationsService;

    public HeaderViewComponent(UserManager<AkUser> userManager, AKContext db, TranslationsService translationsService)
    {
        _userManager = userManager;
        _db = db;
        _translationsService = translationsService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var loggedIn = User.Identity.IsAuthenticated;
        var balett = loggedIn && User.IsInRole(AkRoles.Balett);
        var gigsName = _translationsService.Get(TranslationDomains.Common, "Gigs");
        var upcomingName = _translationsService.Get(TranslationDomains.Common, "Upcoming");
        var menus = (await _db.Menus.OrderBy(x => x.PosIndex)
            .Include(b => b.Link)
            .Include(x => x.Children)
            .ThenInclude(x => x.Link)
            .ToListAsync())
            .Where(x => loggedIn || !x.LoggedIn)
            .Where(x => balett || !x.Balett)
            .Where(x => x.Link == null || loggedIn || !x.Link.LoggedIn)
            .Where(x => x.Link == null || !loggedIn || !x.Link.LoggedOut)
            .Where(x => x.Link == null || !x.Link.BalettOnly || (loggedIn && balett));
        var modelMenus = menus.Select(m => new ModelMenu(m, loggedIn)).ToList();
        var upcoming = new ModelMenu(loggedIn ? upcomingName : gigsName, "/upcoming", true) { Id = 10003 };
        modelMenus.Add(upcoming);
        var member = false;
        var numberUnreadRecruits = 0;
        var numberUnreadMail = 0;
        if (loggedIn)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var roles = await _userManager.GetRolesAsync(user);
            var nintendo = roles.Contains("SuperNintendo");
            var editor = roles.Contains("Editor");
            member = roles.Contains("Medlem");
            if (nintendo || editor)
            {
                var signUpMenus = new ModelMenu("Intresseanmälningar", "/Signup/Recruits", true) { Id = 10007 };
                modelMenus.Add(signUpMenus);

                var adminMenu = new ModelMenu("Admin", "", true) { Id = 10015 };
                if (nintendo) adminMenu.Children.Add(new ModelMenu("Ändra användare", "/User", true));
                adminMenu.Children.Add(new ModelMenu("Ändra filer", "/Media", true));
                if (nintendo)
                {
                    adminMenu.Children.Add(new ModelMenu("Ändra händelser", "/AdminEvent", true));
                    adminMenu.Children.Add(new ModelMenu("Ändra menyer", "/MenuEdit", true));
                }
                adminMenu.Children.Add(new ModelMenu("Ändra sidor", "/Edit", true) { Id = 10017 });
                adminMenu.Children.Add(new ModelMenu("Ändra skivor", "/AlbumEdit", true));

                if (nintendo)
                {
                    adminMenu.Children.Add(new ModelMenu("Adminlogg", "/Log", true));
                }

                modelMenus.Add(adminMenu);
                numberUnreadRecruits = _db.Recruits.Count(x => x.Archived == false);
            }
            if (nintendo)
            {
                numberUnreadMail = _db.MailBoxItems.Count(x => x.Archived == false);
            }
        }

        var model = new HeaderModel
        {
            Menus = modelMenus,
            LoggedIn = loggedIn,
            UserName = User.Identity.Name,
            CurrentUrl = Request.Path.ToString(),
            Member = member,
            Recruits = numberUnreadRecruits,
            Mails = numberUnreadMail
        };
        return View(model);
    }
}