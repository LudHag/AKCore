using AKCore.DataModel;
using AKCore.Extensions;
using AKCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Services;

public class PageService
{

    private readonly AKContext _db;
    private readonly UserManager<AkUser> _userManager;
    private readonly AdminLogService _adminLogService;

    public PageService(AKContext db, UserManager<AkUser> userManager, AdminLogService adminLogService)
    {
        _db = db;
        _userManager = userManager;
        _adminLogService = adminLogService;
    }

    public async Task<Page> GetPage(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return await _db.Pages.FirstOrDefaultAsync(x => x.Slug == "/");
        }
        return await _db.Pages.FirstOrDefaultAsync(x => x.Slug == ("/" + slug));
    }

    public async Task<PageEditModel> GetPageEditModel(int id)
    {
        var page = await _db.Pages.Include(x => x.Revisions).ThenInclude(x => x.ModifiedBy).FirstOrDefaultAsync(x => x.Id == id);
        if (page == null)
        {
            return null;
        }
        return new PageEditModel
        {
            Name = page.Name,
            Slug = page.Slug,
            PageId = page.Id,
            LastModified = page.LastModified,
            LoggedIn = page.LoggedIn,
            LoggedOut = page.LoggedOut,
            BalettOnly = page.BalettOnly,
            Albums = _db.Albums.ToList(),
            Widgets = page.WidgetsJson.GetWidgetsFromString(),
            MetaDescription = page.MetaDescription ?? "",
            Revisions = page.Revisions?.SkipLast(1).Map()
        };
    }

    public PageRenderModel GetRenderModel(Page page)
    {
        var model = new PageRenderModel()
        {
            Widgets = page.WidgetsJson != null ? JsonConvert.DeserializeObject<List<Widget>>(page.WidgetsJson) : new List<Widget>()
        };
        for (var i = 0; i < model.Widgets.Count; i++)
        {
            model.Widgets[i].Id = i;
        }

        return model;
    }

    public async Task<List<Page>> GetPages()
    {
        return await _db.Pages.OrderBy(x => x.Name).ToListAsync();
    }


    public async Task CreatePage(string name, string slug, string loggedIn, string userName)
    {
        if (_db.Pages.Any(x => x.Slug == slug))
        {
            throw new AkValidationError("Sidlänk måste vara unik");
        }
        var page = new Page
        {
            Name = name,
            Slug = slug,
            LoggedIn = loggedIn == "on",
            LastModified = DateTime.Now.ConvertToSwedishTime(),
        };
        await _db.Pages.AddAsync(page);
        await _db.SaveChangesAsync();

        var user = await _userManager.FindByNameAsync(userName);

        await _adminLogService.LogAction(AkLogTypes.Page, user, "Sida skapad med namn " + name);
    }

    public async Task RemovePage(int id, string userName)
    {
        var page = await _db.Pages.Include(x => x.Revisions).FirstOrDefaultAsync(x => x.Id == id);

        if (page?.Revisions != null)
        {
            foreach (var rev in page.Revisions)
            {
                _db.Revisions.Remove(rev);
            }
        }

        var topmenus = _db.Menus.Where(x => x.Link == page).ToList();
        foreach (var m in topmenus)
        {
            m.Link = null;
        }
        var subMenus = _db.SubMenus.Where(x => x.Link == page).ToList();
        foreach (var m in subMenus)
        {
            _db.SubMenus.Remove(m);
        }

        if (page == null)
        {
            return;
        }
        var pageName = page.Name;

        _db.Pages.Remove(page);
        await _db.SaveChangesAsync();
        var user = await _userManager.FindByNameAsync(userName);
        await _adminLogService.LogAction(AkLogTypes.Page, user, "Sida med namn " + pageName + " borttagen");
    }

    public async Task UpdatePage(PageEditModel model, int id, string userName)
    {

        var page = await _db.Pages.Include(x => x.Revisions).ThenInclude(x => x.ModifiedBy).FirstOrDefaultAsync(x => x.Id == id);
        if (page == null)
        {
            throw new AkValidationError("Could not find page with id " + id);
        }
        var user = await _userManager.FindByNameAsync(userName);

        if (page.Revisions == null)
        {
            page.Revisions = new List<Revision>();
        }
        else if (page.Revisions.Count > 5)
        {
            var oldestRevision = page.Revisions.OrderBy(x => x.Modified).FirstOrDefault();
            if (oldestRevision != null) _db.Revisions.Remove(oldestRevision);
        }

        page.Revisions.Add(new Revision()
        {
            BalettOnly = model.BalettOnly,
            LoggedIn = model.LoggedIn,
            LoggedOut = model.LoggedOut,
            Modified = DateTime.Now.ConvertToSwedishTime(),
            MetaDescription = model.MetaDescription,
            ModifiedBy = user,
            Name = model.Name,
            Slug = model.Slug,
            WidgetsJson = JsonConvert.SerializeObject(model.Widgets)
        });

        page.Name = model.Name;
        page.Slug = model.Slug;
        page.WidgetsJson = JsonConvert.SerializeObject(model.Widgets);
        page.MetaDescription = model.MetaDescription;
        page.LoggedIn = model.LoggedIn;
        page.LoggedOut = model.LoggedOut;
        page.BalettOnly = model.BalettOnly;
        page.LastModified = DateTime.Now.ConvertToSwedishTime();

        await _db.SaveChangesAsync();

        await _adminLogService.LogAction(AkLogTypes.Page, user, "Sida med namn " + model.Name + " uppdaterad");
    }
}
