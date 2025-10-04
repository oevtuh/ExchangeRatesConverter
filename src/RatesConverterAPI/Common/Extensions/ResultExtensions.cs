namespace RatesConverterAPI.Common.Extensions;

/// <summary>
/// Extension methods for Result to HTTP response conversion
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a Result to an HTTP response
    /// </summary>
    /// <typeparam name="T">The type of the result value</typeparam>
    /// <param name="result">The result to convert</param>
    /// <returns>HTTP result</returns>
    public static IResult ToHttpResult<T>(this Models.Result<T> result)
    {
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new 
            {
                error = result.Error,
                timestamp = DateTime.UtcNow
            });
    }
}