using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tracker.Web.Db;
using Tracker.Web.Domain;
using Tracker.Web.RequestModels;

namespace Tracker.Web.Validators;

public class RoleDeletingValidator : AbstractValidator<RoleDeletingRm>
{
    private readonly AppDbContext _db;
    
    public RoleDeletingValidator(AppDbContext db)
    {
        _db = db;
        RuleFor(rm => rm.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Идентификатор роли не может быть пустым")
            .CustomAsync(MustBeValid);
    }
    
    private async Task MustBeValid(string roleId, ValidationContext<RoleDeletingRm> context, CancellationToken token)
    {
        var role = await _db.Roles.SingleOrDefaultAsync(r => r.Id == roleId, token);
        if (role is null)
        {
            context.AddFailure($"Роль с идентификатором {roleId} не найдена");
            return;
        }

        var isAnyUserBelongToRole = await _db.UserRoles.AnyAsync(r => r.RoleId == roleId, token);
        if (isAnyUserBelongToRole)
        {
            context.AddFailure($"Пользователи используют роль '{role.Name}'");
            return;
        }
    }
}