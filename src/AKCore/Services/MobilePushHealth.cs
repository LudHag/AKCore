using System;

namespace AKCore.Services;

public enum MobilePushHealthStatus
{
    Disabled,
    Ready,
    Healthy,
    Unhealthy
}

public class MobilePushHealth
{
    private readonly object _lock = new();

    public MobilePushHealth(bool enabled)
    {
        Status = enabled
            ? MobilePushHealthStatus.Ready
            : MobilePushHealthStatus.Disabled;
    }

    public MobilePushHealthStatus Status { get; private set; }

    public DateTime? LastSuccessfulSendAt { get; private set; }

    public DateTime? LastFailureAt { get; private set; }

    public string LastFailure { get; private set; } = "";

    public void MarkHealthy(DateTime utcNow)
    {
        lock (_lock)
        {
            Status = MobilePushHealthStatus.Healthy;
            LastSuccessfulSendAt = utcNow;
            LastFailure = "";
        }
    }

    public void MarkUnhealthy(
        DateTime utcNow,
        Exception error)
    {
        lock (_lock)
        {
            Status = MobilePushHealthStatus.Unhealthy;
            LastFailureAt = utcNow;
            LastFailure = error.Message;
        }
    }
}