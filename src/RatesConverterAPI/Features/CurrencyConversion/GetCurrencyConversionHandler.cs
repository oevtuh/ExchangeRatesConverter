using AutoMapper;
using MediatR;
using RatesConverterAPI.Business.Interfaces;
using RatesConverterAPI.Business.Models;
using RatesConverterAPI.Models;

namespace RatesConverterAPI.Features.CurrencyConversion;

/// <summary>
/// Handler for currency conversion requests
/// </summary>
public class GetCurrencyConversionHandler(
    IConversionService conversionService,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor,
    ILogger<GetCurrencyConversionHandler> logger) : IRequestHandler<GetCurrencyConversionQuery, Result<CurrencyConversionResponse>>
{
    public async Task<Result<CurrencyConversionResponse>> Handle(GetCurrencyConversionQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing currency conversion from {FromCurrency} to {ToCurrency} for amount {Amount}",
            request.Request.FromCurrency, request.Request.ToCurrency, request.Request.Amount);

        // Get HTTP context for logging metadata
        var httpContext = httpContextAccessor.HttpContext;
        var clientIp = httpContext?.Connection?.RemoteIpAddress?.ToString();
        var userAgent = httpContext?.Request?.Headers.UserAgent.ToString();
        var correlationId = httpContext?.TraceIdentifier;

        // Create DTO with request data and metadata
        var conversionRequest = new ConversionRequestDto(
            FromCurrency: request.Request.FromCurrency,
            ToCurrency: request.Request.ToCurrency,
            Amount: request.Request.Amount,
            ClientIpAddress: clientIp,
            UserAgent: userAgent,
            CorrelationId: correlationId
        );

        // Perform conversion with validation and logging
        var result = await conversionService.ConvertAsync(conversionRequest, cancellationToken);

        // Map result to response using AutoMapper
        var response = mapper.Map<CurrencyConversionResponse>((request.Request, result));

        if (result.IsSuccessful)
        {
            logger.LogInformation("Successfully converted {Amount} {FromCurrency} to {ConvertedAmount} {ToCurrency}",
                request.Request.Amount, request.Request.FromCurrency, result.ConvertedAmount, request.Request.ToCurrency);
            
            return Result<CurrencyConversionResponse>.Success(response);
        }
        else
        {
            logger.LogWarning("Currency conversion failed: {ErrorMessage}", result.ErrorMessage);
            return Result<CurrencyConversionResponse>.Failure(result.ErrorMessage ?? "Unknown error occurred");
        }
    }
}