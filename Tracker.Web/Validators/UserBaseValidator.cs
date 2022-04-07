using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tracker.Web.Db;
using Tracker.Web.Domain;
using Tracker.Web.RequestModels;

namespace Tracker.Web.Validators;

public class UserBaseValidator : AbstractValidator<UserBaseRm>
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _db;
    
    public UserBaseValidator(UserManager<User> userManager, AppDbContext db)
    {
        _userManager = userManager;
        _db = db;

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
            var isRoleExists = await _db.Roles.AnyAsync(r => r.Name == role, token);
            if (!isRoleExists)
            {
                context.AddFailure($"Роль '{role}' не найдена");
                return;
            }
        }
    }
    
    private async Task<bool> BossExistsAsync(string? bossId, CancellationToken token)
    {
        return await _userManager.Users.AnyAsync(u => u.Id == bossId, token);
    }
}