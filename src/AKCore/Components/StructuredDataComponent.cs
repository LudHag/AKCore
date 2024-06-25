using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AKCore.Components;

public class StructuredDataViewComponent : ViewComponent
{
    private readonly AKContext _db;
    private readonly IMemoryCache _memoryCache;

    public StructuredDataViewComponent(AKContext db, IMemoryCache memoryCache)
    {
        _db = db;
        _memoryCache = memoryCache;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var cacheKey = "structuredData";
        var structuredEvents = await _memoryCache.GetOrCreateAsync(cacheKey, cacheEntry =>
        {
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            return GetStructuredEvents();
        });

        var model = new StructuredDataModel()
        {
            UpcomingGigs = structuredEvents,
        };
        return View(model);
    }

    private async Task<IEnumerable<StructuredDataEvent>> GetStructuredEvents()
    {
        var gigs = await _db.Events
                .Where(x => x.Type == "Spelning" || x.Type == "Evenemang")
                .Where(x => !x.Secret)
                .Where(x => x.Day >= DateTime.UtcNow.Date)
                .ToListAsync();

        var orderdGigs = gigs.OrderBy(x => x.Day.Date).ThenBy(x => x.StartsTime != default ? x.StartsTime : x.HalanTime);
        var lastItem = orderdGigs.LastOrDefault();

        var structuredDataItems = orderdGigs.Select(x => new StructuredDataEvent(x.Name, x.Description, (x.Day + x.StartsTime).ToString("s", System.Globalization.CultureInfo.InvariantCulture), x.Place, x.Id == lastItem.Id));

        return structuredDataItems;
    }
}