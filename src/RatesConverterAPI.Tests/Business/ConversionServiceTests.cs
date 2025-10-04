using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RatesConverterAPI.Business.Interfaces;
using RatesConverterAPI.Business.Models;
using RatesConverterAPI.Business.Services;
using RatesConverterAPI.Domain.Data;
using RatesConverterAPI.Domain.Entities;

namespace RatesConverterAPI.Tests.Business;

public class ConversionServiceTests : IDisposable
{
    private readonly RatesConverterDbContext _dbContext;
    private readonly Mock<IOpenExchangeService> _mockExchangeService;
    private readonly ConversionService _conversionService;

    public ConversionServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<RatesConverterDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _dbContext = new RatesConverterDbContext(options);
        _mockExchangeService = new Mock<IOpenExchangeService>();
        _conversionService = new ConversionService(_dbContext, _mockExchangeService.Object);

        // Seed test data
        SeedTestData();
    }

    [Fact]
    public async Task ConvertAsync_WithValidCurrencyPair_ShouldReturnSuccessfulResult()
    {
        // Arrange
        var request = new ConversionRequestDto(
            FromCurrency: "USD",
            ToCurrency: "EUR",
            Amount: 100m,
            ClientIpAddress: "127.0.0.1",
            UserAgent: "Test",
            CorrelationId: "test-123"
        );

        _mockExchangeService
            .Setup(x => x.GetExchangeRateAsync("USD", "EUR", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExchangeRateResult(
                IsSuccessful: true,
                Rate: 0.85m,
                Source: "Test",
                RetrievedAt: DateTime.UtcNow
            ));

        // Act
        var result = await _conversionService.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        result.ConvertedAmount.Should().Be(85m);
        result.ExchangeRate.Should().Be(0.85m);
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task ConvertAsync_WithUnsupportedCurrencyPair_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new ConversionRequestDto(
            FromCurrency: "XYZ",
            ToCurrency: "ABC",
            Amount: 100m,
            ClientIpAddress: "127.0.0.1",
            UserAgent: "Test",
            CorrelationId: "test-123"
        );

        // Act
        var result = await _conversionService.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeFalse();
        result.ConvertedAmount.Should().Be(0);
        result.ErrorMessage.Should().Contain("not supported");
    }

    [Fact]
    public async Task ConvertAsync_WithZeroAmount_ShouldReturnZeroConversion()
    {
        // Arrange
        var request = new ConversionRequestDto(
            FromCurrency: "USD",
            ToCurrency: "EUR",
            Amount: 0m,
            ClientIpAddress: "127.0.0.1",
            UserAgent: "Test",
            CorrelationId: "test-123"
        );

        _mockExchangeService
            .Setup(x => x.GetExchangeRateAsync("USD", "EUR", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExchangeRateResult(
                IsSuccessful: true,
                Rate: 0.85m,
                Source: "Test",
                RetrievedAt: DateTime.UtcNow
            ));

        // Act
        var result = await _conversionService.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        result.ConvertedAmount.Should().Be(0);
        result.ExchangeRate.Should().Be(0.85m);
    }

    [Fact]
    public async Task ConvertAsync_WhenExchangeServiceFails_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new ConversionRequestDto(
            FromCurrency: "USD",
            ToCurrency: "EUR",
            Amount: 100m,
            ClientIpAddress: "127.0.0.1",
            UserAgent: "Test",
            CorrelationId: "test-123"
        );

        _mockExchangeService
            .Setup(x => x.GetExchangeRateAsync("USD", "EUR", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExchangeRateResult(
                IsSuccessful: false,
                Rate: 0,
                Source: "Test",
                RetrievedAt: DateTime.UtcNow,
                ErrorMessage: "Service unavailable"
            ));

        // Act
        var result = await _conversionService.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Service unavailable");
    }

    [Fact]
    public async Task ConvertAsync_ShouldLogRequestToDatabase()
    {
        // Arrange
        var request = new ConversionRequestDto(
            FromCurrency: "USD",
            ToCurrency: "EUR",
            Amount: 100m,
            ClientIpAddress: "127.0.0.1",
            UserAgent: "Test",
            CorrelationId: "test-123"
        );

        _mockExchangeService
            .Setup(x => x.GetExchangeRateAsync("USD", "EUR", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExchangeRateResult(
                IsSuccessful: true,
                Rate: 0.85m,
                Source: "Test",
                RetrievedAt: DateTime.UtcNow
            ));

        var initialRequestCount = await _dbContext.ConversionRequests.CountAsync();

        // Act
        await _conversionService.ConvertAsync(request);

        // Assert
        var finalRequestCount = await _dbContext.ConversionRequests.CountAsync();
        finalRequestCount.Should().Be(initialRequestCount + 1);

        var loggedRequest = await _dbContext.ConversionRequests
            .OrderByDescending(r => r.RequestedAt)
            .FirstAsync();

        loggedRequest.FromCurrency.Should().Be("USD");
        loggedRequest.ToCurrency.Should().Be("EUR");
        loggedRequest.RequestedAmount.Should().Be(100m);
        loggedRequest.CorrelationId.Should().Be("test-123");
    }

    private void SeedTestData()
    {
        _dbContext.CurrencyPairs.AddRange(
            new CurrencyPair { FromCurrency = "USD", ToCurrency = "EUR", IsEnabled = true },
            new CurrencyPair { FromCurrency = "EUR", ToCurrency = "USD", IsEnabled = true },
            new CurrencyPair { FromCurrency = "USD", ToCurrency = "GBP", IsEnabled = true },
            new CurrencyPair { FromCurrency = "GBP", ToCurrency = "USD", IsEnabled = true }
        );
        _dbContext.SaveChanges();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}