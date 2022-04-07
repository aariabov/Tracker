using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Tracker.Web.Domain;
using Tracker.Web.RequestModels;

namespace Tracker.Web.Validators;

public class RoleCreationValidator : AbstractValidator<RoleCreationRm>
{
    private readonly RoleManager<Role> _roleManager;
    
    public RoleCreationValidator(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
        
        const int roleMinLen = 3;
        const int roleMaxLen = 100;
        
        RuleFor(role => role.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Название роли не может быть пустым")
            .Length(roleMinLen, roleMaxLen).WithMessage($"Название роли должно быть от {roleMinLen} до {roleMaxLen} символов")
            .MustAsync(UniqueRoleAsync).WithMessage("Название роли уже существует");
    }
    
    private async Task<bool> UniqueRoleAsync(string role, CancellationToken token)
    {
        var isRoleExists = await _roleManager.RoleExistsAsync(role);
        return !isRoleExists;
    }
}