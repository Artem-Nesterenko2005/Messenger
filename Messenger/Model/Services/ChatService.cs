using Messenger;

public interface IChatService
{
    Task<List<User>> GetUserInterlocutorsAsync(string userId);

    Task<List<Message>> GetChatHistoryAsync(string currentUserId, string otherUserId);

    Task SaveMessageAsync(string senderId, string recipientId, string senderName, string content);

    List<User> SearchUsers(string subName);
}

public class ChatService : IChatService
{
    private readonly IUserRepository _userRepository;

    private readonly IMessageRepository _messageRepository;

    public ChatService(IUserRepository userRepository, IMessageRepository messageRepository)
    {
        _userRepository = userRepository;
        _messageRepository = messageRepository;
    }

    public async Task<List<User>> GetUserInterlocutorsAsync(string userId)
    {
        var interlocutorsIds = await _userRepository.GetInterlocutorsAsync(userId);
        var interlocutors = new List<User>();

        foreach (var i in interlocutorsIds)
        {
            interlocutors.Add(await _userRepository.GetByIdAsync(i));
        }

        return interlocutors;
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

    public List<User> SearchUsers(string subName) => _userRepository.GetBySubName(subName);
}