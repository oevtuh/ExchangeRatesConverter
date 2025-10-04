using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using RatesConverterAPI.Business.Interfaces;
using RatesConverterAPI.Domain.Data;

namespace RatesConverterAPI.Business.Services;

/// <summary>
/// Fake implementation of OpenExchangeRates service for testing and development
/// Returns hardcoded rates only for valid currency pairs from the database
/// </summary>
public class FakeOpenExchangeService(
    RatesConverterDbContext dbContext,
    ILogger<FakeOpenExchangeService> logger) : IOpenExchangeService
{
    // Predefined fake exchange rates for testing (rates are relative to USD as base)
    private static readonly Dictionary<string, decimal> FakeRatesFromUsd = new()
    {
        ["EUR"] = 0.85m,
        ["GBP"] = 0.73m,
        ["JPY"] = 110.50m,
        ["CAD"] = 1.25m,
        ["AUD"] = 1.35m,
        ["CHF"] = 0.92m,
        ["CNY"] = 6.45m,
        ["USD"] = 1.00m
    };

    public async Task<ExchangeRateResult> GetExchangeRateAsync(string fromCurrency, string toCurrency, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Using fake exchange service for {FromCurrency} to {ToCurrency}", 
            fromCurrency, toCurrency);

        try
        {
            // Simulate some processing time
            await Task.Delay(100, cancellationToken);

            fromCurrency = fromCurrency.ToUpper();
            toCurrency = toCurrency.ToUpper();

            // Check if this currency pair exists in our database
            var pairExists = await dbContext.CurrencyPairs
                .AnyAsync(cp => cp.IsEnabled && 
                              cp.FromCurrency == fromCurrency && 
                              cp.ToCurrency == toCurrency, 
                         cancellationToken);

            if (!pairExists)
            {
                logger.LogWarning("Currency pair {FromCurrency}-{ToCurrency} is not available in database", 
                    fromCurrency, toCurrency);
                return new ExchangeRateResult(
                    IsSuccessful: false,
                    Rate: 0,
                    Source: "FakeService",
                    RetrievedAt: DateTime.UtcNow,
                    ErrorMessage: $"Currency pair {fromCurrency}-{toCurrency} is not supported"
                );
            }

            // Calculate exchange rate using USD as intermediate
            decimal exchangeRate = CalculateExchangeRate(fromCurrency, toCurrency);

            logger.LogInformation("Fake service returning rate: 1 {FromCurrency} = {Rate} {ToCurrency}", 
                fromCurrency, exchangeRate, toCurrency);

            return new ExchangeRateResult(
                IsSuccessful: true,
                Rate: exchangeRate,
                Source: "FakeService",
                RetrievedAt: DateTime.UtcNow
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in fake exchange service for {FromCurrency} to {ToCurrency}", 
                fromCurrency, toCurrency);
            return new ExchangeRateResult(
                IsSuccessful: false,
                Rate: 0,
                Source: "FakeService",
                RetrievedAt: DateTime.UtcNow,
                ErrorMessage: $"Internal error: {ex.Message}"
            );
        }
    }

    /// <summary>
    /// Calculate exchange rate between two currencies using USD as base
    /// </summary>
    private static decimal CalculateExchangeRate(string fromCurrency, string toCurrency)
    {
        // If we don't have predefined rates, generate deterministic fake rates
        if (!FakeRatesFromUsd.TryGetValue(fromCurrency, out var fromRate))
        {
            fromRate = GenerateFakeRate(fromCurrency);
        }

        if (!FakeRatesFromUsd.TryGetValue(toCurrency, out var toRate))
        {
            toRate = GenerateFakeRate(toCurrency);
        }

        // Calculate cross rate: FROM -> USD -> TO
        decimal exchangeRate;
        if (fromCurrency == "USD")
        {
            exchangeRate = toRate;
        }
        else if (toCurrency == "USD")
        {
            exchangeRate = 1 / fromRate;
        }
        else
        {
            exchangeRate = toRate / fromRate;
        }

        return Math.Round(exchangeRate, 6);
    }

    /// <summary>
    /// Generate a deterministic fake rate for currencies not in our predefined list
    /// </summary>
    private static decimal GenerateFakeRate(string currency)
    {
        var hash = Math.Abs(currency.GetHashCode());
        var rate = 0.1m + (hash % 500) / 100m; // Rate between 0.1 and 5.1
        return Math.Round(rate, 4);
    }
}