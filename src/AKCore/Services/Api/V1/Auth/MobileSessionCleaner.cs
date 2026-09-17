using AKCore.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Services.Api.V1.Auth;

public class MobileSessionCleaner
{
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromHours(1);
    private static readonly TimeSpan RevokedRetention = TimeSpan.FromDays(1);

    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly Task intervalTask;

    public MobileSessionCleaner(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        intervalTask = SetupInterval();
    }

    public async Task ClearOldSessionsAsync()
    {
        var now = DateTime.UtcNow;
        var revokedCutoff = now - RevokedRetention;

        using var scope = serviceScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();

        var staleSessions = await db.MobileSessions
            .Where(x =>
                x.ExpiresAt <= now ||
                (x.RevokedAt != null &&
                x.RevokedAt <= revokedCutoff))
            .ToListAsync();

        if (staleSessions.Count == 0)
        {
            return;
        }

        db.MobileSessions.RemoveRange(staleSessions);
        await db.SaveChangesAsync();
    }

    private async Task SetupInterval()
    {
        while (true)
        {
            try
            {
                await ClearOldSessionsAsync();
            }
            catch
            {
            }
            await Task.Delay(CleanupInterval);
        }
    }
}
