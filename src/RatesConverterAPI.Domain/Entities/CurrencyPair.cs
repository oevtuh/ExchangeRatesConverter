using System.ComponentModel.DataAnnotations;

namespace RatesConverterAPI.Domain.Entities;

/// <summary>
/// Entity representing available currency pairs for conversion validation
/// </summary>
public class CurrencyPair
{
    /// <summary>
    /// Unique identifier for the currency pair
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
    /// Whether this pair is currently enabled for conversions
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// When this configuration was created
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    /// <summary>
    /// Optional notes about this currency pair configuration
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }
}