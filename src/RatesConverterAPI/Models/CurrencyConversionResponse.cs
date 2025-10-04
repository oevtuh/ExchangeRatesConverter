using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RatesConverterAPI.Models;

/// <summary>
/// Response model for currency conversion
/// </summary>
public record CurrencyConversionResponse
{
    /// <summary>
    /// Source currency code
    /// </summary>
    [JsonPropertyName("from_currency")]
    public required string FromCurrency { get; init; }
    
    /// <summary>
    /// Target currency code
    /// </summary>
    [JsonPropertyName("to_currency")]
    public required string ToCurrency { get; init; }
    
    /// <summary>
    /// Original amount before conversion
    /// </summary>
    [JsonPropertyName("requested_amount")]
    public required decimal RequestedAmount { get; init; }
    
    /// <summary>
    /// Converted amount
    /// </summary>
    [JsonPropertyName("converted_amount")]
    public required decimal ConvertedAmount { get; init; }
    
    /// <summary>
    /// Exchange rate used for conversion
    /// </summary>
    [JsonPropertyName("exchange_rate")]
    public required decimal ExchangeRate { get; init; }
    
    /// <summary>
    /// Source of the exchange rate
    /// </summary>
    [JsonPropertyName("rate_source")]
    public required string RateSource { get; init; }
    
    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    [JsonPropertyName("processing_time_ms")]
    public required int ProcessingTimeMs { get; init; }
    
    /// <summary>
    /// Timestamp when conversion was performed
    /// </summary>
    [JsonPropertyName("converted_at")]
    public DateTime ConvertedAt { get; init; } = DateTime.UtcNow;
    
    /// <summary>
    /// Whether the conversion was successful
    /// </summary>
    [JsonPropertyName("is_successful")]
    public required bool IsSuccessful { get; init; }
    
    /// <summary>
    /// Error message if conversion failed
    /// </summary>
    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; init; }
}