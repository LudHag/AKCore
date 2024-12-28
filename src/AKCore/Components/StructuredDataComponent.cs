using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AKCore.Components;

public class StructuredDataViewComponent : ViewComponent
{
    private readonly AKContext _db;
    private readonly IMemoryCache _memoryCache;
    private static readonly ImmutableList<string> _akImages =
       ["/images/eventimages/ak1.jpg", "/images/eventimages/ak2.jpg", "/images/eventimages/ak3.jpg", "/images/eventimages/ak4.jpg"];
    private static readonly Random _random = new();

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

    private string GetRandomAkImage(int itemId)
    {
        int index = Math.Abs(itemId.GetHashCode()) % _akImages.Count;
        return _akImages[index];
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

        var structuredDataItems = orderdGigs
            .Select(x => 
            new StructuredDataEvent(
                x.Name, 
                x.Description, 
                (x.Day + x.StartsTime).ToString("s", System.Globalization.CultureInfo.InvariantCulture),
                (x.Day + x.StartsTime + TimeSpan.FromHours(2)).ToString("s", System.Globalization.CultureInfo.InvariantCulture),
                x.Place,
                GetRandomAkImage(x.Id),
                x.Id == lastItem.Id));

        return structuredDataItems;
    }
}