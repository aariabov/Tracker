using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tracker.Db;
using Tracker.Db.Models;
using Tracker.Users.RequestModels;

namespace Tracker.Users.Validators;

public class UserDeletingValidator : AbstractValidator<UserDeletingRm>
{
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;
    private readonly string _adminEmail;
    
    public UserDeletingValidator(AppDbContext db, UserManager<User> userManager, IConfiguration config)
    {
        _db = db;
        _userManager = userManager;
        _adminEmail = config.GetValue<string>("DefaultAdmin:Email");
        
        RuleFor(rm => rm.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Идентификатор не может быть пустым")
            .CustomAsync(MustBeValid);
    }
    
    private async Task MustBeValid(string userId, ValidationContext<UserDeletingRm> context, CancellationToken token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            context.AddFailure($"Пользователь с идентификатором {userId} не найден");
            return;
        }

        var hasChildren = await _userManager.Users.AnyAsync(u => u.BossId == user.Id, token);
        if (hasChildren)
        {
            context.AddFailure("У пользователя есть подчиненные");
            return;
        }

        var hasInstructions = await _db.Instructions
            .AnyAsync(i => i.CreatorId == user.Id || i.ExecutorId == user.Id, token);
        if (hasInstructions)
        {
            context.AddFailure("У пользователя есть поручения");
            return;
        }
        
        if (user.IsAdmin(_adminEmail))
        {
            context.AddFailure("Администратора нельзя удалить");
            return;
        }
    }
}