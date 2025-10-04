namespace RatesConverterAPI.Business.Models;

/// <summary>
/// Conversion statistics model
/// </summary>
public record ConversionStatistics(
    int TotalRequests,
    int SuccessfulRequests,
    int FailedRequests,
    decimal SuccessRate,
    decimal AverageProcessingTimeMs,
    decimal TotalVolumeConverted,
    DateTime? FirstRequestDate,
    DateTime? LastRequestDate
);

/// <summary>
/// Currency pair statistics model
/// </summary>
public record CurrencyPairStatistic(
    string FromCurrency,
    string ToCurrency,
    int RequestCount,
    decimal TotalVolume,
    decimal AverageExchangeRate
);