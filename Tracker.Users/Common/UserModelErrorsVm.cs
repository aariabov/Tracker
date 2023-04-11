namespace Tracker.Users.Common;

public class UserModelErrorsVm
{
    public IEnumerable<string> CommonErrors { get; set; }
    public Dictionary<string, string> ModelErrors { get; set; }

    public UserModelErrorsVm(Result result)
    {
        ModelErrors = result.ValidationErrors;
        CommonErrors = result.CommonValidationErrors;
    }

    public UserModelErrorsVm()
    {
    }
}
