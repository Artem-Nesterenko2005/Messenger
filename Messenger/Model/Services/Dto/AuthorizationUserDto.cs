using FluentValidation;

namespace Messenger;

public record AuthorizationUserDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class AuthorizationUserValidator : AbstractValidator<AuthorizationUserDto>
{
    public AuthorizationUserValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Имя пользователя обязательно")
            .MinimumLength(3).WithMessage("Имя пользователя: Минимум 3 символа")
            .MaximumLength(50).WithMessage("Имя пользователя: Максимум 50 символов")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Имя пользователя: Только латинские буквы, цифры и подчеркивание");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(6).WithMessage("Пароль: Минимум 6 символов");
    }
}