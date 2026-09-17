namespace AKCore.Models;

public static class LoginRateLimit
{
    public const string PolicyName = "login";
    public const int PermitLimit = 30;
    public const int WindowMinutes = 5;
}
