namespace AKCore.Models.Api.V1.Auth;

public class MobileAuthOptions
{
    public const string SectionName = "MobileAuth";

    public string Issuer { get; set; } = "AKCore";
    public string Audience { get; set; } = "AlteKamerer";
    public string SigningKey { get; set; } = "";
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 30;
}