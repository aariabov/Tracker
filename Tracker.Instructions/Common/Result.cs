namespace Tracker.Instructions.Common;

public record Result
{
    public bool IsSuccess { get; }
    public Dictionary<string, string> ValidationErrors { get; }
    public IEnumerable<string> CommonValidationErrors { get; }

    protected Result(Dictionary<string, string>? validationErrors = null
        , IEnumerable<string>? commonErrors = null)
    {
        ValidationErrors = validationErrors ?? new Dictionary<string, string>();
        CommonValidationErrors = commonErrors ?? new List<string>();
        IsSuccess = !ValidationErrors.Any() && !CommonValidationErrors.Any();
    }

    public static Result Ok()
    {
        return new Result();
    }

    public static Result<T> Ok<T>(T value)
    {
        if (value is null)
        {
            throw new Exception("Value can't be null");
        }

        return new Result<T>(value);
    }

    public static Result Errors(Dictionary<string, string> errors)
    {
        if (errors is null || !errors.Any())
        {
            throw new Exception("Errors can't be empty");
        }

        return new Result(errors);
    }

    public static Result<T> CommonErrors<T>(IEnumerable<string> commonErrors)
    {
        if (commonErrors is null || !commonErrors.Any())
        {
            throw new Exception("Errors can't be empty");
        }

        return new Result<T>(default, validationErrors: null, commonErrors);
    }

    public static Result<T> Errors<T>(Dictionary<string, string> errors)
    {
        if (errors is null || !errors.Any())
        {
            throw new Exception("Errors can't be empty");
        }

        return new Result<T>(default, errors);
    }
}

public record Result<T> : Result
{
    public T? Value { get; }

    protected internal Result(T? value
        , Dictionary<string, string>? validationErrors = null
        , IEnumerable<string>? commonErrors = null)
        : base(validationErrors, commonErrors)
    {
        Value = value;
    }
}
