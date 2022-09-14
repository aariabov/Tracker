using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Tracker.Audit;
using Tracker.Common;
using Tracker.Db.Models;
using Tracker.Db.Transactions;
using Tracker.Db.UnitOfWorks;
using Tracker.Users.RequestModels;
using Tracker.Users.Validators;
using Tracker.Users.ViewModels;

namespace Tracker.Users;

public class UsersService
{
    private readonly IUserManagerService _userManagerService;
    private readonly IUserValidationService _userValidationService;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITransactionManager _transactionManager;
    private readonly IAuditService _auditService;
    private readonly IUnitOfWork _unitOfWork;

    public UsersService(IUserManagerService userManagerService
        , IUserValidationService userValidationService
        , IUserRepository userRepository
        , IHttpContextAccessor httpContextAccessor
        , ITransactionManager transactionManager
        , IAuditService auditService
        , IUnitOfWork unitOfWork)
    {
        _userManagerService = userManagerService;
        _userValidationService = userValidationService;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
        _transactionManager = transactionManager;
        _auditService = auditService;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrgStructElementVm[]> GetAllUsersAsync()
    {
        return await _userRepository.GetOrgStructVm();
    }

    public async Task<User[]> GetUsersTreeAsync()
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

    public async Task<string[]> GetUserRolesAsync(User user)
    {
        return (await _userManagerService.GetRolesAsync(user)).ToArray();
    }

    public async Task<bool> HasUserChildrenAsync(string userId)
    {
        return await _userRepository.HasChildren(userId);
    }

    public async Task<bool> IsUserExistsAsync(string userId)
    {
        return await _userRepository.IsUserExistsAsync(userId);
    }
    
    public async Task<Result<string>> RegisterAsync(UserRegistrationRm userRm)
    {
        using var transaction = _transactionManager.BeginTransaction();
        try
        {
            var validationResult = await _userValidationService.ValidateRegistrationModelAsync(userRm);
            if (!validationResult.IsSuccess)
                return Result.Errors<string>(validationResult.ValidationErrors);
        
            var newUser = new User(userRm.Name, userRm.Email, userRm.BossId);
            var result = await _userManagerService.CreateAsync(newUser, userRm.Password);
            if (!result.Succeeded)
                throw new Exception(result.Errors.Join());
        
            _auditService.LogAsync(AuditType.Create, newUser.Id, newUser.GetType().Name, GetCurrentUserId());
            
            if (userRm.Roles.Any())
            {
                var rolesResult = await _userManagerService.AddToRolesAsync(newUser, userRm.Roles);
                if (!rolesResult.Succeeded)
                    throw new Exception(rolesResult.Errors.Join());
            }

            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();
            return Result.Ok(newUser.Id);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<Result> UpdateUserAsync(UserUpdatingRm userUpdatingRm)
    {
        using var transaction = _transactionManager.BeginTransaction();
        try
        {
            var validationResult = await _userValidationService.ValidateUpdatingModelAsync(userUpdatingRm);
            if (!validationResult.IsSuccess)
                return Result.Errors<string>(validationResult.ValidationErrors);

            var updatedUser = await _userManagerService.FindByIdAsync(userUpdatingRm.Id);
            if (updatedUser is null)
                throw new Exception($"User with id {userUpdatingRm.Id} not found");
        
            updatedUser.UserName = userUpdatingRm.Name;
            updatedUser.Email = userUpdatingRm.Email;
            updatedUser.BossId = userUpdatingRm.BossId;

            await UpdateUserAsync(updatedUser);

            var userRoles = await _userManagerService.GetRolesAsync(updatedUser);
            var addedRoles = userUpdatingRm.Roles.Except(userRoles);
            var removedRoles = userRoles.Except(userUpdatingRm.Roles);
 
            var roleAddingResult = await _userManagerService.AddToRolesAsync(updatedUser, addedRoles);
            if (!roleAddingResult.Succeeded)
                throw new Exception(roleAddingResult.Errors.Join());
        
            var roleRemovingResult = await _userManagerService.RemoveFromRolesAsync(updatedUser, removedRoles);
            if (!roleRemovingResult.Succeeded)
                throw new Exception(roleRemovingResult.Errors.Join());
        
            await transaction.CommitAsync();
            return Result.Ok();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task UpdateUserAsync(User updatedUser)
    {
        var result = await _userManagerService.UpdateAsync(updatedUser);
        if (!result.Succeeded)
            throw new Exception(result.Errors.Join());
    }
    
    public async Task<Result> DeleteUserAsync(UserDeletingRm userDeletingRm)
    {
        var validationResult = await _userValidationService.ValidateDeletingModelAsync(userDeletingRm);
        if (!validationResult.IsSuccess)
            return Result.Errors<string>(validationResult.ValidationErrors);

        var deletedUser = await _userManagerService.FindByIdAsync(userDeletingRm.Id);
        if (deletedUser is null)
            throw new Exception($"User with id {userDeletingRm.Id} not found");
        
        var result = await _userManagerService.DeleteAsync(deletedUser);
        if (!result.Succeeded)
            throw new Exception(result.Errors.Join());

        return Result.Ok();
    }

    public string GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //var user = await _userManager.GetUserAsync(HttpContext.User);
        if (userId is null)
            throw new Exception("User not found");

        return userId;
    }

    public async Task<User> GetCurrentUser()
    {
        return await _userManagerService.GetUserAsync(_httpContextAccessor.HttpContext!.User);
    }
}