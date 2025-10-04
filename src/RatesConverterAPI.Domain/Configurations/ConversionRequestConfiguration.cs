using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RatesConverterAPI.Domain.Entities;

namespace RatesConverterAPI.Domain.Configurations;

/// <summary>
/// Entity Framework configuration for ConversionRequest entity
/// </summary>
public class ConversionRequestConfiguration : IEntityTypeConfiguration<ConversionRequest>
{
    public void Configure(EntityTypeBuilder<ConversionRequest> builder)
    {
        // Table configuration
        builder.ToTable("ConversionRequests");
        
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
        
        builder.Property(e => e.RequestedAmount)
            .HasColumnType("decimal(18,4)")
            .IsRequired();
        
        builder.Property(e => e.ExchangeRate)
            .HasColumnType("decimal(18,8)")
            .IsRequired();
        
        builder.Property(e => e.ConvertedAmount)
            .HasColumnType("decimal(18,4)")
            .IsRequired();
        
        builder.Property(e => e.RateSource)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(e => e.RequestedAt)
            .HasDefaultValueSql("NOW()")
            .IsRequired();
        
        builder.Property(e => e.ClientIpAddress)
            .HasMaxLength(45);
        
        builder.Property(e => e.UserAgent)
            .HasMaxLength(500);
        
        builder.Property(e => e.CorrelationId)
            .HasMaxLength(100);
        
        builder.Property(e => e.ProcessingTimeMs)
            .IsRequired();
        
        builder.Property(e => e.IsSuccessful)
            .HasDefaultValue(true)
            .IsRequired();
        
        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(1000);

        // Indexes for better query performance
        builder.HasIndex(e => e.RequestedAt)
            .HasDatabaseName("IX_ConversionRequests_RequestedAt");
        
        builder.HasIndex(e => new { e.FromCurrency, e.ToCurrency })
            .HasDatabaseName("IX_ConversionRequests_CurrencyPair");
        
        builder.HasIndex(e => e.IsSuccessful)
            .HasDatabaseName("IX_ConversionRequests_IsSuccessful");
        
        builder.HasIndex(e => e.RateSource)
            .HasDatabaseName("IX_ConversionRequests_RateSource");
        
        builder.HasIndex(e => e.CorrelationId)
            .HasDatabaseName("IX_ConversionRequests_CorrelationId");
        
        // Composite index for analytics queries
        builder.HasIndex(e => new { e.RequestedAt, e.FromCurrency, e.ToCurrency, e.IsSuccessful })
            .HasDatabaseName("IX_ConversionRequests_Analytics");
    }
}