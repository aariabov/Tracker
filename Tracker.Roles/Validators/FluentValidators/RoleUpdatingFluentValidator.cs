using FluentValidation;
using Tracker.Roles.RequestModels;

namespace Tracker.Roles.Validators.FluentValidators;

public class RoleUpdatingFluentValidator : AbstractValidator<RoleUpdatingRm>
{
    private readonly IRoleRepo _roleRepo;
    private readonly string _adminRole;
    
    public RoleUpdatingFluentValidator(IRoleRepo roleRepo
        , RoleCreationFluentValidator roleCreationFluentValidator
        , string adminRole)
    {
        _roleRepo = roleRepo;
        _adminRole = adminRole;
        
        Include(roleCreationFluentValidator);
        RuleFor(role => role)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Идентификатор роли не может быть пустым")
            .CustomAsync(MustBeValid);
    }
    
    private async Task MustBeValid(RoleUpdatingRm roleRm, ValidationContext<RoleUpdatingRm> context, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(roleRm.Id))
        {
            context.AddFailure(nameof(RoleUpdatingRm.Name), "Идентификатор роли не может быть пустым");
            return;
        }
        
        var role = await _roleRepo.GetRoleById(roleRm.Id);
        if (role is null)
        {
            context.AddFailure(nameof(RoleUpdatingRm.Name), $"Роль с идентификатором {roleRm.Id} не найдена");
            return;
        }

        if (role.Name == _adminRole)
        {
            context.AddFailure(nameof(RoleUpdatingRm.Name), "Роль 'Admin' нельзя редактировать");
            return;
        }
    }
}