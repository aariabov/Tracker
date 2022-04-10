using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tracker.Db.Models;
using Tracker.Users.RequestModels;

namespace Tracker.Users.Validators;

public class UserUpdatingValidator : AbstractValidator<UserUpdatingRm>
{
    private readonly UserManager<User> _userManager;
    
    public UserUpdatingValidator(UserBaseValidator userBaseValidator, UserManager<User> userManager)
    {
        _userManager = userManager;
        
        Include(userBaseValidator);
        RuleFor(user => user.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Идентификатор пользователя не может быть пустым")
            .MustAsync(UserExistsAsync).WithMessage("Пользователь не найден");
        RuleFor(user => user.Email)
            .MustAsync(UniqueEmailAsync).WithMessage("Email уже существует");
    }
    
    private async Task<bool> UniqueEmailAsync(UserUpdatingRm user, string email, CancellationToken token)
    {
        var emailExists = await _userManager.Users
            .AnyAsync(u => u.Email == email && u.Id != user.Id, token);
        return !emailExists;
    }
    
    private async Task<bool> UserExistsAsync(string id, CancellationToken token)
    {
        return await _userManager.Users.AnyAsync(u => u.Id == id, token);
    }
}