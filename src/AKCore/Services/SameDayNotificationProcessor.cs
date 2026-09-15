using AKCore.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AKCore.Services;

public class SameDayNotificationProcessor
{
    private readonly SameDayNotificationRelevanceService _relevanceService;
    private readonly MobileNotificationService _notificationService;
    private readonly ILogger<SameDayNotificationProcessor> _logger;

    public SameDayNotificationProcessor(
        SameDayNotificationRelevanceService relevanceService,
        MobileNotificationService notificationService,
        ILogger<SameDayNotificationProcessor> logger)
    {
        _relevanceService = relevanceService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task RunOnceAsync(
        DateTime utcNow,
        CancellationToken cancellationToken = default)
    {
        var swedishNow = utcNow.ConvertToSwedishTime();

        if (swedishNow.TimeOfDay < TimeSpan.FromHours(8))
        {
            return;
        }

        var candidates = await _relevanceService.GetCandidatesAsync(
            swedishNow.Date,
            cancellationToken);

        foreach (var candidate in candidates)
        {
            try
            {
                await _notificationService.SendAsync(
                    candidate.UserId,
                    candidate.EventId,
                    utcNow,
                    cancellationToken);
            }
            catch (Exception error)
            {
                _logger.LogError(
                    error,
                    "Failed to send same-day notification for user {UserId} and event {EventId}.",
                    candidate.UserId,
                    candidate.EventId);
            }
        }
    }
}
