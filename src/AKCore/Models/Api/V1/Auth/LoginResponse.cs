namespace AKCore.Models.Api.V1.Auth;

public class LoginResponse
{
    public bool Authenticated { get; set; }

    public string AccessToken { get; set; } = "";

    public string RefreshToken { get; set; } = "";
}
