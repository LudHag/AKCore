using System.Threading;
using System.Threading.Tasks;

namespace AKCore.Services;

public interface IFcmNotificationSender
{
    Task SendAsync(
        string pushToken,
        int eventId,
        string eventName,
        CancellationToken cancellationToken = default);
}
