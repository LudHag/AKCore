using System.ComponentModel.DataAnnotations;

namespace AKCore.Models.Api.V1.Devices;

public class DeviceRegistrationRequest
{
    [Required]
    [StringLength(512)]
    public string PushToken { get; set; } = "";

    [Required]
    [StringLength(32)]
    public string Platform { get; set; } = "";
}
