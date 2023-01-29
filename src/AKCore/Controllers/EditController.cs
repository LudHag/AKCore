using AKCore.DataModel;
using AKCore.Models;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AKCore.Controllers;

[Route("Edit")]
[Authorize(Roles = "SuperNintendo,Editor")]
public class EditController : Controller
{
    private readonly UserManager<AkUser> _userManager;
    private readonly PageService _pageService;

    public EditController(UserManager<AkUser> userManager, PageService pageService)
    {
        _userManager = userManager;
        _pageService = pageService;
    }

    [Authorize(Roles = "SuperNintendo,Editor")]
    public async Task<ActionResult> Index()
    {
        ViewBag.Title = "Redigera sidor";
        var model = new EditPagesModel
        {
            Pages = await _pageService.GetPages(),
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

        try
        {
            await _pageService.CreatePage(name, slug, loggedIn, User.Identity.Name);

            return Json(new { success = true });
        }
        catch (AkValidationError error)
        {
            return Json(new { success = false, message = error.Message });
        }

    }

    [Route("Page/{id:int}")]
    [Authorize(Roles = "SuperNintendo,Editor")]
    public async Task<ActionResult> Page(int id)
    {
        var page = await _pageService.GetPageEditModel(id);
        if (page == null)
        {
            return Redirect("/Edit");
        }

        ViewBag.Title = "Redigera " + page.Name;
        return View("EditPage");
    }


    [HttpGet("Page/{id:int}/model")]
    [Authorize(Roles = "SuperNintendo,Editor")]
    public async Task<ActionResult> PageEditModel(int id)
    {
        var model = await _pageService.GetPageEditModel(id);
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
        try
        {
            await _pageService.UpdatePage(model, id, User.Identity.Name);
            var newModel = await _pageService.GetPageEditModel(id);
            return Json(new { success = true, message = "Uppdaterade sidan framgångsrikt", newModel });
        }
        catch (AkValidationError error)
        {
            return Json(new { success = false, message = error.Message });
        }
    }

    [Route("RemovePage/{id:int}")]
    [Authorize(Roles = "SuperNintendo")]
    public async Task<ActionResult> RemovePage(string id)
    {
        if (!int.TryParse(id, out var pId))
        {
            return Redirect("/Edit");
        }
        await _pageService.RemovePage(pId, User.Identity.Name);

        return Redirect("/Edit");
    }
}