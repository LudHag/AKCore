using AKCore.DataModel;
using AKCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests.Api.V1;

public class MobileNotificationServiceTests
{
    [Fact]
    public async Task Send_SendsToAllFcmAndroidDevices()
    {
        await using var factory = new CustomWebApplicationFactory();

        var userId = await factory.SeedMemberAndReturnIdAsync();

        var evt = new Event
        {
            Type = AkEventTypes.Evenemang,
            Name = "Multiple device test",
            Day = DateTime.UtcNow.Date,
            SignUps = []
        };

        var eventId = await factory.SeedEventAndReturnIdAsync(evt);

        await factory.SeedAsync(db =>
        {
            db.MobileDevices.AddRange(
                new MobileDevice
                {
                    UserId = userId,
                    InstallationId = "phone",
                    PushToken = "phone-token",
                    Provider = "fcm",
                    Platform = "android",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new MobileDevice
                {
                    UserId = userId,
                    InstallationId = "tablet",
                    PushToken = "tablet-token",
                    Provider = "fcm",
                    Platform = "android",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow.AddMinutes(-1)
                });

            return Task.CompletedTask;
        });

        var sender = new FakeFcmNotificationSender();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var deliveryService =
            scope.ServiceProvider.GetRequiredService<MobileNotificationDeliveryService>();

        var service = new MobileNotificationService(
            db,
            deliveryService,
            sender);

        var sent = await service.SendAsync(
            userId,
            eventId,
            DateTime.UtcNow);

        Assert.True(sent);
        Assert.Equal(2, sender.Calls.Count);

        Assert.Contains(
            sender.Calls,
            x => x.PushToken == "phone-token");

        Assert.Contains(
            sender.Calls,
            x => x.PushToken == "tablet-token");

        var deliveries = await db.MobileNotificationDeliveries
            .ToListAsync();

        Assert.Equal(2, deliveries.Count);

        Assert.All(
            deliveries,
            delivery => Assert.NotNull(delivery.SentAt));
    }

    [Fact]
    public async Task Send_NoDevice_DoesNotClaimOrSend()
    {
        await using var factory = new CustomWebApplicationFactory();

        var userId = await factory.SeedMemberAndReturnIdAsync();

        var evt = new Event
        {
            Type = AkEventTypes.Evenemang,
            Name = "No device test",
            Day = DateTime.UtcNow.Date,
            SignUps = []
        };

        var eventId = await factory.SeedEventAndReturnIdAsync(evt);
        var sender = new FakeFcmNotificationSender();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var deliveryService =
            scope.ServiceProvider.GetRequiredService<MobileNotificationDeliveryService>();

        var service = new MobileNotificationService(
            db,
            deliveryService,
            sender);

        var sent = await service.SendAsync(
            userId,
            eventId,
            DateTime.UtcNow);

        Assert.False(sent);
        Assert.Empty(sender.Calls);
        Assert.Empty(await db.MobileNotificationDeliveries.ToListAsync());
    }

    [Fact]
    public async Task Send_ExistingClaim_DoesNotSendAgain()
    {
        await using var factory = new CustomWebApplicationFactory();

        var userId = await factory.SeedMemberAndReturnIdAsync();

        var evt = new Event
        {
            Type = AkEventTypes.Evenemang,
            Name = "Duplicate send test",
            Day = DateTime.UtcNow.Date,
            SignUps = []
        };

        var eventId = await factory.SeedEventAndReturnIdAsync(evt);

        await factory.SeedAsync(db =>
        {
            db.MobileDevices.Add(new MobileDevice
            {
                UserId = userId,
                InstallationId = "duplicate-device",
                PushToken = "duplicate-token",
                Provider = "fcm",
                Platform = "android",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            return Task.CompletedTask;
        });

        var sender = new FakeFcmNotificationSender();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var deliveryService =
            scope.ServiceProvider.GetRequiredService<MobileNotificationDeliveryService>();

        var service = new MobileNotificationService(
            db,
            deliveryService,
            sender);

        Assert.True(await service.SendAsync(
            userId,
            eventId,
            DateTime.UtcNow));

        Assert.False(await service.SendAsync(
            userId,
            eventId,
            DateTime.UtcNow.AddMinutes(1)));

        Assert.Single(sender.Calls);
        Assert.Single(await db.MobileNotificationDeliveries.ToListAsync());
    }

    [Fact]
    public async Task Send_Success_MarksDeliverySent()
    {
        await using var factory = new CustomWebApplicationFactory();

        var userId = await factory.SeedMemberAndReturnIdAsync();

        var evt = new Event
        {
            Type = AkEventTypes.Evenemang,
            Name = "Successful send test",
            Day = DateTime.UtcNow.Date,
            SignUps = []
        };

        var eventId = await factory.SeedEventAndReturnIdAsync(evt);

        await factory.SeedAsync(db =>
        {
            db.MobileDevices.Add(new MobileDevice
            {
                UserId = userId,
                InstallationId = "success-device",
                PushToken = "success-token",
                Provider = "fcm",
                Platform = "android",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            return Task.CompletedTask;
        });

        var sender = new FakeFcmNotificationSender();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var deliveryService =
            scope.ServiceProvider.GetRequiredService<MobileNotificationDeliveryService>();

        var service = new MobileNotificationService(
            db,
            deliveryService,
            sender);

        var claimedAt = DateTime.UtcNow;

        Assert.True(await service.SendAsync(
            userId,
            eventId,
            claimedAt));

        var delivery = await db.MobileNotificationDeliveries.SingleAsync();

        Assert.Equal(claimedAt, delivery.ClaimedAt);
        Assert.NotNull(delivery.SentAt);
    }

    [Fact]
    public async Task Send_OneDeviceFails_SuccessfulDeviceIsNotResentOnRetry()
    {
        await using var factory = new CustomWebApplicationFactory();

        var userId = await factory.SeedMemberAndReturnIdAsync();

        var evt = new Event
        {
            Type = AkEventTypes.Evenemang,
            Name = "Partial failure test",
            Day = DateTime.UtcNow.Date,
            SignUps = []
        };

        var eventId = await factory.SeedEventAndReturnIdAsync(evt);

        await factory.SeedAsync(db =>
        {
            db.MobileDevices.AddRange(
                new MobileDevice
                {
                    UserId = userId,
                    InstallationId = "phone",
                    PushToken = "phone-token",
                    Provider = "fcm",
                    Platform = "android",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new MobileDevice
                {
                    UserId = userId,
                    InstallationId = "tablet",
                    PushToken = "tablet-token",
                    Provider = "fcm",
                    Platform = "android",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

            return Task.CompletedTask;
        });

        var sender = new FakeFcmNotificationSender();
        sender.FailingTokens.Add("tablet-token");

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var deliveryService =
            scope.ServiceProvider.GetRequiredService<MobileNotificationDeliveryService>();

        var service = new MobileNotificationService(
            db,
            deliveryService,
            sender);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(
                userId,
                eventId,
                DateTime.UtcNow));

        var firstDeliveries = await db.MobileNotificationDeliveries
            .ToListAsync();

        var firstDelivery = Assert.Single(firstDeliveries);

        Assert.Equal("phone", firstDelivery.InstallationId);
        Assert.NotNull(firstDelivery.SentAt);

        sender.FailingTokens.Remove("tablet-token");

        var retried = await service.SendAsync(
            userId,
            eventId,
            DateTime.UtcNow.AddMinutes(1));

        Assert.True(retried);

        Assert.Equal(
            1,
            sender.Calls.Count(x => x.PushToken == "phone-token"));

        Assert.Equal(
            2,
            sender.Calls.Count(x => x.PushToken == "tablet-token"));

        var finalDeliveries = await db.MobileNotificationDeliveries
            .OrderBy(x => x.InstallationId)
            .ToListAsync();

        Assert.Equal(2, finalDeliveries.Count);

        Assert.All(
            finalDeliveries,
            delivery => Assert.NotNull(delivery.SentAt));
    }

    private sealed class FakeFcmNotificationSender : IFcmNotificationSender
    {
        public List<SendCall> Calls { get; } = [];

        public HashSet<string> FailingTokens { get; } = [];

        public Task SendAsync(
            string pushToken,
            int eventId,
            string eventName,
            CancellationToken cancellationToken = default)
        {
            Calls.Add(new SendCall(
                pushToken,
                eventId,
                eventName));

            if (FailingTokens.Contains(pushToken))
            {
                throw new InvalidOperationException(
                    "Simulated FCM failure.");
            }

            return Task.CompletedTask;
        }
    }

    private sealed record SendCall(
        string PushToken,
        int EventId,
        string EventName);
}
