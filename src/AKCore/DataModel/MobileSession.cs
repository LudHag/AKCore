using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AKCore.DataModel;

public class MobileSession
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [StringLength(127)]
    public string UserId { get; set; } = "";

    [Required]
    [StringLength(128)]
    public string RefreshTokenHash { get; set; } = "";

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public AkUser User { get; set; } = null!;
}
