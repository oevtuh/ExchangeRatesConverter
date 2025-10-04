using Microsoft.EntityFrameworkCore;
using RatesConverterAPI.Domain.Entities;
using RatesConverterAPI.Domain.Configurations;

namespace RatesConverterAPI.Domain.Data;

/// <summary>
/// Database context for the RatesConverter API
/// </summary>
public class RatesConverterDbContext(DbContextOptions<RatesConverterDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Available currency pairs for conversion
    /// </summary>
    public DbSet<CurrencyPair> CurrencyPairs { get; set; } = null!;
    
    /// <summary>
    /// Currency conversion requests log
    /// </summary>
    public DbSet<ConversionRequest> ConversionRequests { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations from separate files
        modelBuilder.ApplyConfiguration(new CurrencyPairConfiguration());
        modelBuilder.ApplyConfiguration(new ConversionRequestConfiguration());
    }
}