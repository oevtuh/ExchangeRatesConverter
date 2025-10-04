using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RatesConverterAPI.Domain.Data;

namespace RatesConverterAPI.Domain.Extensions;

/// <summary>
/// Extension methods for configuring Domain services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Domain services to the dependency injection container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Configured service collection</returns>
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        // Add Entity Framework
        services.AddDbContext<RatesConverterDbContext>((serviceProvider, options) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly("RatesConverterAPI");
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            });

            // Enable sensitive data logging in development
            if (configuration.GetValue<bool>("Logging:EnableSensitiveDataLogging"))
            {
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }
}