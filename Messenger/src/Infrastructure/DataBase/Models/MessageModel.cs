using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Messenger;

public record Message
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; } = Guid.NewGuid().ToString();

    public required string SenderId { get; set; }

    public required string SenderName { get; set; }

    public required string RecipientId { get; set; }

    public required string Content { get; set; }

    public DateTime Timestamp { get; } = DateTime.UtcNow;
}