using RatesConverterAPI.Business.Models;
using RatesConverterAPI.Domain.Entities;

namespace RatesConverterAPI.Business.Interfaces;

/// <summary>
/// Service interface for currency conversion operations
/// </summary>
public interface IConversionService
{
    /// <summary>
    /// Perform currency conversion with logging
    /// </summary>
    Task<ConversionResult> ConvertAsync(ConversionRequestDto request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get conversion history with filtering and pagination
    /// </summary>
    Task<ConversionHistoryResult> GetHistoryAsync(ConversionHistoryQuery query, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get conversion statistics
    /// </summary>
    Task<ConversionStatistics> GetStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get popular currency pairs
    /// </summary>
    Task<IEnumerable<CurrencyPairStatistic>> GetPopularPairsAsync(int limit = 10, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validate if currency pair is supported
    /// </summary>
    Task<bool> IsPairSupportedAsync(string fromCurrency, string toCurrency, CancellationToken cancellationToken = default);
}

/// <summary>
/// Conversion result model
/// </summary>
public record ConversionResult(
    bool IsSuccessful,
    decimal ConvertedAmount,
    decimal ExchangeRate,
    string RateSource,
    int ProcessingTimeMs,
    string? ErrorMessage = null
);

/// <summary>
/// Conversion history query parameters
/// </summary>
public record ConversionHistoryQuery(
    int Page = 1,
    int PageSize = 20,
    string? FromCurrency = null,
    string? ToCurrency = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    bool? IsSuccessful = null
);

/// <summary>
/// Conversion history result with pagination
/// </summary>
public record ConversionHistoryResult(
    IEnumerable<ConversionRequest> Requests,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);