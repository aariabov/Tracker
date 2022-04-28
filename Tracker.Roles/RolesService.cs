using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tracker.Common;
using Tracker.Db;
using Tracker.Db.Models;
using Tracker.Roles.RequestModels;
using Tracker.Roles.Validators;

namespace Tracker.Roles;

public class RolesService
{
    private readonly RoleValidationService _validationService;
    private readonly RoleManager<Role> _roleManager;
    private readonly IRoleRepo _roleRepo;

    public RolesService(RoleValidationService validationService
        , RoleManager<Role> roleManager
        , IRoleRepo roleRepo)
    {
        _validationService = validationService;
        _roleManager = roleManager;
        _roleRepo = roleRepo;
    }

    public async Task<RoleVm[]> GetAllRoles()
    {
        return await _roleRepo.GetAllRoles();
    }

    public async Task<Result<string>> CreateRole(RoleCreationRm roleCreationRm)
    {
        var validationResult = await _validationService.ValidateCreationModelAsync(roleCreationRm);
        if (!validationResult.IsSuccess)
            return Result.Errors<string>(validationResult.ValidationErrors);
        
        var newRole = new Role(roleCreationRm.Name);
        var result =  await _roleManager.CreateAsync(newRole);
        if (result.Succeeded)
            return Result.Ok(newRole.Id);
        
        throw new Exception(result.Errors.Join());
    }

    public async Task<Result> UpdateRole(RoleUpdatingRm roleUpdatingRm)
    {
        var validationResult = await _validationService.ValidateUpdatingModelAsync(roleUpdatingRm);
        if (!validationResult.IsSuccess)
            return Result.Errors(validationResult.ValidationErrors);

        var updatedRole = await _roleManager.FindByIdAsync(roleUpdatingRm.Id);
        updatedRole.Name = roleUpdatingRm.Name;
        var result = await _roleManager.UpdateAsync(updatedRole);
        if (result.Succeeded)
            return Result.Ok();
        
        throw new Exception(result.Errors.Join());
    }

    public async Task<Result> DeleteRole(RoleDeletingRm roleDeletingRm)
    {
        var validationResult = await _validationService.ValidateDeletingModelAsync(roleDeletingRm);
        if (!validationResult.IsSuccess)
            return Result.Errors(validationResult.ValidationErrors);

        var deletedRole = await _roleManager.FindByIdAsync(roleDeletingRm.Id);
        var result = await _roleManager.DeleteAsync(deletedRole);
        if (result.Succeeded)
            return Result.Ok();
        
        throw new Exception(result.Errors.Join());
    }
}