using AKCore.DataModel;
using AKCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests.Api.V1;

public class MobileNotificationDeliveryServiceTests
{
    [Fact]
    public async Task TryClaim_FirstClaim_PersistsDelivery()
    {
        await using var factory = new CustomWebApplicationFactory();

        var userId = await factory.SeedMemberAndReturnIdAsync();

        var evt = new Event
        {
            Type = AkEventTypes.Evenemang,
            Name = "Notification delivery test",
            Day = DateTime.UtcNow.Date,
            SignUps = []
        };

        var eventId = await factory.SeedEventAndReturnIdAsync(evt);
        var claimedAt = DateTime.UtcNow;

        using (var scope = factory.Services.CreateScope())
        {
            var service = scope.ServiceProvider
                .GetRequiredService<MobileNotificationDeliveryService>();

            var claimed = await service.TryClaimAsync(
                userId,
                eventId,
                "test-installation",
                claimedAt);

            Assert.True(claimed);
        }

        using var verifyScope = factory.Services.CreateScope();
        var db = verifyScope.ServiceProvider
            .GetRequiredService<AKContext>();

        var delivery = await db.MobileNotificationDeliveries
            .SingleAsync();

        Assert.Equal(userId, delivery.UserId);
        Assert.Equal(eventId, delivery.EventId);
        Assert.Equal(claimedAt, delivery.ClaimedAt);
        Assert.Null(delivery.SentAt);
    }

    [Fact]
    public async Task TryClaim_ExistingUserEvent_ReturnsFalse()
    {
        await using var factory = new CustomWebApplicationFactory();

        var userId = await factory.SeedMemberAndReturnIdAsync();

        var evt = new Event
        {
            Type = AkEventTypes.Evenemang,
            Name = "Duplicate delivery test",
            Day = DateTime.UtcNow.Date,
            SignUps = []
        };

        var eventId = await factory.SeedEventAndReturnIdAsync(evt);

        using var scope = factory.Services.CreateScope();
        var service = scope.ServiceProvider
            .GetRequiredService<MobileNotificationDeliveryService>();

        var first = await service.TryClaimAsync(
            userId,
            eventId,
            "test-installation",
            DateTime.UtcNow);

        var second = await service.TryClaimAsync(
            userId,
            eventId,
            "test-installation",
            DateTime.UtcNow.AddMinutes(1));

        Assert.True(first);
        Assert.False(second);
    }

    [Fact]
    public async Task TryClaim_DifferentEvents_AllowsSeparateClaims()
    {
        await using var factory = new CustomWebApplicationFactory();

        var userId = await factory.SeedMemberAndReturnIdAsync();

        var firstEvent = new Event
        {
            Type = AkEventTypes.Evenemang,
            Name = "First event",
            Day = DateTime.UtcNow.Date,
            SignUps = []
        };

        var secondEvent = new Event
        {
            Type = AkEventTypes.Evenemang,
            Name = "Second event",
            Day = DateTime.UtcNow.Date,
            SignUps = []
        };

        await factory.SeedAsync(db =>
        {
            db.Events.AddRange(firstEvent, secondEvent);
            return Task.CompletedTask;
        });

        using var scope = factory.Services.CreateScope();
        var service = scope.ServiceProvider
            .GetRequiredService<MobileNotificationDeliveryService>();

        Assert.True(await service.TryClaimAsync(
            userId,
            firstEvent.Id,
            "test-installation",
            DateTime.UtcNow));

        Assert.True(await service.TryClaimAsync(
            userId,
            secondEvent.Id,
            "test-installation",
            DateTime.UtcNow));
    }
}
