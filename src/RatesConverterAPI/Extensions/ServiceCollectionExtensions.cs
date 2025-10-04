using RatesConverterAPI.Common.Extensions;
using RatesConverterAPI.Domain.Extensions;
using RatesConverterAPI.Business.Interfaces;
using RatesConverterAPI.Business.Services;
using RatesConverterAPI.Business.Models;
using RatesConverterAPI.Common.Behaviors;
using MediatR;

namespace RatesConverterAPI.Extensions;

/// <summary>
/// Extension methods for service collection configuration
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds application services to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>Configured service collection</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add MediatR with exception handling behavior
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
        });

        // Add Domain services and Entity Framework
        services.AddDomainServices();

        // Configure OpenExchangeRates settings
        services.Configure<OpenExchangeRatesConfig>(
            configuration.GetSection("OpenExchangeRates"));

        // Add HttpClient for OpenExchangeService
        services.AddHttpClient<OpenExchangeService>(client =>
        {
            var config = configuration.GetSection("OpenExchangeRates").Get<OpenExchangeRatesConfig>();
            client.Timeout = TimeSpan.FromSeconds(config?.TimeoutSeconds ?? 30);
        });

        // Register OpenExchange service based on configuration
        services.AddScoped<IOpenExchangeService>(provider =>
        {
            var config = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<OpenExchangeRatesConfig>>();
            
            if (config.Value.UseFakeService)
            {
                return provider.GetRequiredService<FakeOpenExchangeService>();
            }
            else
            {
                return provider.GetRequiredService<OpenExchangeService>();
            }
        });

        // Register both implementations
        services.AddScoped<FakeOpenExchangeService>();
        services.AddScoped<OpenExchangeService>();

        // Add Business services
        // Use factory to disambiguate constructors and prefer repository-based constructor
        services.AddScoped<IConversionService>(sp =>
        {
            var pairRepo = sp.GetRequiredService<RatesConverterAPI.Domain.Interfaces.IRepository<RatesConverterAPI.Domain.Entities.CurrencyPair>>();
            var requestRepo = sp.GetRequiredService<RatesConverterAPI.Domain.Interfaces.IRepository<RatesConverterAPI.Domain.Entities.ConversionRequest>>();
            var openExchange = sp.GetRequiredService<IOpenExchangeService>();
            return new ConversionService(pairRepo, requestRepo, openExchange);
        });

        // Add AutoMapper
        services.AddAutoMapper(typeof(Program).Assembly);

        // Add HTTP context accessor for request metadata
        services.AddHttpContextAccessor();

        // Configure ProblemDetails
        services.ConfigureProblemDetails();

        return services;
    }

    /// <summary>
    /// Configures ProblemDetails services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Configured service collection</returns>
    public static IServiceCollection ConfigureProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            // Customize the problem details transformation
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] = 
                    System.Diagnostics.Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
                context.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
            };
        });

        return services;
    }

    /// <summary>
    /// Adds Swagger/OpenAPI documentation
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Configured service collection</returns>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "RatesConverter API",
                Version = "v1",
                Description = "A modern .NET 8 API for currency rate conversion with MediatR and minimal APIs",
                Contact = new()
                {
                    Name = "API Support",
                    Email = "support@example.com"
                }
            });

            // Include XML comments if available
            var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}

/// <summary>
/// Extension methods for web application configuration
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Maps all feature endpoints automatically
    /// </summary>
    /// <param name="app">Web application</param>
    /// <returns>Configured web application</returns>
    public static WebApplication MapFeatureEndpoints(this WebApplication app)
    {
        // Automatically discover and register all endpoint builders
        var endpointBuilders = typeof(Program).Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IEndpointBuilder)) && !t.IsInterface && !t.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IEndpointBuilder>();

        foreach (var builder in endpointBuilders)
        {
            builder.Map(app);
        }

        return app;
    }
}