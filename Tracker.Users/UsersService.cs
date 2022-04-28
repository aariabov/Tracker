using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Tracker.Common;
using Tracker.Db.Models;
using Tracker.Users.RequestModels;
using Tracker.Users.Validators;
using Tracker.Users.ViewModels;

namespace Tracker.Users;

public class UsersService
{
    private readonly UserManager<User> _userManager;
    private readonly UserValidationService _userValidationService;
    private readonly IUserRepository _userRepository;

    public UsersService(UserManager<User> userManager
        , UserValidationService userValidationService
        , IUserRepository userRepository)
    {
        _userManager = userManager;
        _userValidationService = userValidationService;
        _userRepository = userRepository;
    }

    public async Task<OrgStructElementVm[]> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUsers();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetUserByEmail(email);
    }

    public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        return await _userRepository.GetUserByRefreshToken(refreshToken);
    }

    public async Task<User?> GetUserByClaimsPrincipalAsync(ClaimsPrincipal principal)
    {
        return await _userManager.GetUserAsync(principal);
    }

    public async Task<string[]> GetUserRolesAsync(User user)
    {
        return (await _userManager.GetRolesAsync(user)).ToArray();
    }

    public async Task<bool> HasUserChildrenAsync(User user)
    {
        return await _userRepository.HasChildren(user.Id);
    }
    
    public async Task<Result<string>> RegisterAsync(UserRegistrationRm userRm)
    {
        var validationResult = await _userValidationService.ValidateRegistrationModelAsync(userRm);
        if (!validationResult.IsSuccess)
            return Result.Errors<string>(validationResult.ValidationErrors);
        
        var newUser = new User(userRm.Name, userRm.Email, userRm.BossId);
        var result = await _userManager.CreateAsync(newUser, userRm.Password);
        if (!result.Succeeded)
            throw new Exception(result.Errors.Join());

        if (userRm.Roles.Any())
        {
            var rolesResult = await _userManager.AddToRolesAsync(newUser, userRm.Roles);
            if (!rolesResult.Succeeded)
                throw new Exception(rolesResult.Errors.Join());
        }
        
        return Result.Ok(newUser.Id);
    }
    
    public async Task<Result> UpdateUserAsync(UserUpdatingRm userUpdatingRm)
    {
        var validationResult = await _userValidationService.ValidateUpdatingModelAsync(userUpdatingRm);
        if (!validationResult.IsSuccess)
            return Result.Errors<string>(validationResult.ValidationErrors);

        var updatedUser = await _userManager.FindByIdAsync(userUpdatingRm.Id);
        if (updatedUser is null)
            throw new Exception($"User with id {userUpdatingRm.Id} not found");
        
        updatedUser.UserName = userUpdatingRm.Name;
        updatedUser.Email = userUpdatingRm.Email;
        updatedUser.BossId = userUpdatingRm.BossId;

        await UpdateUserAsync(updatedUser);

        var userRoles = await _userManager.GetRolesAsync(updatedUser);
        var addedRoles = userUpdatingRm.Roles.Except(userRoles);
        var removedRoles = userRoles.Except(userUpdatingRm.Roles);
 
        var roleAddingResult = await _userManager.AddToRolesAsync(updatedUser, addedRoles);
        if (!roleAddingResult.Succeeded)
            throw new Exception(roleAddingResult.Errors.Join());
        
        var roleRemovingResult = await _userManager.RemoveFromRolesAsync(updatedUser, removedRoles);
        if (!roleRemovingResult.Succeeded)
            throw new Exception(roleRemovingResult.Errors.Join());
        
        return Result.Ok();
    }
    
    public async Task UpdateUserAsync(User updatedUser)
    {
        var result = await _userManager.UpdateAsync(updatedUser);
        if (!result.Succeeded)
            throw new Exception(result.Errors.Join());
    }
    
    public async Task<Result> DeleteUserAsync(UserDeletingRm userDeletingRm)
    {
        var validationResult = await _userValidationService.ValidateDeletingModelAsync(userDeletingRm);
        if (!validationResult.IsSuccess)
            return Result.Errors<string>(validationResult.ValidationErrors);

        var deletedUser = await _userManager.FindByIdAsync(userDeletingRm.Id);
        if (deletedUser is null)
            throw new Exception($"User with id {userDeletingRm.Id} not found");
        
        var result = await _userManager.DeleteAsync(deletedUser);
        if (!result.Succeeded)
            throw new Exception(result.Errors.Join());

        return Result.Ok();
    }
}