namespace optiflow_platform.Shared.Application.Model;

/// <summary>
/// Represents the outcome of a void operation that can succeed or fail with an error enum and message.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public Enum? Error { get; }
    public string Message { get; }

    private Result(bool isSuccess, Enum? error, string message)
    {
        IsSuccess = isSuccess;
        Error = error;
        Message = message;
    }

    public static Result Success() => new(true, null, string.Empty);

    public static Result Failure(Enum error, string message) => new(false, error, message);
}

/// <summary>
/// Represents the outcome of an operation that can succeed with a value or fail with an error enum and message.
/// </summary>
/// <typeparam name="TValue">The type of the successful result value.</typeparam>
public class Result<TValue>
{
    public bool IsSuccess { get; }
    public TValue? Value { get; }
    public Enum? Error { get; }
    public string Message { get; }

    private Result(bool isSuccess, TValue? value, Enum? error, string message)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Message = message;
    }

    public static Result<TValue> Success(TValue value) => new(true, value, null, string.Empty);

    public static Result<TValue> Failure(Enum error, string message) => new(false, default, error, message);
}
