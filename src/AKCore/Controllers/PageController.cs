using AKCore.DataModel;
using AKCore.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AKCore.Controllers;

public class PageController : Controller
{
    private readonly PageService _pageService;

    public PageController(PageService pageService)
    {
        _pageService = pageService;
    }

    public async Task<ActionResult> Page(string slug)
    {
        var loggedIn = User.Identity.IsAuthenticated;
        var redirectLink = "/";
        if (loggedIn) redirectLink = "/upcoming";

        var page = await _pageService.GetPage(slug);

        if (slug == "teapot")
        {
            Response.StatusCode = 418;
            return View("Teapot");
        }
        else if (page == null)
        {
            Response.StatusCode = 404;
            return View("Error");
        }
        if (page.LoggedIn && !loggedIn)
        {
            return Redirect(redirectLink);
        }
        if (page.LoggedOut && loggedIn)
        {
            return Redirect(redirectLink);
        }
        if (page.BalettOnly)
        {
            if (!(User.IsInRole(AkRoles.Balett) || User.IsInRole(AkRoles.SuperNintendo)))
            {
                return Redirect(redirectLink);
            }
        }
        ViewData["Title"] = page.Name;
        if (!string.IsNullOrWhiteSpace(page.MetaDescription))
        {
            ViewData["Description"] = page.MetaDescription;
        }
        ViewData["Canonical"] = "https://www.altekamereren.org" + (string.IsNullOrWhiteSpace(slug) ? "" : "/") + slug;

        var model = _pageService.GetRenderModel(page);

        return View("Index", model);
    }
}