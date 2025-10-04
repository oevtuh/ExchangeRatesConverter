namespace RatesConverterAPI.Business.Models;

/// <summary>
/// Request model for currency conversion business logic
/// </summary>
public record ConversionRequestDto(
    string FromCurrency,
    string ToCurrency,
    decimal Amount,
    string? ClientIpAddress = null,
    string? UserAgent = null,
    string? CorrelationId = null
);