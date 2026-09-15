using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AKCore.Services;

public class SameDayNotificationRunner
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<SameDayNotificationRunner> _logger;
    private readonly Task _intervalTask;

    public SameDayNotificationRunner(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<SameDayNotificationRunner> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _intervalTask = SetupIntervalAsync();
    }

    private async Task RunOnceAsync()
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var processor = scope.ServiceProvider
            .GetRequiredService<SameDayNotificationProcessor>();

        await processor.RunOnceAsync(DateTime.UtcNow);
    }

    private async Task SetupIntervalAsync()
    {
        while (true)
        {
            try
            {
                await RunOnceAsync();
            }
            catch (Exception error)
            {
                _logger.LogError(
                    error,
                    "Same-day notification processing failed.");
            }

            await Task.Delay(Interval);
        }
    }
}
