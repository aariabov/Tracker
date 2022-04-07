using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tracker.Web.Domain;
using Tracker.Web.RequestModels;

namespace Tracker.Web.Validators;

public class RoleUpdatingValidator : AbstractValidator<RoleUpdatingRm>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly string _adminRole;
    
    public RoleUpdatingValidator(RoleManager<Role> roleManager
        , RoleCreationValidator roleCreationValidator
        , IConfiguration config)
    {
        _roleManager = roleManager;
        _adminRole = config.GetValue<string>("DefaultAdmin:Role");
        
        Include(roleCreationValidator);
        RuleFor(role => role)
            .CustomAsync(MustBeValid);
    }
    
    private async Task MustBeValid(RoleUpdatingRm roleRm, ValidationContext<RoleUpdatingRm> context, CancellationToken token)
    {
        var role = await _roleManager.Roles.SingleOrDefaultAsync(r => r.Id == roleRm.Id, token);
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