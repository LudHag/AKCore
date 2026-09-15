using AKCore.DataModel;
using AKCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace AKCore.IntegrationTests.Api.V1;

public class SameDayNotificationProcessorTests
{
    [Fact]
    public async Task RunOnce_BeforeEightSwedishTime_DoesNotSend()
    {
        await using var factory = new CustomWebApplicationFactory();

        var userId = await factory.SeedMemberAndReturnIdAsync();

        // 2026-09-12 22:30 UTC is 2026-09-13 00:30 in Sweden.
        var utcNow = new DateTime(
            2026,
            9,
            12,
            22,
            30,
            0,
            DateTimeKind.Utc);

        var evt = new Event
        {
            Type = AkEventTypes.Evenemang,
            Name = "Swedish date boundary",
            Day = new DateTime(2026, 9, 13),
            SignUps =
            [
                new SignUp
                {
                    PersonId = userId,
                    Person = "member",
                    PersonName = "Member",
                    Where = AkSignupType.Halan
                }
            ]
        };

        var eventId = await factory.SeedEventAndReturnIdAsync(evt);

        await factory.SeedAsync(db =>
        {
            db.MobileDevices.Add(new MobileDevice
            {
                UserId = userId,
                InstallationId = "boundary-device",
                PushToken = "boundary-token",
                Provider = "fcm",
                Platform = "android",
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            });

            return Task.CompletedTask;
        });

        var sender = new FakeFcmNotificationSender();

        using var scope = factory.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<AkUser>>();

        var relevanceService =
            new SameDayNotificationRelevanceService(
                db,
                userManager);

        var deliveryService =
            new MobileNotificationDeliveryService(db);

        var notificationService =
            new MobileNotificationService(
                db,
                deliveryService,
                sender);

        var processor =
            new SameDayNotificationProcessor(
                relevanceService,
                notificationService,
                NullLogger<SameDayNotificationProcessor>.Instance);

        await processor.RunOnceAsync(utcNow);

        Assert.Empty(sender.Calls);
        Assert.Empty(await db.MobileNotificationDeliveries.ToListAsync());
    }

    [Fact]
    public async Task RunOnce_FailedCandidate_DoesNotStopLaterCandidates()
    {
        await using var factory = new CustomWebApplicationFactory();

        var firstUserId = await factory.SeedMemberAndReturnIdAsync(
            "notification-member-1");

        var secondUserId = await factory.SeedMemberAndReturnIdAsync(
            "notification-member-2");

        var utcNow = DateTime.UtcNow;

        var evt = new Event
        {
            Type = AkEventTypes.KarRep,
            Name = "Candidate isolation",
            Day = utcNow.Date,
            SignUps = []
        };

        await factory.SeedEventAndReturnIdAsync(evt);

        await factory.SeedAsync(db =>
        {
            db.MobileDevices.AddRange(
                new MobileDevice
                {
                    UserId = firstUserId,
                    InstallationId = "candidate-device-1",
                    PushToken = "candidate-token-1",
                    Provider = "fcm",
                    Platform = "android",
                    CreatedAt = utcNow,
                    UpdatedAt = utcNow
                },
                new MobileDevice
                {
                    UserId = secondUserId,
                    InstallationId = "candidate-device-2",
                    PushToken = "candidate-token-2",
                    Provider = "fcm",
                    Platform = "android",
                    CreatedAt = utcNow,
                    UpdatedAt = utcNow
                });

            return Task.CompletedTask;
        });

        var sender = new FakeFcmNotificationSender
        {
            FailFirstCall = true
        };

        using var scope = factory.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<AkUser>>();

        var relevanceService =
            new SameDayNotificationRelevanceService(
                db,
                userManager);

        var deliveryService =
            new MobileNotificationDeliveryService(db);

        var notificationService =
            new MobileNotificationService(
                db,
                deliveryService,
                sender);

        var processor =
            new SameDayNotificationProcessor(
                relevanceService,
                notificationService,
                NullLogger<SameDayNotificationProcessor>.Instance);

        await processor.RunOnceAsync(utcNow);

        Assert.Equal(2, sender.Calls.Count);

        var deliveries = await db.MobileNotificationDeliveries
            .ToListAsync();

        Assert.Equal(2, deliveries.Count);
        Assert.Single(deliveries, x => x.SentAt == null);
        Assert.Single(deliveries, x => x.SentAt != null);
    }

    [Fact]
    public async Task RunOnce_AfterEightSwedishTime_SendsTodaysNotification()
    {
        await using var factory = new CustomWebApplicationFactory();

        var userId = await factory.SeedMemberAndReturnIdAsync();

        var utcNow = new DateTime(
            2026,
            9,
            13,
            6,
            30,
            0,
            DateTimeKind.Utc);

        var evt = new Event
        {
            Type = AkEventTypes.Evenemang,
            Name = "Morning delivery window",
            Day = new DateTime(2026, 9, 13),
            SignUps =
            [
                new SignUp
                {
                    PersonId = userId,
                    Person = "member",
                    PersonName = "Member",
                    Where = AkSignupType.Halan
                }
            ]
        };

        var eventId = await factory.SeedEventAndReturnIdAsync(evt);

        await factory.SeedAsync(db =>
        {
            db.MobileDevices.Add(new MobileDevice
            {
                UserId = userId,
                InstallationId = "morning-device",
                PushToken = "morning-token",
                Provider = "fcm",
                Platform = "android",
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            });

            return Task.CompletedTask;
        });

        var sender = new FakeFcmNotificationSender();

        using var scope = factory.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AKContext>();
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<AkUser>>();

        var relevanceService =
            new SameDayNotificationRelevanceService(
                db,
                userManager);

        var deliveryService =
            new MobileNotificationDeliveryService(db);

        var notificationService =
            new MobileNotificationService(
                db,
                deliveryService,
                sender);

        var processor =
            new SameDayNotificationProcessor(
                relevanceService,
                notificationService,
                NullLogger<SameDayNotificationProcessor>.Instance);

        await processor.RunOnceAsync(utcNow);

        var call = Assert.Single(sender.Calls);

        Assert.Equal("morning-token", call.PushToken);
        Assert.Equal(eventId, call.EventId);

        var delivery = await db.MobileNotificationDeliveries
            .SingleAsync();

        Assert.NotNull(delivery.SentAt);
    }

    private sealed class FakeFcmNotificationSender : IFcmNotificationSender
    {
        public List<SendCall> Calls { get; } = [];

        public bool FailFirstCall { get; init; }

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

            if (FailFirstCall && Calls.Count == 1)
            {
                throw new InvalidOperationException(
                    "Simulated first-candidate failure.");
            }

            return Task.CompletedTask;
        }
    }

    private sealed record SendCall(
        string PushToken,
        int EventId,
        string EventName);
}
