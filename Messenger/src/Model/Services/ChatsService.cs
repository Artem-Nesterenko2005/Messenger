namespace Messenger;

public interface IChatsService
{
    Task<List<User>> GetUserInterlocutorsAsync(string userId);

    List<User> SearchUsers(string subName);
}

public class ChatsService : IChatsService
{
    private readonly IUserRepository _userRepository;

    public ChatsService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public List<User> SearchUsers(string subName) => _userRepository.GetBySubName(subName);

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
}
