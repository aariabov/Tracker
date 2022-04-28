using FluentValidation;
using Tracker.Users.RequestModels;

namespace Tracker.Users.Validators.FluentValidators;

public class UserBaseValidator : AbstractValidator<UserBaseRm>
{
    private readonly IUserRepository _userRepository;
    
    public UserBaseValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        const int nameMinLen = 3;
        const int nameMaxLen = 100;
        
        RuleFor(user => user.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("ФИО не может быть пустым")
            .Length(nameMinLen, nameMaxLen).WithMessage($"ФИО должно быть от {nameMinLen} до {nameMaxLen} символов");
        RuleFor(user => user.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email не может быть пустым")
            .EmailAddress().WithMessage("Email неправильного формата");
        RuleFor(user => user.BossId)
            .MustAsync(BossExistsAsync).WithMessage("Руководитель не найден")
            .When(u => !string.IsNullOrWhiteSpace(u.BossId));
        RuleFor(user => user.Roles)
            .CustomAsync(MustBeValidInstruction);
    }
    
    private async Task MustBeValidInstruction(IEnumerable<string> roles
        , ValidationContext<UserBaseRm> context, CancellationToken token)
    {
        foreach (var role in roles)
        {
            var isRoleExists = await _userRepository.IsRoleExistsAsync(role);
            if (!isRoleExists)
            {
                context.AddFailure($"Роль '{role}' не найдена");
                return;
            }
        }
    }
    
    private async Task<bool> BossExistsAsync(string? bossId, CancellationToken token)
    {
        return await _userRepository.IsUserExistsAsync(bossId!);
    }
}