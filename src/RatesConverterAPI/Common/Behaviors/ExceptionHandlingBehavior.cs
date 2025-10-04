using MediatR;

namespace RatesConverterAPI.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior for handling exceptions in all handlers
/// </summary>
public class ExceptionHandlingBehavior<TRequest, TResponse>(
    ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger) 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            
            logger.LogError(ex, "Request {RequestName} failed with exception", requestName);
            
            // If TResponse is Result<T>, we can return a failure result
            if (typeof(TResponse).IsGenericType && 
                typeof(TResponse).GetGenericTypeDefinition() == typeof(Models.Result<>))
            {
                var resultType = typeof(TResponse).GetGenericArguments()[0];
                var failureMethod = typeof(Models.Result<>)
                    .MakeGenericType(resultType)
                    .GetMethod("Failure", new[] { typeof(string) });
                
                var failureResult = failureMethod?.Invoke(null, new object[] { $"Internal error: {ex.Message}" });
                return (TResponse)failureResult!;
            }
            
            // For non-Result types, re-throw to be handled by global middleware
            throw;
        }
    }
}