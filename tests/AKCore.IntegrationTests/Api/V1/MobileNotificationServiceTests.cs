using AKCore.DataModel;
using AKCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests.Api.V1;

public class MobileNotificationServiceTests
{
    [Fact]
    public async Task Send_UsesMostRecentlyUpdatedFcmAndroidDevice()
    {
        await using var factory = new CustomWebApplicationFactory();

        var userId = await factory.SeedMemberAndReturnIdAsync();

        var evt = new Event
        {
            Type = AkEventTypes.Evenemang,
            Name = "Newest device test",
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
                    InstallationId = "old-device",
                    PushToken = "old-token",
                    Provider = "fcm",
                    Platform = "android",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new MobileDevice
                {
                    UserId = userId,
                    InstallationId = "new-device",
                    PushToken = "new-token",
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
        var call = Assert.Single(sender.Calls);
        Assert.Equal("new-token", call.PushToken);
        Assert.Equal(eventId, call.EventId);
        Assert.Equal(evt.Name, call.EventName);
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
    public async Task Send_SenderFailure_LeavesClaimUnsent()
    {
        await using var factory = new CustomWebApplicationFactory();

        var userId = await factory.SeedMemberAndReturnIdAsync();

        var evt = new Event
        {
            Type = AkEventTypes.Evenemang,
            Name = "Failed send test",
            Day = DateTime.UtcNow.Date,
            SignUps = []
        };

        var eventId = await factory.SeedEventAndReturnIdAsync(evt);

        await factory.SeedAsync(db =>
        {
            db.MobileDevices.Add(new MobileDevice
            {
                UserId = userId,
                InstallationId = "failure-device",
                PushToken = "failure-token",
                Provider = "fcm",
                Platform = "android",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            return Task.CompletedTask;
        });

        var sender = new FakeFcmNotificationSender
        {
            ThrowOnSend = true
        };

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

        var delivery = await db.MobileNotificationDeliveries.SingleAsync();

        Assert.Null(delivery.SentAt);
        Assert.Single(sender.Calls);
    }

    private sealed class FakeFcmNotificationSender : IFcmNotificationSender
    {
        public List<SendCall> Calls { get; } = [];

        public bool ThrowOnSend { get; init; }

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

            if (ThrowOnSend)
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
