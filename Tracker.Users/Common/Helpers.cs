using System.Text.Json;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace Tracker.Users.Common;

public static class Helpers
{
    public static Dictionary<string, string> Format(this List<ValidationFailure> errors)
    {
        // тк на одно свойство мб несколько ошибок - берем первую (по дизайну)
        var errorsDict = errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(e => JsonNamingPolicy.CamelCase.ConvertName(e.Key)
                , e => e.ToArray().First().ErrorMessage);

        return errorsDict;
    }

    public static string Join(this IEnumerable<IdentityError> errors)
    {
        return string.Join(", ", errors.Select(e => e.Description));
    }
}
