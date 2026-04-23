namespace FlowinglyImport.Api.Common;

public class Result<T>
{
    private Result(T? value, IReadOnlyList<ValidationError> errors)
    {
        Value = value;
        Errors = errors;
    }

    public T? Value { get; }

    public IReadOnlyList<ValidationError> Errors { get; }

    public bool IsSuccess => Errors.Count == 0;

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, []);
    }

    public static Result<T> Failure(IReadOnlyList<ValidationError> errors)
    {
        return new Result<T>(default, errors);
    }
}
