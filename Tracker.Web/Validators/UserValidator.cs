using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tracker.Web.Domain;
using Tracker.Web.RequestModels;

namespace Tracker.Web.Validators;

public class UserValidator : AbstractValidator<UserRegistrationRm>
{
    private readonly UserManager<User> _userManager;
    
    public UserValidator(UserManager<User> userManager)
    {
        _userManager = userManager;

        const int nameMinLen = 3;
        const int nameMaxLen = 100;
        const int pwdMinLen = 1;
        const int pwdMaxLen = 100;
        
        RuleFor(user => user.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("ФИО не может быть пустым")
            .Length(nameMinLen, nameMaxLen).WithMessage($"ФИО должно быть от {nameMinLen} до {nameMaxLen} символов");
        RuleFor(user => user.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email не может быть пустым")
            .EmailAddress().WithMessage("Email неправильного формата")
            .MustAsync(UniqueEmailAsync).WithMessage("Email уже существует");
        RuleFor(user => user.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Пароль не может быть пустым")
            .Length(pwdMinLen, pwdMaxLen).WithMessage($"Пароль должен быть от {pwdMinLen} до {pwdMaxLen} символов");
        RuleFor(user => user.BossId)
            .MustAsync(BossExistsAsync).WithMessage("Руководитель не найден")
            .When(u => !string.IsNullOrWhiteSpace(u.BossId));
    }
    
    private async Task<bool> UniqueEmailAsync(string email, CancellationToken token)
    {
        var emailExists = await _userManager.Users.AnyAsync(u => u.Email == email, token);
        return !emailExists;
    }
    
    private async Task<bool> BossExistsAsync(string? bossId, CancellationToken token)
    {
        return await _userManager.Users.AnyAsync(u => u.Id == bossId, token);
    }
}