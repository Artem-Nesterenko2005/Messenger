using Microsoft.EntityFrameworkCore;

namespace Messenger;

public interface IMessageRepository
{
    Task AddMessageAsync(Message message);
    Task<List<Message>> GetChatHistoryAsync(string currentUserId, string otherUserId);
    Task DeleteMessageByIdAsync(string messageId);
}

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddMessageAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Message>> GetChatHistoryAsync(string currentUserId, string otherUserId)
    {
        return await _context.Messages
            .Where(m => (m.SenderId == currentUserId && m.RecipientId == otherUserId) ||
                       (m.SenderId == otherUserId && m.RecipientId == currentUserId))
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }

    public async Task DeleteMessageByIdAsync(string messageId)
    {
        var message = await _context.Messages.FindAsync(messageId);
        if (message != null)
        {
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }
    }
}