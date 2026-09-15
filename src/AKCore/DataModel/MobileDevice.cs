using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AKCore.DataModel;

public class MobileDevice
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [StringLength(95)]
    public string UserId { get; set; } = "";

    [Required]
    [StringLength(128)]
    public string InstallationId { get; set; } = "";

    [Required]
    [StringLength(512)]
    public string PushToken { get; set; } = "";

    [Required]
    [StringLength(32)]
    public string Provider { get; set; } = "";

    [Required]
    [StringLength(32)]
    public string Platform { get; set; } = "";

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public AkUser User { get; set; } = null!;
}
