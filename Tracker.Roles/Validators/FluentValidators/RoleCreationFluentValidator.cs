using FluentValidation;
using Tracker.Roles.RequestModels;

namespace Tracker.Roles.Validators.FluentValidators;

public class RoleCreationFluentValidator : AbstractValidator<RoleCreationRm>
{
    public const int RoleMinLen = 3;
    public const int RoleMaxLen = 100;
    
    private readonly IRoleRepo _roleRepo;
    
    public RoleCreationFluentValidator(IRoleRepo roleRepo)
    {
        _roleRepo = roleRepo;
        
        RuleFor(role => role.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Название роли не может быть пустым")
            .Length(RoleMinLen, RoleMaxLen).WithMessage($"Название роли должно быть от {RoleMinLen} до {RoleMaxLen} символов")
            .MustAsync(UniqueRoleAsync).WithMessage("Название роли уже существует");
    }
    
    private async Task<bool> UniqueRoleAsync(string role, CancellationToken token)
    {
        var isRoleExists = await _roleRepo.RoleExistsAsync(role);
        return !isRoleExists;
    }
}