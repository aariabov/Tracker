using System.Text.Json;
using FluentValidation.Results;

namespace Tracker.Common;

public static class Helpers
{
    public static ModelErrorsVm Format(this List<ValidationFailure> errors)
    {
        // тк на одно свойство мб несколько ошибок - берем первую (по дизайну)
        var errorsDict = errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(e => JsonNamingPolicy.CamelCase.ConvertName(e.Key)
                        , e => e.ToArray().First().ErrorMessage);

        return new ModelErrorsVm(errorsDict);
    }
    
    public static string GetString(this ExecStatus status)
    {
        switch (status)
        {
            case ExecStatus.InWork: return "В работе";
            case ExecStatus.InWorkOverdue: return "В работе просрочено";
            case ExecStatus.Completed: return "Выполнено в срок";
            case ExecStatus.CompletedOverdue: return "Выполнено с нарушением срока";
            default: throw new Exception("Unknown ExecStatus");
        }
    }
}