namespace AKCore.Models;

public record RouteMetrics(int Mobile, int Desktop)
{
    public static RouteMetrics operator +(RouteMetrics left, RouteMetrics right)
    {
        return new RouteMetrics(left.Mobile + right.Mobile, left.Desktop + right.Desktop);
    }
}

