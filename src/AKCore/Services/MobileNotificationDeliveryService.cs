using AKCore.DataModel;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AKCore.Services;

public class MobileNotificationDeliveryService
{
    private readonly AKContext _db;

    public MobileNotificationDeliveryService(AKContext db)
    {
        _db = db;
    }

    public async Task<bool> TryClaimAsync(
        string userId,
        int eventId,
        string installationId,
        DateTime claimedAt,
        CancellationToken cancellationToken = default)
    {
        var alreadyClaimed = await _db.MobileNotificationDeliveries
            .AnyAsync(
                x => x.UserId == userId &&
                    x.EventId == eventId &&
                    x.InstallationId == installationId,
                cancellationToken);

        if (alreadyClaimed)
        {
            return false;
        }

        var delivery = new MobileNotificationDelivery
        {
            UserId = userId,
            EventId = eventId,
            InstallationId = installationId,
            ClaimedAt = claimedAt
        };

        _db.MobileNotificationDeliveries.Add(delivery);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException error) when (
            error.InnerException is MySqlException { Number: 1062 })
        {
            _db.Entry(delivery).State = EntityState.Detached;
            return false;
        }
    }

    public async Task MarkSentAsync(
        string userId,
        int eventId,
        string installationId,
        DateTime sentAt,
        CancellationToken cancellationToken = default)
    {
        var delivery = await _db.MobileNotificationDeliveries
            .SingleAsync(
                x => x.UserId == userId &&
                    x.EventId == eventId &&
                    x.InstallationId == installationId,
                cancellationToken);

        delivery.SentAt = sentAt;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ReleaseClaimAsync(
        string userId,
        int eventId,
        string installationId,
        CancellationToken cancellationToken = default)
    {
        var delivery = await _db.MobileNotificationDeliveries
            .SingleOrDefaultAsync(
                x => x.UserId == userId &&
                    x.EventId == eventId &&
                    x.InstallationId == installationId,
                cancellationToken);

        if (delivery == null || delivery.SentAt != null)
        {
            return;
        }

        _db.MobileNotificationDeliveries.Remove(delivery);

        await _db.SaveChangesAsync(cancellationToken);
    }
}