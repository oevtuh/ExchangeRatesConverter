namespace RatesConverterAPI.Business.Interfaces;

/// <summary>
/// Service for retrieving exchange rates from external providers
/// </summary>
public interface IOpenExchangeService
{
    /// <summary>
    /// Get exchange rate between two currencies
    /// </summary>
    /// <param name="fromCurrency">Source currency code</param>
    /// <param name="toCurrency">Target currency code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Exchange rate result</returns>
    Task<ExchangeRateResult> GetExchangeRateAsync(string fromCurrency, string toCurrency, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of exchange rate lookup
/// </summary>
public record ExchangeRateResult(
    bool IsSuccessful,
    decimal Rate,
    string Source,
    DateTime RetrievedAt,
    string? ErrorMessage = null
);