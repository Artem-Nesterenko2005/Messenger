using Messenger;
using Microsoft.EntityFrameworkCore;

public interface IChatService
{
    Task<List<User>> GetUserInterlocutorsAsync(string userId);

    Task<List<Message>> GetChatHistoryAsync(string currentUserId, string otherUserId);

    Task<Message> SaveMessageAsync(string senderId, string recipientId, string senderName, string content);

    Task<List<User>> SearchUsersAsync(string currentUserId, string searchTerm);
}

public class ChatService : IChatService
{
    private readonly AppDbContext _context;

    private readonly IClaimService _claimService;

    public ChatService(AppDbContext context, IClaimService claimService)
    {
        _context = context;
        _claimService = claimService;
    }

    public async Task<List<User>> GetUserInterlocutorsAsync(string userId)
    {
        var interlocutors = await _context.Messages
            .Where(m => m.SenderId == userId || m.RecipientId == userId)
            .Select(m => m.SenderId == userId ? m.RecipientId : m.SenderId)
            .Distinct()
            .ToListAsync();

        var chats = new List<ChatDto>();

        foreach (var interlocutorId in interlocutors)
        {
            var interlocutor = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == interlocutorId);

            if (interlocutor == null) continue;

            var lastMessages = await _context.Messages
                .Where(m => (m.SenderId == userId && m.RecipientId == interlocutorId) ||
                           (m.SenderId == interlocutorId && m.RecipientId == userId))
                .OrderByDescending(m => m.Timestamp)
                .Take(50)
                .ToListAsync();

            var chat = new ChatDto
            {
                Id = $"chat_{userId}_{interlocutorId}",
                Users = new List<User> { interlocutor },
                Messages = lastMessages
            };

            chats.Add(chat);
        }

        return chats.Select(e => e.Users.First(w => w.Username != _claimService.GetUserName())).ToList();
    }

    public async Task<List<Message>> GetChatHistoryAsync(string currentUserId, string otherUserId)
    {
        return await _context.Messages
            .Where(m => (m.SenderId == currentUserId && m.RecipientId == otherUserId) ||
                       (m.SenderId == otherUserId && m.RecipientId == currentUserId))
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }

    public async Task<Message> SaveMessageAsync(string senderId, string recipientId, string senderName, string content)
    {
        var message = new Message
        {
            Id = Guid.NewGuid().ToString(),
            SenderId = senderId,
            RecipientId = recipientId,
            Content = content,
            Timestamp = DateTime.UtcNow,
            SenderName = senderName,
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return message;
    }

    public async Task<List<User>> SearchUsersAsync(string currentUserId, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return new List<User>();
        }

        return await _context.Users
            .Where(u => u.Id != currentUserId &&
                       (u.Username.Contains(searchTerm) ||
                        u.Email.Contains(searchTerm)))
            .Take(10)
            .ToListAsync();
    }
}