using Microsoft.Extensions.Configuration;
using Tracker.Common;
using Tracker.Db;
using Tracker.Users.RequestModels;
using Tracker.Users.Validators.FluentValidators;

namespace Tracker.Users.Validators;

// TODO: покрыть юнит тестами
public class UserValidationService
{
    private readonly IUserRepository _userRepository;
    private readonly AppDbContext _db;
    private readonly string _adminEmail;

    public UserValidationService(AppDbContext db, IUserRepository userRepository, IConfiguration config)
    {
        _db = db;
        _userRepository = userRepository;
        _adminEmail = config.GetValue<string>("DefaultAdmin:Email");
    }

    public async Task<Result> ValidateRegistrationModelAsync(UserRegistrationRm userRm)
    {
        var baseValidator = new UserBaseValidator(_userRepository);
        var validator = new UserCreationValidator(baseValidator, _userRepository);
        var validationResult = await validator.ValidateAsync(userRm);
        if (validationResult.IsValid)
            return Result.Ok();
        
        return Result.Errors<string>(validationResult.Errors.Format());
    }
    
    public async Task<Result> ValidateUpdatingModelAsync(UserUpdatingRm userUpdatingRm)
    {
        var baseValidator = new UserBaseValidator(_userRepository);
        var validator = new UserUpdatingValidator(baseValidator, _userRepository);
        var validationResult = await validator.ValidateAsync(userUpdatingRm);
        if (validationResult.IsValid)
            return Result.Ok();
            
        return Result.Errors<string>(validationResult.Errors.Format());
    }
    
    public async Task<Result> ValidateDeletingModelAsync(UserDeletingRm userDeletingRm)
    {
        var validator = new UserDeletingValidator(_db, _userRepository, _adminEmail);
        var validationResult = await validator.ValidateAsync(userDeletingRm);
        if (validationResult.IsValid)
            return Result.Ok();
            
        return Result.Errors<string>(validationResult.Errors.Format());
    }
}