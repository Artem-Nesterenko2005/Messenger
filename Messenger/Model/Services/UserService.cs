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

    private readonly IAuthorizationService _authorizationService;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(
        IUserRepository repository,
        IValidator<RegistrationUserDto> registrationValidator,
        IValidator<AuthorizationUserDto> authorizationValidator,
        IUserValidationService validationService,
        IAuthorizationService authorizationService,
        IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = repository;
        _registrationValidator = registrationValidator;
        _authorizationValidator = authorizationValidator;
        _validationService = validationService;
        _authorizationService = authorizationService;
        _httpContextAccessor = httpContextAccessor;
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
            await _authorizationService.AddCookies(recordByName.Id, "/Authorization", _httpContextAccessor.HttpContext!);
            return null;
        }
        return new List<string> { "Неверный логин или пароль" };
    }
}
