using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Booking;

[Table("ProcessedMessages")]
internal sealed class ProcessedMessage
{
    [Key, Required]
    public Guid OrderId { get; set; }
    [Required]
    public Guid MessageId { get; set; }
}