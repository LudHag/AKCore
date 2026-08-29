using AKCore.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AKCore.Services;

public class UsageCollector
{
    private readonly object lockObject = new();
    private Dictionary<string, int> usageCounts = [];
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly Task intervalTask;

    public UsageCollector(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        intervalTask = SetupInterval();
    }

    public void Record(string type)
    {
        lock (lockObject)
        {
            usageCounts[type] = usageCounts.TryGetValue(type, out var count) ? count + 1 : 1;
        }
    }

    public async Task FlushAsync()
    {
        Dictionary<string, int> countsToSave;
        lock (lockObject)
        {
            countsToSave = usageCounts;
            usageCounts = [];
        }

        if (countsToSave.Count == 0)
        {
            return;
        }

        using var scope = serviceScopeFactory.CreateScope();
        var usageService = scope.ServiceProvider.GetRequiredService<UsageService>();
        var nowTime = DateTime.Now.ConvertToSwedishTime();
        await usageService.SaveUsage(countsToSave, nowTime);
    }

    private async Task SetupInterval()
    {
        var intervalPeriod = TimeSpan.FromHours(1);

        while (true)
        {
            try
            {
                await FlushAsync();
            }
            catch
            {
            }
            await Task.Delay(intervalPeriod);
        }
    }
}
