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

    public PageService(AKContext db, UserManager<AkUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<Page> GetPage(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return await _db.Pages.FirstOrDefaultAsync(x => x.Slug == "/");
        }
        return await _db.Pages.FirstOrDefaultAsync(x => x.Slug == ("/" + slug));
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


    public async Task<string> CreatePage(string name, string slug, string loggedIn, string userName)
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

        var user = await _userManager.FindByNameAsync(userName);
        await _db.Log.AddAsync(new LogItem()
        {
            Type = AkLogTypes.Page,
            Modified = DateTime.Now.ConvertToSwedishTime(),
            ModifiedBy = user,
            Comment = "Sida skapad med namn " + name
        });

        await _db.SaveChangesAsync();
        var id = (await _db.Pages.FirstAsync(x => x.Slug == slug)).Id;

        return "/Edit/Page/" + id;
    }
}
