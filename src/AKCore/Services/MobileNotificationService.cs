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
        var device = await _db.MobileDevices
            .Where(x =>
                x.UserId == userId &&
                x.Provider == FcmProvider &&
                x.Platform == AndroidPlatform)
            .OrderByDescending(x => x.UpdatedAt)
            .ThenByDescending(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (device == null)
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

        var claimed = await _deliveryService.TryClaimAsync(
            userId,
            eventId,
            now,
            cancellationToken);

        if (!claimed)
        {
            return false;
        }

        await _sender.SendAsync(
            device.PushToken,
            eventId,
            evt.Name,
            cancellationToken);

        await _deliveryService.MarkSentAsync(
            userId,
            eventId,
            DateTime.UtcNow,
            cancellationToken);

        return true;
    }
}
