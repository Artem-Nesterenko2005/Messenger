using Messenger;

public interface IMessageService
{
    Task<List<Message>> GetChatHistoryAsync(string currentUserId, string otherUserId);

    Task SaveMessageAsync(string senderId, string recipientId, string senderName, string content);

    Task DeleteMessageByIdAsync(string messageId);
}

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;

    public MessageService(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<List<Message>> GetChatHistoryAsync(string currentUserId, string otherUserId)
        => await _messageRepository.GetChatHistoryAsync(currentUserId, otherUserId);

    public async Task SaveMessageAsync(string senderId, string recipientId, string senderName, string content)
    {
        var message = new Message
        {
            SenderId = senderId,
            RecipientId = recipientId,
            Content = content,
            SenderName = senderName,
        };

        await _messageRepository.AddMessageAsync(message);
    }

    public async Task DeleteMessageByIdAsync(string messageId)
    {
        await _messageRepository.DeleteMessageByIdAsync(messageId);
    }
}