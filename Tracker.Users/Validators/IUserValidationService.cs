using Riabov.Tracker.Common;
using Tracker.Users.RequestModels;

namespace Tracker.Users.Validators;

public interface IUserValidationService
{
    Task<Result> ValidateRegistrationModelAsync(UserRegistrationRm userRm);
    Task<Result> ValidateUpdatingModelAsync(UserUpdatingRm userUpdatingRm);
    Task<Result> ValidateDeletingModelAsync(UserDeletingRm userDeletingRm);
}
