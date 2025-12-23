namespace Messenger;

public record MessageDto
{
    public required string SenderName { get; set; }

    public required string RecipientId { get; set; }

    public required string Content { get; set; }
}
