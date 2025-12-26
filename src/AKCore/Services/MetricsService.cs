using AKCore.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Services;

public class MetricsService(AKContext db)
{
   
    public async Task SaveMetrics(IDictionary<string, int> metrics, bool loggedIn, int mobileRequests, int desktopRequests, DateTime time)
    {
        var metricsEntities = metrics
            .Select(metric => new RequestsData
            {
                Path = metric.Key,
                Amount = metric.Value,
                Mobile = mobileRequests,
                Desktop = desktopRequests,
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
