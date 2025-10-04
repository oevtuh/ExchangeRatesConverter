namespace RatesConverterAPI.Models;

/// <summary>
/// Result pattern for error handling
/// </summary>
/// <typeparam name="T">The type of the result value</typeparam>
public record Result<T>
{
    public T? Value { get; init; }
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }
    
    public static Result<T> Success(T value) => new() { Value = value, IsSuccess = true };
    public static Result<T> Failure(string error) => new() { Error = error, IsSuccess = false };
}

/// <summary>
/// Result pattern for operations without return value
/// </summary>
public record Result
{
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }
    
    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(string error) => new() { Error = error, IsSuccess = false };
}