using AKCore.DataModel;
using AKCore.Extensions;
using AKCore.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Services;

public class MenuService
{
    private readonly AKContext _db;
    private readonly AdminLogService _adminLogService;

    public MenuService(AKContext db, AdminLogService adminLogService)
    {
        _db = db;
        _adminLogService = adminLogService;
    }

    public MenuEditModel GetMenuListData()
    {
        var pages = _db.Pages.OrderBy(x => x.Name).ToList();
        var menus = _db.Menus.Include(x => x.Children).OrderBy(x => x.PosIndex).ToList();
        return new MenuEditModel
        {
            Pages = pages,
            Menus = menus.Select(m => new ModelMenu(m, true)).ToList()
        };
    }

    public async Task<ServiceResult> AddTopMenuAsync(string name, string pageId, string loggedIn, string balett, string userName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ServiceResult.Fail("Menyn måste ha ett namn");
        }

        var menu = new Menu
        {
            Name = name,
            LoggedIn = loggedIn != null,
            Balett = balett != null
        };

        var highIndex = _db.Menus.OrderByDescending(z => z.PosIndex).FirstOrDefault();
        if (highIndex != null)
        {
            menu.PosIndex = highIndex.PosIndex + 1;
        }

        AssignPageLink(menu, pageId);
        _db.Menus.Add(menu);
        await _adminLogService.LogAction(AkLogTypes.Menus, userName, "Toppmeny med namn " + menu.Name + " skapas");
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> EditMenuAsync(
        string parentId, string menuId, string text, string textEng, string pageId,
        string loggedIn, string balett, string userName)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return ServiceResult.Fail("Menyn måste ha ett namn");
        }

        if (string.IsNullOrWhiteSpace(parentId))
        {
            var parseResult = ParseMenuId(menuId, "Felaktigt menyid");
            if (!parseResult.Success)
            {
                return ServiceResult.Fail(parseResult.Message);
            }

            var menu = _db.Menus.Include(x => x.Link).FirstOrDefault(x => x.Id == parseResult.Value);
            if (menu == null)
            {
                return ServiceResult.Fail("Meny finns ej");
            }

            menu.Name = text;
            menu.NameEng = textEng;
            AssignPageLink(menu, pageId);
            menu.LoggedIn = loggedIn != null;
            menu.Balett = balett != null;
        }
        else
        {
            var parseResult = ParseMenuId(menuId, "Felaktigt menyid");
            if (!parseResult.Success)
            {
                return ServiceResult.Fail(parseResult.Message);
            }

            var subMenu = _db.SubMenus.FirstOrDefault(x => x.Id == parseResult.Value);
            if (subMenu == null)
            {
                return ServiceResult.Fail("Submeny finns ej");
            }

            subMenu.Name = text;
            subMenu.NameEng = textEng;
            AssignPageLink(subMenu, pageId);
        }

        await _adminLogService.LogAction(AkLogTypes.Menus, userName, "Meny med id " + menuId + " redigeras");
        var res = await _db.SaveChangesAsync();
        return res > 0 ? ServiceResult.Ok() : ServiceResult.Fail("Inga ändringar gjorda");
    }

    public async Task<ServiceResult> AddSubMenuAsync(string parentId, string pageId, string text, string userName)
    {
        if (string.IsNullOrWhiteSpace(parentId) || string.IsNullOrWhiteSpace(pageId) || string.IsNullOrWhiteSpace(text))
        {
            return ServiceResult.Fail("Otillräcklig input");
        }

        if (!int.TryParse(parentId, out var pid))
        {
            return ServiceResult.Fail("Felaktigt format på föräldrameny");
        }

        var parent = _db.Menus.Include(z => z.Children).FirstOrDefault(x => x.Id == pid);
        if (parent == null)
        {
            return ServiceResult.Fail("Föräldrameny finns ej");
        }

        var subMenu = new SubMenu { Name = text };
        var highIndex = parent.Children.OrderByDescending(z => z.SubPosIndex).FirstOrDefault();
        if (highIndex != null)
        {
            subMenu.SubPosIndex = highIndex.SubPosIndex + 1;
        }

        AssignPageLink(subMenu, pageId);
        parent.Children.Add(subMenu);
        await _adminLogService.LogAction(AkLogTypes.Menus, userName, "Submeny med namn " + text + " läggs till");
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> RemoveTopMenuAsync(string id, string userName)
    {
        var parseResult = ParseRequiredMenuId(id);
        if (!parseResult.Success)
        {
            return ServiceResult.Fail(parseResult.Message);
        }

        var menu = _db.Menus.Include(x => x.Children).FirstOrDefault(x => x.Id == parseResult.Value);
        if (menu == null)
        {
            return ServiceResult.Fail("Meny finns ej");
        }

        while (menu.Children.Count > 0)
        {
            _db.SubMenus.Remove(menu.Children.First());
        }

        var menuName = menu.Name;
        _db.Menus.Remove(menu);
        await _adminLogService.LogAction(AkLogTypes.Menus, userName, "Submeny med namn " + menuName + " tas bort");
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> MoveTopMenuAsync(string id, bool moveLeft, string userName)
    {
        var parseResult = ParseRequiredMenuId(id);
        if (!parseResult.Success)
        {
            return ServiceResult.Fail(parseResult.Message);
        }

        var menu = _db.Menus.FirstOrDefault(x => x.Id == parseResult.Value);
        if (menu == null)
        {
            return ServiceResult.Fail("Meny finns ej");
        }

        var menu2 = moveLeft
            ? _db.Menus.Where(x => x.PosIndex < menu.PosIndex).OrderByDescending(x => x.PosIndex).FirstOrDefault()
            : _db.Menus.Where(x => x.PosIndex > menu.PosIndex).OrderBy(x => x.PosIndex).FirstOrDefault();

        if (menu2 != null)
        {
            var tempPos = menu.PosIndex;
            menu.PosIndex = menu2.PosIndex;
            menu2.PosIndex = tempPos;
        }

        await _adminLogService.LogAction(AkLogTypes.Menus, userName, "Meny med id " + id + " flyttas");
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> MoveSubMenuAsync(string id, string parentId, bool moveUp, string userName)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(parentId))
        {
            return ServiceResult.Fail("Inget id eller topmeny inskickat");
        }

        var parseResult = ParseRequiredMenuId(id);
        if (!parseResult.Success)
        {
            return ServiceResult.Fail(parseResult.Message);
        }

        if (!int.TryParse(parentId, out var topMenuId))
        {
            return ServiceResult.Fail("Ej numeriskt id");
        }

        var menu = _db.SubMenus.FirstOrDefault(x => x.Id == parseResult.Value);
        if (menu == null)
        {
            return ServiceResult.Fail("Meny finns ej");
        }

        var topMenu = _db.Menus.Include(x => x.Children).FirstOrDefault(x => x.Id == topMenuId);
        if (topMenu == null)
        {
            return ServiceResult.Fail("Topmeny finns ej");
        }

        var menu2 = moveUp
            ? topMenu.Children.Where(x => x.SubPosIndex < menu.SubPosIndex).OrderByDescending(x => x.SubPosIndex).FirstOrDefault()
            : topMenu.Children.Where(x => x.SubPosIndex > menu.SubPosIndex).OrderBy(x => x.SubPosIndex).FirstOrDefault();

        if (menu2 != null)
        {
            var tempPos = menu.SubPosIndex;
            menu.SubPosIndex = menu2.SubPosIndex;
            menu2.SubPosIndex = tempPos;
        }

        await _adminLogService.LogAction(AkLogTypes.Menus, userName, "Meny med id " + id + " flyttas");
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> RemoveSubMenuAsync(string id, string userName)
    {
        var parseResult = ParseRequiredMenuId(id);
        if (!parseResult.Success)
        {
            return ServiceResult.Fail(parseResult.Message);
        }

        var menu = _db.SubMenus.FirstOrDefault(x => x.Id == parseResult.Value);
        if (menu == null)
        {
            return ServiceResult.Fail("Meny finns ej");
        }

        var menuName = menu.Name;
        _db.SubMenus.Remove(menu);
        await _adminLogService.LogAction(AkLogTypes.Menus, userName, "Submeny med namn " + menuName + " flyttas");
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    private static ServiceResult<int> ParseRequiredMenuId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return ServiceResult<int>.Fail("Inget id inskickat");
        }

        if (!int.TryParse(id, out var parsedId))
        {
            return ServiceResult<int>.Fail("Ej numeriskt id");
        }

        return ServiceResult<int>.Ok(parsedId);
    }

    private static ServiceResult<int> ParseMenuId(string menuId, string invalidMessage)
    {
        if (!int.TryParse(menuId, out var id))
        {
            return ServiceResult<int>.Fail(invalidMessage);
        }

        return ServiceResult<int>.Ok(id);
    }

    private void AssignPageLink(Menu menu, string pageId)
    {
        if (int.TryParse(pageId, out var pId))
        {
            menu.Link = _db.Pages.FirstOrDefault(x => x.Id == pId);
        }
        else
        {
            menu.Link = null;
        }
    }

    private void AssignPageLink(SubMenu menu, string pageId)
    {
        if (int.TryParse(pageId, out var pId))
        {
            menu.Link = _db.Pages.FirstOrDefault(x => x.Id == pId);
        }
        else
        {
            menu.Link = null;
        }
    }
}
