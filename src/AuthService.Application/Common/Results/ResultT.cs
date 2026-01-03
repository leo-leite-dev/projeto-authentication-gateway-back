namespace AuthService.Application.Common.Results;

public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool success, T? value, string? error)
        : base(success, error)
    {
        Value = value;
    }

    public static Result<T> Ok(T value) => new(true, value, null);
}
