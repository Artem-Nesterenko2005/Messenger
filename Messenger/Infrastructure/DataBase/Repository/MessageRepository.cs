using Microsoft.EntityFrameworkCore;

namespace Messenger;

public interface IMessageRepository
{
    Task AddAsync(Message message);
    Task<List<Message>> GetRecentMessagesAsync(string userId, int count = 50);
}

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Message>> GetRecentMessagesAsync(string userId, int count = 50)
    {
        return await _context.Messages
            .Where(m => m.SenderId == userId)
            .OrderByDescending(m => m.Timestamp)
            .Take(count)
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }
}