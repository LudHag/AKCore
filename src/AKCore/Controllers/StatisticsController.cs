﻿using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Controllers;

[Route("Statistics")]
[Authorize(Roles = "SuperNintendo")]
public class StatisticsController(AKContext db) : Controller
{

    public ActionResult Index()
    {
        ViewBag.Title = "Statistics";
       
        return View();
    }

    [Route("Model")]
    public async Task<ActionResult> GetModel()
    {
        var dataItems = await db.RequestsDatas
            .Where(x => x.Created > DateTime.UtcNow.AddDays(-30))
            .OrderBy(x => x.Created)
            .ToListAsync();

        var dates = dataItems
             .Select(x => x.Created)
             .Distinct()              
             .ToList();

        var groupedItems = dataItems.GroupBy(x => x.Path)
            .Select(x => new { Path = x.Key, Items = NormalizeItems(x, dates) });

        return Json(new
        {
            items = groupedItems,
            dates = dates.Select(x=> x.ToString("HH"))
        });
    }

    private static IEnumerable<StatisticsItemModel> NormalizeItems(IEnumerable<RequestsData> items, IEnumerable<DateTime> dates)
    {
        var distinctItems = items.GroupBy(item => item.Created)
            .Select(group => new StatisticsItemModel(group.Key, group.Sum(item => item.Amount)));

        return dates
            .Select(date => {
            var item = distinctItems.FirstOrDefault(x => x.Created == date);
            return new StatisticsItemModel(date, item == null ? 0 : item.Amount);
        });
    }


}
