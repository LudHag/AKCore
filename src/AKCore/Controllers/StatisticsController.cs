using AKCore.DataModel;
using AKCore.Models;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Controllers;

[Route("Statistics")]
[Authorize(Roles = "SuperNintendo")]
public class StatisticsController(AKContext db, TranslationsService translationsService) : Controller
{
    public ActionResult Index()
    {
        ViewBag.Title = "Statistics";

        return View();
    }

    [Route("Model")]
    public async Task<ActionResult> GetModel(bool loggedIn = true, bool loggedOut = true, StatisticsRange range = StatisticsRange.Day)
    {
        if (!loggedIn && !loggedOut)
        {
            return Json(new
            {
                items = new List<string>(),
                dates = new List<StatisticsItemModel>()
            });
        }
        var dataItems = await GetRequestItems(loggedIn, loggedOut, range);


        var dates = dataItems
             .Select(x => x.Created)
             .Distinct()
             .ToList();

        var groupedItems = dataItems.GroupBy(x => x.Path)
            .Select(x => new { Path = x.Key, Items = NormalizeItems(x, dates) })
            .OrderByDescending(x => x.Items.Sum(y => y.Amount));

        CultureInfo culture = translationsService.IsEnglish() ? new("en-US") : new("sv-SE");

        return Json(new
        {
            items = groupedItems,
            dates = dates.Select(x => FormatTime(x, culture, range))
        });
    }

    private static string FormatTime(DateTime date, CultureInfo culture, StatisticsRange range)
    {
        var perDay = range == StatisticsRange.Month;

        return perDay ? date.ToString("dd MMM", culture) : date.ToString("ddd HH", culture);
    }

    private async Task<IEnumerable<StatisticsItemModel>> GetRequestItems(bool loggedIn , bool loggedOut , StatisticsRange range )
    {
        var all = loggedIn && loggedOut;

        var perDay = range == StatisticsRange.Month;

        var dataItemsFiltered = db.RequestsDatas
          .Where(x => x.Created > GetRangeCompare(range))
          .Where(x => all || x.LoggedIn == loggedIn);

        if (perDay)
        {
            return  await dataItemsFiltered.GroupBy(r => new
              {
                  Created = r.Created.Date,
                  r.Path
              })
              .Select(g => new
              {
                  g.Key.Created,
                  g.Key.Path,
                  Amount = g.Sum(r => r.Amount)
              })
              .Select(x => new StatisticsItemModel(x.Created, x.Amount, x.Path))
              .ToListAsync();

        }
        return await dataItemsFiltered
                    .Select(x => new StatisticsItemModel(x.Created, x.Amount, x.Path))
                    .ToListAsync();
    }

    private static IEnumerable<StatisticsItemModel> NormalizeItems(IEnumerable<StatisticsItemModel> items, IEnumerable<DateTime> dates)
    {
        var distinctItems = items.GroupBy(item => item.Created)
            .Select(group => new StatisticsItemModel(group.Key, group.Sum(item => item.Amount), group.First().Path));

        return dates
            .Select(date =>
            {
                var item = distinctItems.FirstOrDefault(x => x.Created == date);
                return new StatisticsItemModel(date, item == null ? 0 : item.Amount, null);
            });
    }

    private static DateTime GetRangeCompare(StatisticsRange range)
    {
        return range switch
        {
            StatisticsRange.Day => DateTime.UtcNow.AddDays(-1),
            StatisticsRange.Week => DateTime.UtcNow.AddDays(-7),
            StatisticsRange.Month => DateTime.UtcNow.AddDays(-30),
            _ => DateTime.UtcNow.AddDays(-1)
        };

    }
}

