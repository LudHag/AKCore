using System.ComponentModel.DataAnnotations;

namespace AKCore.Models.Api.V1.Profile;

public class ChangePasswordRequest
{
    [Required]
    public string CurrentPassword { get; set; } = "";

    [Required]
    public string NewPassword { get; set; } = "";
}
