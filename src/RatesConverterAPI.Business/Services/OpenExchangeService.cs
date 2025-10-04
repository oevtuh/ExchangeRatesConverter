using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RatesConverterAPI.Business.Interfaces;
using RatesConverterAPI.Business.Models;

namespace RatesConverterAPI.Business.Services;

/// <summary>
/// Real implementation of OpenExchangeRates service
/// </summary>
public class OpenExchangeService(
    HttpClient httpClient,
    IOptions<OpenExchangeRatesConfig> config,
    ILogger<OpenExchangeService> logger) : IOpenExchangeService
{
    private readonly OpenExchangeRatesConfig _config = config.Value;

    public async Task<ExchangeRateResult> GetExchangeRateAsync(string fromCurrency, string toCurrency, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Fetching exchange rate from {FromCurrency} to {ToCurrency} from OpenExchangeRates", 
                fromCurrency, toCurrency);

            // OpenExchangeRates uses USD as base currency, so we need to handle conversions
            var url = $"{_config.BaseUrl}/latest.json?app_id={_config.ApiKey}";
            
            logger.LogDebug("Making request to OpenExchangeRates: {Url}", url.Replace(_config.ApiKey, "***"));

            var response = await httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError("OpenExchangeRates API returned error: {StatusCode} - {Content}", 
                    response.StatusCode, errorContent);
                
                return new ExchangeRateResult(
                    IsSuccessful: false,
                    Rate: 0,
                    Source: "OpenExchangeRates",
                    RetrievedAt: DateTime.UtcNow,
                    ErrorMessage: $"API error: {response.StatusCode}"
                );
            }

            var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var exchangeData = JsonSerializer.Deserialize<OpenExchangeRatesResponse>(jsonContent);

            if (exchangeData?.Rates == null)
            {
                logger.LogError("Invalid response format from OpenExchangeRates");
                return new ExchangeRateResult(
                    IsSuccessful: false,
                    Rate: 0,
                    Source: "OpenExchangeRates",
                    RetrievedAt: DateTime.UtcNow,
                    ErrorMessage: "Invalid API response format"
                );
            }

            // Calculate the exchange rate
            var rate = CalculateExchangeRate(fromCurrency, toCurrency, exchangeData.Rates);

            if (rate == null)
            {
                logger.LogWarning("Currency pair {FromCurrency}/{ToCurrency} not supported by OpenExchangeRates", 
                    fromCurrency, toCurrency);
                
                return new ExchangeRateResult(
                    IsSuccessful: false,
                    Rate: 0,
                    Source: "OpenExchangeRates",
                    RetrievedAt: DateTime.UtcNow,
                    ErrorMessage: $"Currency pair {fromCurrency}/{toCurrency} not supported"
                );
            }

            logger.LogInformation("Successfully retrieved exchange rate: 1 {FromCurrency} = {Rate} {ToCurrency}", 
                fromCurrency, rate, toCurrency);

            return new ExchangeRateResult(
                IsSuccessful: true,
                Rate: rate.Value,
                Source: "OpenExchangeRates",
                RetrievedAt: DateTime.UtcNow
            );
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Network error while fetching exchange rates");
            return new ExchangeRateResult(
                IsSuccessful: false,
                Rate: 0,
                Source: "OpenExchangeRates",
                RetrievedAt: DateTime.UtcNow,
                ErrorMessage: $"Network error: {ex.Message}"
            );
        }
        catch (TaskCanceledException ex)
        {
            logger.LogError(ex, "Request timeout while fetching exchange rates");
            return new ExchangeRateResult(
                IsSuccessful: false,
                Rate: 0,
                Source: "OpenExchangeRates",
                RetrievedAt: DateTime.UtcNow,
                ErrorMessage: "Request timeout"
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while fetching exchange rates");
            return new ExchangeRateResult(
                IsSuccessful: false,
                Rate: 0,
                Source: "OpenExchangeRates",
                RetrievedAt: DateTime.UtcNow,
                ErrorMessage: $"Unexpected error: {ex.Message}"
            );
        }
    }

    /// <summary>
    /// Calculate exchange rate between two currencies using USD as base
    /// OpenExchangeRates provides rates with USD as base currency
    /// </summary>
    private static decimal? CalculateExchangeRate(string fromCurrency, string toCurrency, Dictionary<string, decimal> rates)
    {
        fromCurrency = fromCurrency.ToUpper();
        toCurrency = toCurrency.ToUpper();

        // If converting from USD
        if (fromCurrency == "USD")
        {
            return rates.TryGetValue(toCurrency, out var toRate) ? toRate : null;
        }

        // If converting to USD
        if (toCurrency == "USD")
        {
            return rates.TryGetValue(fromCurrency, out var fromRate) ? 1 / fromRate : null;
        }

        // If converting between two non-USD currencies
        if (rates.TryGetValue(fromCurrency, out var fromRateToUsd) && 
            rates.TryGetValue(toCurrency, out var toRateToUsd))
        {
            // Convert: FROM -> USD -> TO
            return toRateToUsd / fromRateToUsd;
        }

        return null;
    }
}