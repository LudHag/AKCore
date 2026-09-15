using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AKCore.DataModel;

public class MobileNotificationDelivery
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [StringLength(95)]
    public string UserId { get; set; } = "";

    public int EventId { get; set; }

    [Required]
    [StringLength(128)]
    public string InstallationId { get; set; } = "";

    public DateTime ClaimedAt { get; set; }

    public DateTime? SentAt { get; set; }

    public AkUser User { get; set; } = null!;

    public Event Event { get; set; } = null!;
}
