using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RatesConverterAPI.Common.Extensions;
using RatesConverterAPI.Common.Exceptions;

namespace RatesConverterAPI.Common.Middleware;

/// <summary>
/// Global exception handling middleware to catch and handle all unhandled exceptions
/// </summary>
public class GlobalExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred while processing request {TraceIdentifier}", 
                context.TraceIdentifier);
            
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var (statusCode, title, detail) = exception switch
        {
            ValidationException validationEx => (HttpStatusCode.BadRequest, "Validation Failed", validationEx.Message),
            NotFoundException notFoundEx => (HttpStatusCode.NotFound, "Resource Not Found", notFoundEx.Message),
            BusinessRuleException businessEx => (HttpStatusCode.BadRequest, "Business Rule Violation", businessEx.Message),
            ExternalServiceException serviceEx => (HttpStatusCode.ServiceUnavailable, "External Service Error", $"Service '{serviceEx.ServiceName}' is unavailable: {serviceEx.Message}"),
            ArgumentNullException => (HttpStatusCode.BadRequest, "Bad Request", "Required parameter is missing"),
            ArgumentException => (HttpStatusCode.BadRequest, "Bad Request", exception.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, "Bad Request", exception.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized", "Unauthorized access"),
            NotImplementedException => (HttpStatusCode.NotImplemented, "Not Implemented", "Feature not implemented"),
            TimeoutException => (HttpStatusCode.RequestTimeout, "Request Timeout", "Request timeout"),
            _ => (HttpStatusCode.InternalServerError, "Internal Server Error", "An internal server error occurred")
        };

        context.Response.StatusCode = (int)statusCode;

        ProblemDetails problemDetails;

        // Handle validation exceptions with detailed error information
        if (exception is ValidationException validationException && validationException.Errors.Any())
        {
            var validationProblemDetails = context.CreateValidationProblemDetails(
                errors: validationException.Errors,
                title: title,
                detail: detail
            );
            problemDetails = validationProblemDetails;
        }
        else
        {
            problemDetails = context.CreateProblemDetails(
                statusCode: (int)statusCode,
                title: title,
                detail: detail
            );
        }

        var jsonResponse = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}