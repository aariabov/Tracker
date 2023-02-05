using FluentValidation;
using Tracker.Users.RequestModels;

namespace Tracker.Users.Validators.FluentValidators;

public class UserCreationValidator : AbstractValidator<UserRegistrationRm>
{
    private readonly IUserRepository _userRepository;

    public UserCreationValidator(UserBaseValidator userBaseValidator, IUserRepository userRepository)
    {
        _userRepository = userRepository;

        const int pwdMinLen = 1;
        const int pwdMaxLen = 100;

        Include(userBaseValidator);
        RuleFor(user => user.Email)
            .MustAsync(UniqueEmailAsync).WithMessage("Email уже существует");
        RuleFor(user => user.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Пароль не может быть пустым")
            .Length(pwdMinLen, pwdMaxLen).WithMessage($"Пароль должен быть от {pwdMinLen} до {pwdMaxLen} символов");
    }

    private async Task<bool> UniqueEmailAsync(string email, CancellationToken token)
    {
        var emailExists = await _userRepository.IsEmailExistsAsync(email);
        return !emailExists;
    }
}
