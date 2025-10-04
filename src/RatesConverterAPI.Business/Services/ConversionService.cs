using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RatesConverterAPI.Business.Interfaces;
using RatesConverterAPI.Business.Models;
using RatesConverterAPI.Domain.Entities;
using RatesConverterAPI.Domain.Data;
using RatesConverterAPI.Domain.Interfaces;
using RatesConverterAPI.Domain.Repositories;
using System.Diagnostics;

namespace RatesConverterAPI.Business.Services;

/// <summary>
/// Service implementation for currency conversion operations
/// </summary>
public class ConversionService : IConversionService
{
    private readonly IRepository<CurrencyPair> _currencyPairRepository;
    private readonly IRepository<ConversionRequest> _conversionRequestRepository;
    private readonly IOpenExchangeService _openExchangeService;

    [ActivatorUtilitiesConstructor]
    public ConversionService(
        IRepository<CurrencyPair> currencyPairRepository,
        IRepository<ConversionRequest> conversionRequestRepository,
        IOpenExchangeService openExchangeService)
    {
        _currencyPairRepository = currencyPairRepository;
        _conversionRequestRepository = conversionRequestRepository;
        _openExchangeService = openExchangeService;
    }

    // Backward-compatible constructor: accept DbContext and adapt to repositories
    public ConversionService(RatesConverterDbContext dbContext, IOpenExchangeService openExchangeService)
        : this(new EfRepository<CurrencyPair>(dbContext), new EfRepository<ConversionRequest>(dbContext), openExchangeService)
    { }
    public async Task<ConversionResult> ConvertAsync(ConversionRequestDto request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            // 1. Validate if currency pair is supported
            var isPairSupported = await IsPairSupportedAsync(request.FromCurrency, request.ToCurrency, cancellationToken);
            if (!isPairSupported)
            {
                var errorResult = new ConversionResult(
                    IsSuccessful: false,
                    ConvertedAmount: 0,
                    ExchangeRate: 0,
                    RateSource: "Validation",
                    ProcessingTimeMs: (int)stopwatch.ElapsedMilliseconds,
                    ErrorMessage: $"Currency pair {request.FromCurrency}/{request.ToCurrency} is not supported"
                );

                // Log failed request
                await LogConversionAsync(request, errorResult, stopwatch.ElapsedMilliseconds, cancellationToken);
                return errorResult;
            }

            // 2. Get exchange rate using OpenExchange service
            var exchangeRateResult = await _openExchangeService.GetExchangeRateAsync(
                request.FromCurrency, 
                request.ToCurrency, 
                cancellationToken);

            if (!exchangeRateResult.IsSuccessful)
            {
                var errorResult = new ConversionResult(
                    IsSuccessful: false,
                    ConvertedAmount: 0,
                    ExchangeRate: 0,
                    RateSource: exchangeRateResult.Source,
                    ProcessingTimeMs: (int)stopwatch.ElapsedMilliseconds,
                    ErrorMessage: exchangeRateResult.ErrorMessage
                );

                // Log failed request
                await LogConversionAsync(request, errorResult, stopwatch.ElapsedMilliseconds, cancellationToken);
                return errorResult;
            }
            
            // 3. Calculate converted amount
            var convertedAmount = request.Amount * exchangeRateResult.Rate;

            stopwatch.Stop();

            var result = new ConversionResult(
                IsSuccessful: true,
                ConvertedAmount: Math.Round(convertedAmount, 4),
                ExchangeRate: exchangeRateResult.Rate,
                RateSource: exchangeRateResult.Source,
                ProcessingTimeMs: (int)stopwatch.ElapsedMilliseconds
            );

            // 4. Log successful request
            await LogConversionAsync(request, result, stopwatch.ElapsedMilliseconds, cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            var errorResult = new ConversionResult(
                IsSuccessful: false,
                ConvertedAmount: 0,
                ExchangeRate: 0,
                RateSource: "Error",
                ProcessingTimeMs: (int)stopwatch.ElapsedMilliseconds,
                ErrorMessage: ex.Message
            );

            // Log error
            await LogConversionAsync(request, errorResult, stopwatch.ElapsedMilliseconds, cancellationToken);
            return errorResult;
        }
    }

    public async Task<ConversionHistoryResult> GetHistoryAsync(ConversionHistoryQuery query, CancellationToken cancellationToken = default)
    {
    var dbQuery = _conversionRequestRepository.Query(asNoTracking: true);

        // Apply filters
        if (!string.IsNullOrEmpty(query.FromCurrency))
            dbQuery = dbQuery.Where(r => r.FromCurrency == query.FromCurrency);

        if (!string.IsNullOrEmpty(query.ToCurrency))
            dbQuery = dbQuery.Where(r => r.ToCurrency == query.ToCurrency);

        if (query.FromDate.HasValue)
            dbQuery = dbQuery.Where(r => r.RequestedAt >= query.FromDate.Value);

        if (query.ToDate.HasValue)
            dbQuery = dbQuery.Where(r => r.RequestedAt <= query.ToDate.Value);

        if (query.IsSuccessful.HasValue)
            dbQuery = dbQuery.Where(r => r.IsSuccessful == query.IsSuccessful.Value);

        // Get total count for pagination
        var totalCount = await dbQuery.CountAsync(cancellationToken);

        // Apply pagination and ordering
        var requests = await dbQuery
            .OrderByDescending(r => r.RequestedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

        return new ConversionHistoryResult(
            requests,
            totalCount,
            query.Page,
            query.PageSize,
            totalPages);
    }

    public async Task<ConversionStatistics> GetStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
    var query = _conversionRequestRepository.Query(asNoTracking: true);

        // Apply date filters
        if (fromDate.HasValue)
            query = query.Where(r => r.RequestedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(r => r.RequestedAt <= toDate.Value);

        var stats = await query
            .GroupBy(r => 1) // Group all records
            .Select(g => new
            {
                TotalRequests = g.Count(),
                SuccessfulRequests = g.Count(r => r.IsSuccessful),
                FailedRequests = g.Count(r => !r.IsSuccessful),
                AverageProcessingTime = g.Average(r => (double)r.ProcessingTimeMs),
                TotalVolume = g.Sum(r => r.RequestedAmount),
                FirstRequest = g.Min(r => r.RequestedAt),
                LastRequest = g.Max(r => r.RequestedAt)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (stats == null)
        {
            return new ConversionStatistics(0, 0, 0, 0, 0, 0, null, null);
        }

        var successRate = stats.TotalRequests > 0 
            ? (decimal)stats.SuccessfulRequests / stats.TotalRequests * 100 
            : 0;

        return new ConversionStatistics(
            stats.TotalRequests,
            stats.SuccessfulRequests,
            stats.FailedRequests,
            Math.Round(successRate, 2),
            Math.Round((decimal)stats.AverageProcessingTime, 2),
            stats.TotalVolume,
            stats.FirstRequest,
            stats.LastRequest
        );
    }

    public async Task<IEnumerable<CurrencyPairStatistic>> GetPopularPairsAsync(int limit = 10, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var query = _conversionRequestRepository.Query(asNoTracking: true)
            .Where(r => r.IsSuccessful);

        // Apply date filters
        if (fromDate.HasValue)
            query = query.Where(r => r.RequestedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(r => r.RequestedAt <= toDate.Value);

        return await query
            .GroupBy(r => new { r.FromCurrency, r.ToCurrency })
            .Select(g => new CurrencyPairStatistic(
                g.Key.FromCurrency,
                g.Key.ToCurrency,
                g.Count(),
                g.Sum(r => r.RequestedAmount),
                Math.Round(g.Average(r => r.ExchangeRate), 8)
            ))
            .OrderByDescending(s => s.RequestCount)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsPairSupportedAsync(string fromCurrency, string toCurrency, CancellationToken cancellationToken = default)
    {
        return await _currencyPairRepository.Query(asNoTracking: true)
            .AnyAsync(cp => cp.FromCurrency == fromCurrency.ToUpper() 
                         && cp.ToCurrency == toCurrency.ToUpper() 
                         && cp.IsEnabled, 
                      cancellationToken);
    }

    /// <summary>
    /// Log conversion request to database
    /// </summary>
    private async Task LogConversionAsync(ConversionRequestDto originalRequest, ConversionResult result, long processingTimeMs, CancellationToken cancellationToken)
    {
        var logEntry = new ConversionRequest
        {
            FromCurrency = originalRequest.FromCurrency.ToUpper(),
            ToCurrency = originalRequest.ToCurrency.ToUpper(),
            RequestedAmount = originalRequest.Amount,
            ExchangeRate = result.ExchangeRate,
            ConvertedAmount = result.ConvertedAmount,
            RateSource = result.RateSource,
            RequestedAt = DateTime.UtcNow,
            ClientIpAddress = originalRequest.ClientIpAddress,
            UserAgent = originalRequest.UserAgent,
            CorrelationId = originalRequest.CorrelationId,
            ProcessingTimeMs = (int)processingTimeMs,
            IsSuccessful = result.IsSuccessful,
            ErrorMessage = result.ErrorMessage
        };

        await _conversionRequestRepository.AddAsync(logEntry, cancellationToken);
        await _conversionRequestRepository.SaveChangesAsync(cancellationToken);
    }
}