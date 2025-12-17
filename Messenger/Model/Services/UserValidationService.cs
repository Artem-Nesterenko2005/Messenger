using Messenger;

public interface IUserValidationService
{
    Task<List<string>?> ValidateUserRegistrationAsync(RegistrationUserDto dto);
}

public class UserValidationService : IUserValidationService
{
    private readonly IUserRepository _userRepository;

    public UserValidationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<string>?> ValidateUserRegistrationAsync(RegistrationUserDto dto)
    {
        var errors = new List<string>();

        var existingUser = await _userRepository.GetByUsernameAsync(dto.Username);
        if (existingUser != null)
            errors.Add($"Имя пользователя '{dto.Username}' уже занято");

        var existingEmail = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingEmail != null)
            errors.Add($"Email '{dto.Email}' уже используется");

        return errors.Count > 0 ? errors : null;
    }
}