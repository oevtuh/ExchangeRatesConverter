using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace RatesConverterAPI.Common.Extensions;

/// <summary>
/// Extension methods for creating ProblemDetails responses
/// </summary>
public static class ProblemDetailsExtensions
{
    /// <summary>
    /// Creates a ProblemDetails instance with common properties set
    /// </summary>
    public static ProblemDetails CreateProblemDetails(
        this HttpContext context,
        int statusCode,
        string? title = null,
        string? detail = null,
        string? type = null,
        string? instance = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title ?? GetDefaultTitle(statusCode),
            Detail = detail,
            Type = type ?? GetDefaultType(statusCode),
            Instance = instance ?? context.Request.Path.Value
        };

        // Add trace identifier for debugging
        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        return problemDetails;
    }

    /// <summary>
    /// Creates a validation ProblemDetails with error details
    /// </summary>
    public static ValidationProblemDetails CreateValidationProblemDetails(
        this HttpContext context,
        IDictionary<string, string[]> errors,
        string? title = null,
        string? detail = null,
        string? type = null,
        string? instance = null)
    {
        var problemDetails = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = title ?? "One or more validation errors occurred.",
            Detail = detail,
            Type = type ?? "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Instance = instance ?? context.Request.Path.Value
        };

        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        return problemDetails;
    }

    /// <summary>
    /// Converts Result to ProblemDetails HTTP response
    /// </summary>
    public static IResult ToProblemDetailsResult<T>(this Models.Result<T> result, HttpContext context)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        var problemDetails = context.CreateProblemDetails(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Bad Request",
            detail: result.Error
        );

        return Results.Problem(problemDetails);
    }

    /// <summary>
    /// Converts Result to ProblemDetails HTTP response
    /// </summary>
    public static IResult ToProblemDetailsResult(this Models.Result result, HttpContext context)
    {
        if (result.IsSuccess)
        {
            return Results.Ok();
        }

        var problemDetails = context.CreateProblemDetails(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Bad Request",
            detail: result.Error
        );

        return Results.Problem(problemDetails);
    }

    private static string GetDefaultTitle(int statusCode) => statusCode switch
    {
        400 => "Bad Request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        409 => "Conflict",
        422 => "Unprocessable Entity",
        500 => "Internal Server Error",
        502 => "Bad Gateway",
        503 => "Service Unavailable",
        _ => "An error occurred"
    };

    private static string GetDefaultType(int statusCode) => statusCode switch
    {
        400 => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        401 => "https://tools.ietf.org/html/rfc7235#section-3.1",
        403 => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
        404 => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        409 => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
        422 => "https://tools.ietf.org/html/rfc4918#section-11.2",
        500 => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        502 => "https://tools.ietf.org/html/rfc7231#section-6.6.3",
        503 => "https://tools.ietf.org/html/rfc7231#section-6.6.4",
        _ => "about:blank"
    };
}