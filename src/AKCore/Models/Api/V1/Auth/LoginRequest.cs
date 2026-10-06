using System.ComponentModel.DataAnnotations;

namespace AKCore.Models.Api.V1.Auth;

public class LoginRequest
{
    [Required]
    public string Username { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}
