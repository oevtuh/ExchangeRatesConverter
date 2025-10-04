using System.ComponentModel.DataAnnotations;

namespace RatesConverterAPI.Models;

/// <summary>
/// Request model for currency conversion
/// </summary>
public class CurrencyConversionRequest
{
    /// <summary>
    /// Source currency code (3 characters, e.g., "USD")
    /// </summary>
    [Required]
    [StringLength(3, MinimumLength = 3)]
    [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Currency code must be 3 uppercase letters")]
    public required string FromCurrency { get; init; }

    /// <summary>
    /// Target currency code (3 characters, e.g., "EUR")
    /// </summary>
    [Required]
    [StringLength(3, MinimumLength = 3)]
    [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Currency code must be 3 uppercase letters")]
    public required string ToCurrency { get; init; }

    /// <summary>
    /// Amount to convert (must be positive)
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public required decimal Amount { get; init; }
}