using AKCore.Extensions;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AKCore.Controllers;

[Route("MenuEdit")]
[Authorize(Roles = "SuperNintendo")]
public class MenuEditController : Controller
{
    private readonly MenuService _menuService;

    public MenuEditController(MenuService menuService)
    {
        _menuService = menuService;
    }

    public ActionResult Index()
    {
        ViewBag.Title = "Redigera Menyer";
        return View();
    }

    [Route("MenuListData")]
    public ActionResult MenuListData() => Json(_menuService.GetMenuListData());

    [HttpPost]
    [Route("AddTopMenu")]
    public async Task<ActionResult> AddTopMenu(string name, string pageId, string loggedIn, string balett) =>
        this.ServiceJson(await _menuService.AddTopMenuAsync(name, pageId, loggedIn, balett, User.Identity.Name));

    [HttpPost]
    [Route("EditMenu")]
    public async Task<ActionResult> EditMenu(string parentId, string menuId, string text, string textEng, string pageId, string loggedIn, string balett) =>
        this.ServiceJson(await _menuService.EditMenuAsync(parentId, menuId, text, textEng, pageId, loggedIn, balett, User.Identity.Name));

    [HttpPost]
    [Route("AddSubMenu")]
    public async Task<ActionResult> AddSubMenu(string parentId, string pageId, string text) =>
        this.ServiceJson(await _menuService.AddSubMenuAsync(parentId, pageId, text, User.Identity.Name));

    [HttpPost]
    [Route("RemoveTopMenu")]
    public async Task<ActionResult> RemoveTopMenu(string id) =>
        this.ServiceJson(await _menuService.RemoveTopMenuAsync(id, User.Identity.Name));

    [HttpPost]
    [Route("MoveLeft")]
    public async Task<ActionResult> MoveLeft(string id) =>
        this.ServiceJson(await _menuService.MoveTopMenuAsync(id, moveLeft: true, User.Identity.Name));

    [HttpPost]
    [Route("MoveRight")]
    public async Task<ActionResult> MoveRight(string id) =>
        this.ServiceJson(await _menuService.MoveTopMenuAsync(id, moveLeft: false, User.Identity.Name));

    [HttpPost]
    [Route("MoveUp")]
    public async Task<ActionResult> MoveUp(string id, string parent) =>
        this.ServiceJson(await _menuService.MoveSubMenuAsync(id, parent, moveUp: true, User.Identity.Name));

    [HttpPost]
    [Route("MoveDown")]
    public async Task<ActionResult> MoveDown(string id, string parent) =>
        this.ServiceJson(await _menuService.MoveSubMenuAsync(id, parent, moveUp: false, User.Identity.Name));

    [HttpPost]
    [Route("RemoveSubMenu")]
    public async Task<ActionResult> RemoveSubMenu(string id) =>
        this.ServiceJson(await _menuService.RemoveSubMenuAsync(id, User.Identity.Name));
}
