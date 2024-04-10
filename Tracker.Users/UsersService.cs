using System.Security.Claims;
using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Riabov.Tracker.Common;
using Riabov.Tracker.Common.Cache;
using Tracker.Db.Models;
using Tracker.Db.Transactions;
using Tracker.Db.UnitOfWorks;
using Tracker.Users.Kafka;
using Tracker.Users.RequestModels;
using Tracker.Users.Validators;
using Tracker.Users.ViewModels;

namespace Tracker.Users;

public class UsersService
{
    private readonly string _userWasUpdatedTopic;
    private readonly string _userWasAddedTopic;
    private readonly string _userWasDeletedTopic;
    private readonly TimeSpan _cacheExpire;

    private readonly IUserManagerService _userManagerService;
    private readonly IUserValidationService _userValidationService;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITransactionManager _transactionManager;
    private readonly IAuditWebService _auditWebService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProducer _producer;
    private readonly ICacheService _cacheService;

    public UsersService(IUserManagerService userManagerService
        , IUserValidationService userValidationService
        , IUserRepository userRepository
        , IHttpContextAccessor httpContextAccessor
        , ITransactionManager transactionManager
        , IAuditWebService auditWebService
        , IUnitOfWork unitOfWork
        , IProducer producer
        , IConfiguration config
        , ICacheService cacheService)
    {
        _userManagerService = userManagerService;
        _userValidationService = userValidationService;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
        _transactionManager = transactionManager;
        _auditWebService = auditWebService;
        _unitOfWork = unitOfWork;
        _producer = producer;
        _cacheService = cacheService;

        _userWasUpdatedTopic = config.GetValue<string>("Kafka:UserWasUpdatedTopic");
        _userWasAddedTopic = config.GetValue<string>("Kafka:UserWasAddedTopic");
        _userWasDeletedTopic = config.GetValue<string>("Kafka:UserWasDeletedTopic");
        _cacheExpire = TimeSpan.FromMinutes(config.GetValue<int>("Cache:GetAllUsersCacheExpireInMinutes"));
    }

    public async Task<OrgStructElementVm[]> GetAllUsersAsync()
    {
        var usersFromCache = await _cacheService.GetAsync<OrgStructElementVm[]>("users");
        if (usersFromCache is not null)
        {
            return usersFromCache;
        }

        var users = await _userRepository.GetOrgStructVm();
        await _cacheService.SetAsync("users", users, _cacheExpire);

        return users;
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
        var auditLogId = 0;

        using var transaction = _transactionManager.BeginTransaction();
        try
        {
            var validationResult = await _userValidationService.ValidateRegistrationModelAsync(userRm);
            if (!validationResult.IsSuccess)
            {
                return Result.Errors<string>(validationResult.ValidationErrors);
            }

            var newUser = new User(userRm.Name, userRm.Email, userRm.BossId);
            var result = await _userManagerService.CreateAsync(newUser, userRm.Password);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.Join());
            }

            var logModel = new LogModel
            {
                Type = AuditType.Create,
                EntityId = newUser.Id,
                EntityName = newUser.GetType().Name,
                UserId = GetCurrentUserId()
            };
            auditLogId = await _auditWebService.CreateLogAsync(logModel);

            if (userRm.Roles.Any())
            {
                var rolesResult = await _userManagerService.AddToRolesAsync(newUser, userRm.Roles);
                if (!rolesResult.Succeeded)
                {
                    throw new Exception(rolesResult.Errors.Join());
                }
            }

            await _unitOfWork.SaveChangesAsync();

            var kafkaUser = KafkaUser.CreateFromUser(newUser);
            await _producer.Produce(_userWasAddedTopic, newUser.Id, kafkaUser);

            await transaction.CommitAsync();
            return Result.Ok(newUser.Id);
        }
        catch
        {
            if (auditLogId > 0)
            {
                await _auditWebService.DeleteLogAsync(new DeleteLogModel { Id = auditLogId });
            }

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
            {
                return Result.Errors<string>(validationResult.ValidationErrors);
            }

            var updatedUser = await _userManagerService.FindByIdAsync(userUpdatingRm.Id);
            if (updatedUser is null)
            {
                throw new Exception($"User with id {userUpdatingRm.Id} not found");
            }

            updatedUser.UserName = userUpdatingRm.Name;
            updatedUser.Email = userUpdatingRm.Email;
            updatedUser.BossId = userUpdatingRm.BossId;

            await UpdateUserAsync(updatedUser);

            var userRoles = await _userManagerService.GetRolesAsync(updatedUser);
            var addedRoles = userUpdatingRm.Roles.Except(userRoles);
            var removedRoles = userRoles.Except(userUpdatingRm.Roles);

            var roleAddingResult = await _userManagerService.AddToRolesAsync(updatedUser, addedRoles);
            if (!roleAddingResult.Succeeded)
            {
                throw new Exception(roleAddingResult.Errors.Join());
            }

            var roleRemovingResult = await _userManagerService.RemoveFromRolesAsync(updatedUser, removedRoles);
            if (!roleRemovingResult.Succeeded)
            {
                throw new Exception(roleRemovingResult.Errors.Join());
            }

            var logModel = new LogModel
            {
                Type = AuditType.Update,
                EntityId = updatedUser.Id,
                EntityName = updatedUser.GetType().Name,
                UserId = GetCurrentUserId()
            };
            await _auditWebService.CreateLogAsync(logModel);

            var kafkaUser = KafkaUser.CreateFromUser(updatedUser);
            await _producer.Produce(_userWasUpdatedTopic, updatedUser.Id, kafkaUser);
            // TODO: вот тут может отпасть БД, но сообщение в Кавку уже отправлено - надо сделать компенсирующую транзакцию
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
        {
            throw new Exception(result.Errors.Join());
        }
    }

    public async Task<Result> DeleteUserAsync(UserDeletingRm userDeletingRm)
    {
        using var transaction = _transactionManager.BeginTransaction();
        try
        {
            var validationResult = await _userValidationService.ValidateDeletingModelAsync(userDeletingRm);
            if (!validationResult.IsSuccess)
            {
                return Result.Errors<string>(validationResult.ValidationErrors);
            }

            var deletedUser = await _userManagerService.FindByIdAsync(userDeletingRm.Id);
            if (deletedUser is null)
            {
                throw new Exception($"User with id {userDeletingRm.Id} not found");
            }

            var result = await _userManagerService.DeleteAsync(deletedUser);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.Join());
            }

            var logModel = new LogModel
            {
                Type = AuditType.Delete,
                EntityId = deletedUser.Id,
                EntityName = deletedUser.GetType().Name,
                UserId = GetCurrentUserId()
            };
            await _auditWebService.CreateLogAsync(logModel);

            await _producer.Produce(_userWasDeletedTopic, deletedUser.Id, deletedUser.Id);

            await transaction.CommitAsync();
            return Result.Ok();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public string GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //var user = await _userManager.GetUserAsync(HttpContext.User);
        if (userId is null)
        {
            throw new Exception("User not found");
        }

        return userId;
    }

    public async Task<User> GetCurrentUser()
    {
        return await _userManagerService.GetUserAsync(_httpContextAccessor.HttpContext!.User);
    }
}
