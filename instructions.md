# RatesConverterAPI - Development Instructions

## Project Overview
This is a .NET 8 Web API project for currency rate conversion services. Follow these guidelines for consistent, modern C# development.

## Code Style & Conventions

### Modern C# Features (C# 12+)
- Use **primary constructors** for dependency injection
- Leverage **collection expressions** `[]` instead of `new List<T>()`
- Use **required properties** for mandatory fields
- Implement **record types** for DTOs and value objects
- Use **file-scoped namespaces** throughout the project
- Prefer **implicit usings** and global using statements
- Use **nullable reference types** enabled by default

### Naming Conventions
- Use **PascalCase** for classes, methods, properties, and public fields
- Use **camelCase** for private fields, local variables, and parameters
- Use **kebab-case** for API endpoints (`/api/rates/convert`)
- Prefix interfaces with `I` (e.g., `IRateService`)
- Use descriptive names: `ConvertCurrencyAsync` not `ConvertAsync`

### Project Structure
```
RatesConverterAPI/
├── Controllers/          # API controllers
├── Services/            # Business logic services
├── Models/              # Domain models and DTOs
├── Data/                # Entity Framework context and entities
├── Middleware/          # Custom middleware
├── Extensions/          # Extension methods
├── Configuration/       # Configuration classes
└── Tests/              # Unit and integration tests
```

## API Development Patterns

### Controllers
- Use **minimal APIs** for simple endpoints
- Use **controller-based APIs** for complex scenarios
- Always use **async/await** for I/O operations
- Implement proper **HTTP status codes**
- Use **ActionResult<T>** return types
- Add **XML documentation** for all public methods

```csharp
/// <summary>
/// Converts currency from one type to another
/// </summary>
/// <param name="request">Currency conversion request</param>
/// <returns>Converted currency amount</returns>
[HttpPost("convert")]
public async Task<ActionResult<CurrencyConversionResponse>> ConvertCurrencyAsync(
    [FromBody] CurrencyConversionRequest request)
{
    var result = await _rateService.ConvertAsync(request);
    return Ok(result);
}
```

### DTOs and Models
- Use **record types** for immutable data transfer objects
- Use **required properties** for mandatory fields
- Implement **validation attributes**
- Use **JsonPropertyName** for API contract consistency

```csharp
public record CurrencyConversionRequest
{
    [Required]
    [JsonPropertyName("from_currency")]
    public required string FromCurrency { get; init; }
    
    [Required]
    [JsonPropertyName("to_currency")]
    public required string ToCurrency { get; init; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public required decimal Amount { get; init; }
}
```

### Services
- Use **primary constructors** for dependency injection
- Implement **interfaces** for all services
- Use **CancellationToken** in async methods
- Return **Result<T>** or **Option<T>** patterns for error handling
- Use **ILogger<T>** for structured logging

```csharp
public interface IRateService
{
    Task<Result<CurrencyConversionResponse>> ConvertAsync(
        CurrencyConversionRequest request, 
        CancellationToken cancellationToken = default);
}

public class RateService(IHttpClientFactory httpClientFactory, ILogger<RateService> logger) : IRateService
{
    public async Task<Result<CurrencyConversionResponse>> ConvertAsync(
        CurrencyConversionRequest request, 
        CancellationToken cancellationToken = default)
    {
        // Implementation here
    }
}
```

## Error Handling

### Global Exception Handling
- Implement **global exception middleware**
- Use **ProblemDetails** for error responses
- Log exceptions with structured logging
- Never expose internal errors to clients

### Result Pattern
```csharp
public record Result<T>
{
    public T? Value { get; init; }
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }
    
    public static Result<T> Success(T value) => new() { Value = value, IsSuccess = true };
    public static Result<T> Failure(string error) => new() { Error = error, IsSuccess = false };
}
```

## Data Access

### Entity Framework Core
- Use **Entity Framework Core 8+**
- Implement **repository pattern** if needed
- Use **migrations** for database changes
- Enable **nullable reference types**
- Use **value converters** for custom types

```csharp
public class Rate
{
    public required int Id { get; init; }
    public required string FromCurrency { get; init; }
    public required string ToCurrency { get; init; }
    public required decimal ExchangeRate { get; init; }
    public required DateTime LastUpdated { get; init; }
}
```

## Performance & Best Practices

### HTTP Clients
- Use **IHttpClientFactory** for HTTP requests
- Configure **Polly** for retry policies
- Implement **circuit breaker** pattern
- Use **named or typed clients**

### Caching
- Use **IMemoryCache** for in-memory caching
- Implement **Redis** for distributed caching
- Use **cache-aside** pattern
- Set appropriate **expiration policies**

### Validation
- Use **FluentValidation** for complex validation rules
- Implement **custom validation attributes**
- Validate at **model binding** level
- Return **ValidationProblemDetails** for validation errors

## Configuration

### Settings
- Use **IOptions<T>** pattern for configuration
- Implement **configuration validation**
- Use **user secrets** for development
- Use **Azure Key Vault** for production secrets

```csharp
public class RateServiceOptions
{
    public const string SectionName = "RateService";
    
    [Required]
    public required string ApiKey { get; init; }
    
    [Required]
    public required string BaseUrl { get; init; }
    
    [Range(1, 3600)]
    public int CacheExpirationSeconds { get; init; } = 300;
}
```

## Testing

### Unit Tests
- Use **xUnit** as testing framework
- Use **Moq** or **NSubstitute** for mocking
- Use **FluentAssertions** for assertions
- Follow **AAA pattern** (Arrange, Act, Assert)
- Use **Theory** and **InlineData** for parameterized tests

### Integration Tests
- Use **WebApplicationFactory<T>** for API testing
- Use **TestContainers** for database testing
- Implement **test fixtures** for shared setup
- Use **test-specific configuration**

## Security

### Authentication & Authorization
- Implement **JWT Bearer** authentication
- Use **role-based** or **policy-based** authorization
- Validate **input parameters** thoroughly
- Implement **rate limiting**

### API Security
- Use **HTTPS** only
- Implement **CORS** policy
- Add **security headers**
- Use **API versioning**
- Implement **request/response logging**

## Documentation

### OpenAPI/Swagger
- Configure **Swagger/OpenAPI** documentation
- Use **XML comments** for endpoint documentation
- Implement **example values** for requests/responses
- Group endpoints by **tags**

### Code Documentation
- Document **public APIs** with XML comments
- Use **meaningful parameter names**
- Document **business rules** and **constraints**
- Include **usage examples** in comments

## Deployment

### Docker
- Use **multi-stage builds**
- Use **Alpine Linux** base images
- Implement **health checks**
- Configure **environment variables**

### Monitoring
- Use **Application Insights** or **Serilog**
- Implement **structured logging**
- Add **custom metrics**
- Monitor **performance counters**

## Code Quality

### Static Analysis
- Enable **nullable reference types**
- Use **StyleCop** for code style
- Configure **EditorConfig**
- Use **SonarQube** for quality analysis

### Performance
- Use **async/await** consistently
- Implement **object pooling** where appropriate
- Use **Span<T>** and **Memory<T>** for high-performance scenarios
- Profile with **dotnet-trace** and **PerfView**

---

## GitHub Copilot Specific Instructions

When generating code for this project, please:
1. Always use the latest C# 12+ features and syntax
2. Follow the established patterns and conventions above
3. Include proper error handling and validation
4. Add appropriate logging and documentation
5. Use dependency injection and async patterns
6. Implement proper testing patterns
7. Follow REST API best practices
8. Use modern .NET 8 features and libraries
9. Prioritize performance and security
10. Generate comprehensive examples with realistic data

Remember: This is a production-ready API project. Generate code that is maintainable, testable, and follows enterprise-level standards.