namespace Tracker.Roles.Common;

public class RoleModelErrorsVm
{
    public IEnumerable<string> CommonErrors { get; set; }
    public Dictionary<string, string> ModelErrors { get; set; }

    public RoleModelErrorsVm(Result result)
    {
        ModelErrors = result.ValidationErrors;
        CommonErrors = result.CommonValidationErrors;
    }

    public RoleModelErrorsVm()
    {
    }
}
