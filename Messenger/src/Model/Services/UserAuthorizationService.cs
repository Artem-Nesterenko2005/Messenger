namespace Messenger.Services;

public interface IUserAuthorizationService
{
    public Task RegistrationNewUser(RegistrationUserDto dto);
    
    public Task<List<ValidationError>?> Authorization(AuthorizationUserDto dto);
}

public class UserAuthorizationService : IUserAuthorizationService
{
    private readonly IUserRepository _userRepository;

    private readonly IAuthorizationService _authorizationService;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAuthorizationService(
        IUserRepository repository,
        IAuthorizationService authorizationService,
        IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = repository;
        _authorizationService = authorizationService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task RegistrationNewUser(RegistrationUserDto dto)
    {
        await _userRepository.AddAsync(
            new User
            {
                Email = dto.Email,
                Password = dto.Password,
                Username = dto.Username,
                Id = Guid.NewGuid().ToString()
            });
    }

    public async Task<List<ValidationError>?> Authorization(AuthorizationUserDto dto)
    {
        var recordByName = await _userRepository.GetByUsernameAsync(dto.Username);
        if (recordByName != null && recordByName.Password == dto.Password)
        {
            await _authorizationService.AddCookies(recordByName.Id, recordByName.Username, "/Authorization", _httpContextAccessor.HttpContext!);
            return null;
        }
        return new List<ValidationError> { new ValidationError("Неверный логин или пароль") };
    }
}
