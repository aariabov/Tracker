using FluentValidation;
using Tracker.Roles.RequestModels;

namespace Tracker.Roles.Validators.FluentValidators;

public class RoleDeletingFluentValidator : AbstractValidator<RoleDeletingRm>
{
    private readonly IRoleRepo _roleRepo;

    public RoleDeletingFluentValidator(IRoleRepo roleRepo)
    {
        _roleRepo = roleRepo;

        RuleFor(rm => rm.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Идентификатор роли не может быть пустым")
            .CustomAsync(MustBeValid);
    }

    private async Task MustBeValid(string roleId, ValidationContext<RoleDeletingRm> context, CancellationToken token)
    {
        var role = await _roleRepo.GetRoleById(roleId);
        if (role is null)
        {
            context.AddFailure($"Роль с идентификатором {roleId} не найдена");
            return;
        }

        var isAnyUserBelongToRole = await _roleRepo.IsAnyUserBelongToRole(roleId);
        if (isAnyUserBelongToRole)
        {
            context.AddFailure($"Пользователи используют роль '{role.Name}'");
            return;
        }
    }
}
