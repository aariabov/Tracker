using FluentValidation;
using Tracker.Users.RequestModels;

namespace Tracker.Users.Validators.FluentValidators;

public class UserUpdatingValidator : AbstractValidator<UserUpdatingRm>
{
    private readonly IUserRepository _userRepository;

    public UserUpdatingValidator(UserBaseValidator userBaseValidator, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        
        Include(userBaseValidator);
        RuleFor(user => user.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Идентификатор пользователя не может быть пустым")
            .MustAsync(UserExistsAsync).WithMessage("Пользователь не найден");
        RuleFor(user => user.Email)
            .MustAsync(UniqueEmailAsync).WithMessage("Email уже существует");
    }
    
    private async Task<bool> UniqueEmailAsync(UserUpdatingRm userRm, string email, CancellationToken token)
    {
        var user = await _userRepository.GetUserByEmail(email);
        return user is null || user.Id == userRm.Id;
    }
    
    private async Task<bool> UserExistsAsync(string userId, CancellationToken token)
    {
        return await _userRepository.IsUserExistsAsync(userId);
    }
}