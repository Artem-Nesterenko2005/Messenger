namespace Messenger;

public class ChatDto
{
    public required string Id { get; set; }

    public IEnumerable<Message> Messages { get; set; } = Enumerable.Empty<Message>();

    public required IEnumerable<User> Users { get; set; }
}