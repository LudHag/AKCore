using AKCore.DataModel;
using AKCore.Models;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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

    [Route("SiteRequests")]
    public async Task<ActionResult> GetSiteRequests(bool loggedIn = true, bool loggedOut = true, StatisticsRequestRange range = StatisticsRequestRange.Day)
    {
        if (!loggedIn && !loggedOut)
        {
            return Json(new
            {
                items = new List<string>(),
                dates = new List<StatisticsRequestModel>()
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

    [Route("Gigs")]
    public async Task<ActionResult> GetGigs(StatisticsGigsRange range = StatisticsGigsRange.Month)
    {
        var today = DateTime.UtcNow;
        var startDate = range == StatisticsGigsRange.Month ? today.AddMonths(-1) : today.AddYears(-1);

        var dataItems = await db.Events
          .Where(x => x.Type == "Spelning")
          .Where(x => x.Day < today)
          .Where(x => x.Day > startDate)
          .OrderBy(x => x.Day)
          .Select(x => new
          {
              x.Id,
              x.Name,
              x.Day,
              CantCome = x.SignUps.Count(s => s.Where == AkSignupType.CantCome),
              CanCome = x.SignUps.Count(s => s.Where != AkSignupType.CantCome)
          })
          .ToListAsync();

        return Json(new
        {
            items = dataItems,
        });
    }

    private static string FormatTime(DateTime date, CultureInfo culture, StatisticsRequestRange range)
    {
        var perDay = range == StatisticsRequestRange.Month;

        return perDay ? date.ToString("dd MMM", culture) : date.ToString("ddd HH", culture);
    }

    private async Task<IEnumerable<StatisticsRequestModel>> GetRequestItems(bool loggedIn, bool loggedOut, StatisticsRequestRange range )
    {
        var all = loggedIn && loggedOut;

        var perDay = range == StatisticsRequestRange.Month;

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
                  Amount = g.Sum(r => r.Amount),
                  Mobile = g.Sum(r => r.Mobile),
                  Desktop = g.Sum(r => r.Desktop)
              })
              .Select(x => new StatisticsRequestModel(x.Created, x.Amount, x.Mobile, x.Desktop, x.Path))
              .ToListAsync();

        }
        return await dataItemsFiltered
                    .Select(x => new StatisticsRequestModel(x.Created, x.Amount, x.Mobile, x.Desktop, x.Path))
                    .ToListAsync();
    }

    private static IEnumerable<StatisticsRequestModel> NormalizeItems(IEnumerable<StatisticsRequestModel> items, IEnumerable<DateTime> dates)
    {
        var distinctItems = items.GroupBy(item => item.Created)
            .Select(group => new StatisticsRequestModel(group.Key, group.Sum(item => item.Amount), group.Sum(item => item.Mobile), group.Sum(item => item.Desktop), group.First().Path));

        return dates
            .Select(date =>
            {
                var item = distinctItems.FirstOrDefault(x => x.Created == date);
                return new StatisticsRequestModel(date, item == null ? 0 : item.Amount, item == null ? 0 : item.Mobile, item == null ? 0 : item.Desktop, null);
            });
    }

    private static DateTime GetRangeCompare(StatisticsRequestRange range)
    {
        return range switch
        {
            StatisticsRequestRange.Day => DateTime.UtcNow.AddDays(-1),
            StatisticsRequestRange.Week => DateTime.UtcNow.AddDays(-7),
            StatisticsRequestRange.Month => DateTime.UtcNow.AddDays(-30),
            _ => DateTime.UtcNow.AddDays(-1)
        };

    }
}

