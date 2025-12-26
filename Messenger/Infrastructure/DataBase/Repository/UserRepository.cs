using Microsoft.EntityFrameworkCore;

namespace Messenger;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByUsernameAsync(string username);
    List<User> GetBySubName(string subName);
    Task<User?> GetByEmailAsync(string email);
    Task<List<string>> GetInterlocutorsAsync(string id);
    Task AddAsync(User user);
    Task DeleteAsync(string id);
}

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public List<User> GetBySubName(string subName)
    {
        return _context.Users.Where(e => e.Username.Contains(subName)).ToList();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<List<string>> GetInterlocutorsAsync(string id)
        => await _context.Messages
            .Where(m => m.SenderId == id || m.RecipientId == id)
            .Select(m => m.SenderId == id ? m.RecipientId : m.SenderId)
            .Distinct()
            .ToListAsync();

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}