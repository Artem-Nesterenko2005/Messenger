using FluentValidation;
using Messenger;

public interface IUserValidationService
{
    Task<List<ValidationError>?> ValidateRegistrationAsync(RegistrationUserDto dto);
    Task<List<ValidationError>?> ValidateAuthorizationAsync(AuthorizationUserDto dto);
}

public class UserValidationService : IUserValidationService
{
    private readonly IUserRepository _userRepository;

    private readonly IValidator<RegistrationUserDto> _registrationValidator;

    private readonly IValidator<AuthorizationUserDto> _authorizationValidator;

    public UserValidationService(
        IUserRepository userRepository,
        IValidator<RegistrationUserDto> validator,
        IValidator<AuthorizationUserDto> authorizationValidator)
    {
        _userRepository = userRepository;
        _registrationValidator = validator;
        _authorizationValidator = authorizationValidator;
    }

    public async Task<List<ValidationError>?> ValidateAuthorizationAsync(AuthorizationUserDto dto)
    {
        var errorMessages = new List<ValidationError>();
        var validationResult = await _authorizationValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            errorMessages = validationResult.Errors
                .Select(e => new ValidationError(e.ErrorMessage))
                .ToList();
            return errorMessages;
        }
        return null;
    }

    public async Task<List<ValidationError>?> ValidateRegistrationAsync(RegistrationUserDto dto)
    {
        var errorMessages = new List<ValidationError>();
        var validationResult = await _registrationValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            errorMessages = validationResult.Errors
                .Select(e => new ValidationError(e.ErrorMessage))
                .ToList();
        }

        var uniqueErrorList = await ValidateUniqueUserRegistrationAsync(dto);
        if (errorMessages != null && uniqueErrorList != null)
            return errorMessages.Concat(uniqueErrorList).ToList();

        if (errorMessages != null && errorMessages.Count > 0)
            return errorMessages;

        if (uniqueErrorList != null)
            return uniqueErrorList;

        return null;
    }

    private async Task<List<ValidationError>?> ValidateUniqueUserRegistrationAsync(RegistrationUserDto dto)
    {
        var errors = new List<ValidationError>();

        var existingUser = await _userRepository.GetByUsernameAsync(dto.Username);
        if (existingUser != null)
            errors.Add( new ValidationError($"Имя пользователя '{dto.Username}' уже занято"));

        var existingEmail = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingEmail != null)
            errors.Add(new ValidationError($"Email '{dto.Email}' уже используется"));

        return errors.Count > 0 ? errors : null;
    }
}