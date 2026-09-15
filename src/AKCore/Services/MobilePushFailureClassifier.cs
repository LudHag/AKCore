using FirebaseAdmin.Messaging;

namespace AKCore.Services;

public static class MobilePushFailureClassifier
{
    public static bool IsInfrastructureFailure(
        MessagingErrorCode? errorCode)
    {
        return errorCode switch
        {
            MessagingErrorCode.Unregistered => false,
            MessagingErrorCode.SenderIdMismatch => false,
            MessagingErrorCode.InvalidArgument => false,

            MessagingErrorCode.Internal => true,
            MessagingErrorCode.Unavailable => true,
            MessagingErrorCode.QuotaExceeded => true,
            MessagingErrorCode.ThirdPartyAuthError => true,

            _ => true
        };
    }
}