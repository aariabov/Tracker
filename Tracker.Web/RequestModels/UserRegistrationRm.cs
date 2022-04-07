namespace Tracker.Web.RequestModels;

public class UserRegistrationRm : UserBaseRm
{
    public string Password { get; set; } = string.Empty;
}