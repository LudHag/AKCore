using AKCore.DataModel;
using AKCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Services;

public class MetricsService(AKContext db)
{
   
    public async Task SaveMetrics(IDictionary<string, RouteMetrics> metrics, bool loggedIn, DateTime time)
    {
        var metricsEntities = metrics
            .Select(metric => new RequestsData
            {
                Path = metric.Key,
                Amount = metric.Value.Desktop + metric.Value.Mobile,
                Mobile = metric.Value.Mobile,
                Desktop = metric.Value.Desktop,
                LoggedIn = loggedIn,
                Created = time
            }).ToList();

        if(metricsEntities.Count == 0)
        {
            return;
        }

        db.RequestsDatas.AddRange(metricsEntities);
        await db.SaveChangesAsync();
    }

}
