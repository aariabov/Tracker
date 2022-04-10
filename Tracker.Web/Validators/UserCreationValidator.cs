using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tracker.Db.Models;
using Tracker.Web.Domain;
using Tracker.Web.RequestModels;

namespace Tracker.Web.Validators;

public class UserCreationValidator : AbstractValidator<UserRegistrationRm>
{
    private readonly UserManager<User> _userManager;
    
    public UserCreationValidator(UserBaseValidator userBaseValidator, UserManager<User> userManager)
    {
        _userManager = userManager;
        
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
        var emailExists = await _userManager.Users.AnyAsync(u => u.Email == email, token);
        return !emailExists;
    }
}