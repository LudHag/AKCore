using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AKCore.Services;

public class FcmNotificationSender : IFcmNotificationSender
{
    private readonly FirebaseMessaging _messaging;

    public FcmNotificationSender(FirebaseApp app)
    {
        _messaging = FirebaseMessaging.GetMessaging(app);
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

        await _messaging.SendAsync(
            message,
            cancellationToken);
    }
}
