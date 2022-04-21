namespace Tracker.Common;

public record Result
{
    public bool IsSuccess { get; }
    public Dictionary<string, string> ValidationErrors { get; }

    protected Result(Dictionary<string, string>? validationErrors = null)
    {
        ValidationErrors = validationErrors ?? new Dictionary<string, string>();
        IsSuccess = !ValidationErrors.Any();
    }

    public static Result Ok()
    {
        return new Result();
    }

    public static Result<T> Ok<T>(T value)
    {
        if (value is null)
            throw new Exception("Value can't be null");
        
        return new Result<T>(value);
    }
    
    public static Result Errors(Dictionary<string, string> errors)
    {
        if (errors is null || !errors.Any())
            throw new Exception("Errors can't be empty");

        return new Result(errors);
    }
    
    public static Result<T> Errors<T>(Dictionary<string, string> errors)
    {
        if (errors is null || !errors.Any())
            throw new Exception("Errors can't be empty");

        return new Result<T>(default, errors);
    }
}

public record Result<T> : Result
{
    public T? Value { get; }

    protected internal Result(T? value, Dictionary<string, string>? validationErrors = null)
        : base(validationErrors)
    {
        Value = value;
    }
}
