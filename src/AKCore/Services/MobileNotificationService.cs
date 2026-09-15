using AKCore.DataModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AKCore.Services;

public class MobileNotificationService
{
    private const string FcmProvider = "fcm";
    private const string AndroidPlatform = "android";

    private readonly AKContext _db;
    private readonly MobileNotificationDeliveryService _deliveryService;
    private readonly IFcmNotificationSender _sender;

    public MobileNotificationService(
        AKContext db,
        MobileNotificationDeliveryService deliveryService,
        IFcmNotificationSender sender)
    {
        _db = db;
        _deliveryService = deliveryService;
        _sender = sender;
    }

    public async Task<bool> SendAsync(
        string userId,
        int eventId,
        DateTime now,
        CancellationToken cancellationToken = default)
    {
        var devices = await _db.MobileDevices
            .Where(x =>
                x.UserId == userId &&
                x.Provider == FcmProvider &&
                x.Platform == AndroidPlatform)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        if (devices.Count == 0)
        {
            return false;
        }

        var evt = await _db.Events
            .SingleOrDefaultAsync(
                x => x.Id == eventId,
                cancellationToken);

        if (evt == null)
        {
            return false;
        }

        var sentAny = false;
        Exception firstFailure = null;

        foreach (var device in devices)
        {
            var claimed = await _deliveryService.TryClaimAsync(
                userId,
                eventId,
                device.InstallationId,
                now,
                cancellationToken);

            if (!claimed)
            {
                continue;
            }

            try
            {
                await _sender.SendAsync(
                    device.PushToken,
                    eventId,
                    evt.Name,
                    cancellationToken);

                await _deliveryService.MarkSentAsync(
                    userId,
                    eventId,
                    device.InstallationId,
                    DateTime.UtcNow,
                    cancellationToken);

                sentAny = true;
            }
            catch (Exception error)
            {
                await _deliveryService.ReleaseClaimAsync(
                    userId,
                    eventId,
                    device.InstallationId,
                    cancellationToken);

                firstFailure ??= error;
            }
        }

        if (firstFailure != null)
        {
            throw new InvalidOperationException(
                "One or more mobile notification deliveries failed.",
                firstFailure);
        }

        return sentAny;
    }
}