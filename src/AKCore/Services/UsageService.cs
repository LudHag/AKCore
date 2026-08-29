using AKCore.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Services;

public class UsageService(AKContext db)
{
    public async Task SaveUsage(IDictionary<string, int> counts, DateTime time)
    {
        var usageEntities = counts
            .Where(count => count.Value > 0)
            .Select(count => new UsageData
            {
                Type = count.Key,
                Amount = count.Value,
                Created = time
            }).ToList();

        if (usageEntities.Count == 0)
        {
            return;
        }

        db.UsageDatas.AddRange(usageEntities);

        await db.SaveChangesAsync();
    }

    public async Task ClearOldMetrics()
    {
        var cutoffTime = DateTime.UtcNow.AddDays(-60);
        db.UsageDatas.RemoveRange(
            db.UsageDatas
            .Where(r => r.Created < cutoffTime)
        );
        await db.SaveChangesAsync();
    }
}
