using System.Text.Json;
using FluentValidation.Results;
using Tracker.Web.ViewModels;

namespace Tracker.Web;

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
}