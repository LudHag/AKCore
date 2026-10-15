using System.ComponentModel.DataAnnotations;

namespace AKCore.Models.Api.V1.Profile;

public class ChangeUsernameRequest
{
    [Required]
    public string NewUsername { get; set; } = "";

    [Required]
    public string CurrentPassword { get; set; } = "";
}
