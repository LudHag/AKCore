using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AKCore.Services;

public class FcmNotificationSender : IFcmNotificationSender
{
    private readonly FirebaseMessaging _messaging;
    private readonly MobilePushHealth _health;

    public FcmNotificationSender(
        FirebaseApp app,
        MobilePushHealth health)
    {
        _messaging = FirebaseMessaging.GetMessaging(app);
        _health = health;
    }

    public async Task SendAsync(
        string pushToken,
        int eventId,
        string eventName,
        CancellationToken cancellationToken = default)
    {
        var message = new Message
        {
            Notification = new Notification
            {
                Title = eventName,
                Body = "Du har ett evenemang idag."
            },
            Data = new Dictionary<string, string>
            {
                ["eventId"] = eventId.ToString()
            }
        };

#pragma warning disable CS0618
        message.Token = pushToken;
#pragma warning restore CS0618

        try
        {
            await _messaging.SendAsync(
                message,
                cancellationToken);

            _health.MarkHealthy(DateTime.UtcNow);
        }
        catch (FirebaseMessagingException error)
        {
            if (error.MessagingErrorCode ==
                MessagingErrorCode.Unregistered)
            {
                throw new InvalidMobilePushTokenException(
                    "The FCM registration token is no longer valid.",
                    error);
            }

            if (MobilePushFailureClassifier.IsInfrastructureFailure(
                error.MessagingErrorCode))
            {
                _health.MarkUnhealthy(
                    DateTime.UtcNow,
                    error);
            }

            throw;
        }
        catch (Exception error)
        {
            _health.MarkUnhealthy(
                DateTime.UtcNow,
                error);

            throw;
        }
    }
}
