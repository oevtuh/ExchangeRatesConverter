using System.Text.Json.Serialization;

namespace RatesConverterAPI.Business.Models;

/// <summary>
/// Response model from OpenExchangeRates API
/// </summary>
public record OpenExchangeRatesResponse
{
    [JsonPropertyName("disclaimer")]
    public string? Disclaimer { get; init; }

    [JsonPropertyName("license")]
    public string? License { get; init; }

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; init; }

    [JsonPropertyName("base")]
    public string? Base { get; init; }

    [JsonPropertyName("rates")]
    public Dictionary<string, decimal> Rates { get; init; } = [];
}

/// <summary>
/// Configuration for OpenExchangeRates service
/// </summary>
public record OpenExchangeRatesConfig
{
    public string ApiKey { get; init; } = string.Empty;
    public string BaseUrl { get; init; } = "https://openexchangerates.org/api";
    public bool UseFakeService { get; init; } = true;
    public int TimeoutSeconds { get; init; } = 30;
}