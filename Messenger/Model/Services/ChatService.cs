using Messenger;
using Microsoft.EntityFrameworkCore;

public interface IChatService
{
    // Получить все чаты пользователя (группировка сообщений по собеседникам)
    Task<List<ChatDto>> GetUserChatsAsync(string userId);

    // Получить историю сообщений с конкретным пользователем
    Task<List<Message>> GetChatHistoryAsync(string currentUserId, string otherUserId);

    // Отправить сообщение
    Task<Message> SaveMessageAsync(string senderId, string recipientId, string senderName, string content);

    // Поиск пользователей для нового чата
    Task<List<User>> SearchUsersAsync(string currentUserId, string searchTerm);
}

public class ChatService : IChatService
{
    private readonly AppDbContext _context;

    public ChatService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChatDto>> GetUserChatsAsync(string userId)
    {
        // Находим всех уникальных собеседников
        var interlocutors = await _context.Messages
            .Where(m => m.SenderId == userId || m.RecipientId == userId)
            .Select(m => m.SenderId == userId ? m.RecipientId : m.SenderId)
            .Distinct()
            .ToListAsync();

        var chats = new List<ChatDto>();

        foreach (var interlocutorId in interlocutors)
        {
            // Получаем информацию о собеседнике
            var interlocutor = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == interlocutorId);

            if (interlocutor == null) continue;

            // Получаем последние сообщения в этом чате (до 50)
            var lastMessages = await _context.Messages
                .Where(m => (m.SenderId == userId && m.RecipientId == interlocutorId) ||
                           (m.SenderId == interlocutorId && m.RecipientId == userId))
                .OrderByDescending(m => m.Timestamp)
                .Take(50)
                .ToListAsync();

            // Создаем DTO чата
            var chat = new ChatDto
            {
                Id = $"chat_{userId}_{interlocutorId}", // Уникальный ID чата
                Users = new List<User> { interlocutor }, // Собеседник
                Messages = lastMessages
            };

            chats.Add(chat);
        }

        return chats;
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