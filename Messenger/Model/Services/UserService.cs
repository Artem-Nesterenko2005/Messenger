using FluentValidation;

namespace Messenger.Services;

public interface IUserService
{
    public Task<List<string>?> TryRegistrationNewUser(RegistrationUserDto dto);
    
    public Task<List<string>?> TryAuthorization(AuthorizationUserDto dto);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    private readonly IValidator<RegistrationUserDto> _registrationValidator;

    private readonly IValidator<AuthorizationUserDto> _authorizationValidator;

    private readonly IUserValidationService _validationService;

    public UserService(
        IUserRepository repository,
        IValidator<RegistrationUserDto> registrationValidator,
        IValidator<AuthorizationUserDto> authorizationValidator,
        IUserValidationService validationService)
    {
        _userRepository = repository;
        _registrationValidator = registrationValidator;
        _authorizationValidator = authorizationValidator;
        _validationService = validationService;
    }

    public async Task<List<string>?> TryRegistrationNewUser(RegistrationUserDto dto)
    {
        var validationResult = await _registrationValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors
                .Select(e => e.ErrorMessage)
                .ToList();
            return errorMessages;
        };

        var uniqueCheck = await _validationService.ValidateUniqueUserRegistrationAsync(dto);
        if (uniqueCheck != null)
            return uniqueCheck;

        await _userRepository.AddAsync(
            new User
            {
                Email = dto.Email,
                Password = dto.Password,
                Username = dto.Username,
                Id = Guid.NewGuid().ToString()
            });

        return null;
    }

    public async Task<List<string>?> TryAuthorization(AuthorizationUserDto dto)
    {
        var validationResult = await _authorizationValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors
                .Select(e => e.ErrorMessage)
                .ToList();
            return errorMessages;
        };

        var recordByName = await _userRepository.GetByUsernameAsync(dto.Username);
        if (recordByName != null && recordByName.Password == dto.Password)
        {
            return null;
        }
        return new List<string> { "Неверный логин или пароль" };
    }
}
