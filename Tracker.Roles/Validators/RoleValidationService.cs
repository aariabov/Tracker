using Microsoft.Extensions.Configuration;
using Riabov.Tracker.Common;
using Tracker.Roles.RequestModels;
using Tracker.Roles.Validators.FluentValidators;

namespace Tracker.Roles.Validators;

public class RoleValidationService
{
    private readonly IRoleRepo _roleRepo;
    private readonly string _adminRole;

    public RoleValidationService(IRoleRepo roleRepo, IConfiguration config)
    {
        _roleRepo = roleRepo;
        _adminRole = config["DefaultAdmin:Role"];
    }

    public async Task<Result> ValidateCreationModelAsync(RoleCreationRm roleCreationRm)
    {
        var validator = new RoleCreationFluentValidator(_roleRepo);
        var validationResult = await validator.ValidateAsync(roleCreationRm);
        if (validationResult.IsValid)
        {
            return Result.Ok();
        }

        return Result.Errors<string>(validationResult.Errors.Format());
    }

    public async Task<Result> ValidateUpdatingModelAsync(RoleUpdatingRm roleUpdatingRm)
    {
        var creationValidator = new RoleCreationFluentValidator(_roleRepo);
        var validator = new RoleUpdatingFluentValidator(_roleRepo, creationValidator, _adminRole);
        var validationResult = await validator.ValidateAsync(roleUpdatingRm);
        if (validationResult.IsValid)
        {
            return Result.Ok();
        }

        return Result.Errors<string>(validationResult.Errors.Format());
    }

    public async Task<Result> ValidateDeletingModelAsync(RoleDeletingRm roleDeletingRm)
    {
        var validator = new RoleDeletingFluentValidator(_roleRepo);
        var validationResult = await validator.ValidateAsync(roleDeletingRm);
        if (validationResult.IsValid)
        {
            return Result.Ok();
        }

        return Result.Errors<string>(validationResult.Errors.Format());
    }
}
