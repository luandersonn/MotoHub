namespace MotoHub.Domain.Common;

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public ResultErrorType? ErrorType { get; }

    protected Result(bool isSuccess, string? error, ResultErrorType? errorType)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorType = errorType;
    }

    public static Result Success() => new(true, null, null);
    public static Result Failure(string error, ResultErrorType errorType) => new(false, error, errorType);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T? value, bool isSuccess, string? error, ResultErrorType? errorType)
        : base(isSuccess, error, errorType)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(value, true, null, null);
    public static new Result<T> Failure(string error, ResultErrorType errorType) => new(default, false, error, errorType);
}