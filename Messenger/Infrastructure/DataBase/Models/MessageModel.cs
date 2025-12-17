using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Messenger;

public record Message
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required string Id { get; set; }

    public required string SenderId { get; set; }

    [MaxLength(50)]
    public required string SenderUsername { get; set; }

    public required string Content { get; set; }

    public required DateTime Timestamp { get; set; } = DateTime.UtcNow;
}