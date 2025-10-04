using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RatesConverterAPI.Domain.Entities;

namespace RatesConverterAPI.Domain.Configurations;

/// <summary>
/// Entity Framework configuration for CurrencyPair entity
/// </summary>
public class CurrencyPairConfiguration : IEntityTypeConfiguration<CurrencyPair>
{
    public void Configure(EntityTypeBuilder<CurrencyPair> builder)
    {
        // Table configuration
        builder.ToTable("CurrencyPairs");
        
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Properties configuration
        builder.Property(e => e.Id)
            .UseIdentityColumn();
        
        builder.Property(e => e.FromCurrency)
            .HasMaxLength(3)
            .IsRequired();
        
        builder.Property(e => e.ToCurrency)
            .HasMaxLength(3)
            .IsRequired();
        
        builder.Property(e => e.IsEnabled)
            .HasDefaultValue(true)
            .IsRequired();
        
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("NOW()")
            .IsRequired();
        
        builder.Property(e => e.Notes)
            .HasMaxLength(500);

        // Indexes for better query performance
        builder.HasIndex(e => new { e.FromCurrency, e.ToCurrency })
            .IsUnique()
            .HasDatabaseName("IX_CurrencyPairs_FromTo_Unique");
        
        builder.HasIndex(e => e.IsEnabled)
            .HasDatabaseName("IX_CurrencyPairs_IsEnabled");

        // Seed data for available currency pairs (configuration only)
        builder.HasData(
            new CurrencyPair { Id = 1, FromCurrency = "USD", ToCurrency = "EUR", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new CurrencyPair { Id = 2, FromCurrency = "EUR", ToCurrency = "USD", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new CurrencyPair { Id = 3, FromCurrency = "USD", ToCurrency = "GBP", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new CurrencyPair { Id = 4, FromCurrency = "GBP", ToCurrency = "USD", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new CurrencyPair { Id = 5, FromCurrency = "EUR", ToCurrency = "GBP", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new CurrencyPair { Id = 6, FromCurrency = "GBP", ToCurrency = "EUR", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new CurrencyPair { Id = 7, FromCurrency = "USD", ToCurrency = "JPY", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new CurrencyPair { Id = 8, FromCurrency = "JPY", ToCurrency = "USD", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}