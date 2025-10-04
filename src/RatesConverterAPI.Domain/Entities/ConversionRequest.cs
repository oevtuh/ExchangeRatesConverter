using System.ComponentModel.DataAnnotations;

namespace RatesConverterAPI.Domain.Entities;

/// <summary>
/// Entity representing a currency conversion request with exchange rate response
/// </summary>
public class ConversionRequest
{
    /// <summary>
    /// Unique identifier for the conversion request
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Source currency code (e.g., "USD")
    /// </summary>
    [MaxLength(3)]
    public required string FromCurrency { get; init; }
    
    /// <summary>
    /// Target currency code (e.g., "EUR")
    /// </summary>
    [MaxLength(3)]
    public required string ToCurrency { get; init; }
    
    /// <summary>
    /// Original amount requested for conversion
    /// </summary>
    public required decimal RequestedAmount { get; init; }
    
    /// <summary>
    /// Exchange rate used for the conversion
    /// </summary>
    public required decimal ExchangeRate { get; init; }
    
    /// <summary>
    /// Converted amount result
    /// </summary>
    public required decimal ConvertedAmount { get; init; }
    
    /// <summary>
    /// Source of the exchange rate (e.g., "Hardcoded", "ExternalAPI", "Cache")
    /// </summary>
    [MaxLength(50)]
    public required string RateSource { get; init; }
    
    /// <summary>
    /// When the conversion request was made
    /// </summary>
    public DateTime RequestedAt { get; init; } = DateTime.UtcNow;
    
    /// <summary>
    /// Client IP address for analytics
    /// </summary>
    [MaxLength(45)]
    public string? ClientIpAddress { get; init; }
    
    /// <summary>
    /// User agent for analytics
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; init; }
    
    /// <summary>
    /// Request correlation ID for tracing
    /// </summary>
    [MaxLength(100)]
    public string? CorrelationId { get; init; }
    
    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public int ProcessingTimeMs { get; init; }
    
    /// <summary>
    /// Whether the conversion was successful
    /// </summary>
    public bool IsSuccessful { get; init; } = true;
    
    /// <summary>
    /// Error message if conversion failed
    /// </summary>
    [MaxLength(1000)]
    public string? ErrorMessage { get; init; }
}