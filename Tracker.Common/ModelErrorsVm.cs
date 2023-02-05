namespace Tracker.Common;

public class ModelErrorsVm
{
    public IEnumerable<string> CommonErrors { get; set; }
    public Dictionary<string, string> ModelErrors { get; set; }

    public ModelErrorsVm(Result result)
    {
        ModelErrors = result.ValidationErrors;
        CommonErrors = result.CommonValidationErrors;
    }

    public ModelErrorsVm()
    {
    }
}
