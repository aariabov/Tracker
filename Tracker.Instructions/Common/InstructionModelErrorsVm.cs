namespace Tracker.Instructions.Common;

public class InstructionModelErrorsVm
{
    public IEnumerable<string> CommonErrors { get; set; }
    public Dictionary<string, string> ModelErrors { get; set; }

    public InstructionModelErrorsVm(Result result)
    {
        ModelErrors = result.ValidationErrors;
        CommonErrors = result.CommonValidationErrors;
    }

    public InstructionModelErrorsVm()
    {
    }
}
