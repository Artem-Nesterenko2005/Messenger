using FluentValidation;

namespace Messenger;

public record RegistrationUserDto
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class RegistrationUserValidator : AbstractValidator<RegistrationUserDto>
{
    public RegistrationUserValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Имя пользователя обязательно")
            .MinimumLength(3).WithMessage("Имя пользователя: Минимум 3 символа")
            .MaximumLength(50).WithMessage("Имя пользователя: Максимум 50 символов")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Имя пользователя: Только латинские буквы, цифры и подчеркивание");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Email: Некорректный email")
            .MaximumLength(100).WithMessage("Email: Максимум 100 символов");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(6).WithMessage("Пароль: Минимум 6 символов")
            .Matches(@"[A-Z]").WithMessage("Пароль: Должна быть хотя бы одна заглавная буква")
            .Matches(@"[a-z]").WithMessage("Пароль: Должна быть хотя бы одна строчная буква")
            .Matches(@"\d").WithMessage("Пароль: Должна быть хотя бы одна цифра");
    }
}