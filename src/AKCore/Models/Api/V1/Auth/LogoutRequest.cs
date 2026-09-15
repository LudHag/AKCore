using System.ComponentModel.DataAnnotations;

namespace AKCore.Models.Api.V1.Auth;

public class LogoutRequest
{
    [Required]
    public string RefreshToken { get; set; } = "";
    public string InstallationId { get; set; }
}
