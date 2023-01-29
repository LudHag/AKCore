using AKCore.DataModel;
using AKCore.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AKCore.Services;

public class PageService
{

    private readonly AKContext _db;

    public PageService(AKContext db)
    {
        _db = db;
    }

    public Page GetPage(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return _db.Pages.FirstOrDefault(x => x.Slug == "/");
        }
        return _db.Pages.FirstOrDefault(x => x.Slug == ("/" + slug));
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
}
