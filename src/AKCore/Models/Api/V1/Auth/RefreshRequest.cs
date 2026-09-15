using System.ComponentModel.DataAnnotations;

namespace AKCore.Models.Api.V1.Auth;

public class RefreshRequest
{
    [Required]
    public string RefreshToken { get; set; } = "";
}
