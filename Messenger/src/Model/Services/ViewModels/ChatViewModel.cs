namespace Messenger;

public record ChatViewModel
{
    public required List<Message> Messages { get; set; }
    public required string MyId { get; set; }
    public required string InterlocutorId { get; set; }
    public required string MyName { get; set; }
}
