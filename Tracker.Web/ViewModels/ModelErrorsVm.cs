namespace Tracker.Web.ViewModels;

public class ModelErrorsVm
{
    public Dictionary<string, string> ModelErrors { get; }

    public ModelErrorsVm(Dictionary<string, string> modelErrors)
    {
        ModelErrors = modelErrors;
    }
}