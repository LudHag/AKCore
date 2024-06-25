using System;
using System.Linq;
using System.Threading.Tasks;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AKCore.Components;

public class StructuredDataViewComponent : ViewComponent
{
    private readonly AKContext _db;

    public StructuredDataViewComponent(AKContext db) => _db = db;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var gigs = await _db.Events
                .Where(x => x.Type == "Spelning")
                .Where(x => !x.Secret)
                .Where(x => x.Day >= DateTime.UtcNow.Date)
                .ToListAsync();

        var orderdGigs = gigs .OrderBy(x => x.Day.Date).ThenBy(x => x.StartsTime != default ? x.StartsTime : x.HalanTime);
                

        var model = new StructuredDataModel()
        {
            UpcomingGigs = orderdGigs,
        };
        return View(model);
    }
}