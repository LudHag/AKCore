using AKCore.DataModel;
using AKCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Services;

public class MetricsService(AKContext db)
{
   
    public async Task SaveMetrics(IDictionary<string, int> metrics, bool loggedIn)
    {
        var metricsEntities = metrics
            .Select(metric => new RequestsData
            {
                Path = metric.Key,
                Amount = metric.Value,
                LoggedIn = loggedIn,
                Created = DateTime.Now.ConvertToSwedishTime()
            }).ToList();

        if(metricsEntities.Count == 0)
        {
            return;
        }

        db.RequestsDatas.AddRange(metricsEntities);
        await db.SaveChangesAsync();
    }

}
