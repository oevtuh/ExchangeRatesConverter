namespace RatesConverterAPI.Common.Exceptions;

/// <summary>
/// Base class for business logic exceptions
/// </summary>
public abstract class BusinessException : Exception
{
    protected BusinessException(string message) : base(message) { }
    protected BusinessException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a resource is not found
/// </summary>
public class NotFoundException : BusinessException
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string resourceName, object key) 
        : base($"{resourceName} with key '{key}' was not found.") { }
}

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationException : BusinessException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors) 
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}

/// <summary>
/// Exception thrown when a business rule is violated
/// </summary>
public class BusinessRuleException : BusinessException
{
    public BusinessRuleException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when an external service is unavailable
/// </summary>
public class ExternalServiceException : BusinessException
{
    public string ServiceName { get; }

    public ExternalServiceException(string serviceName, string message) : base(message)
    {
        ServiceName = serviceName;
    }

    public ExternalServiceException(string serviceName, string message, Exception innerException) 
        : base(message, innerException)
    {
        ServiceName = serviceName;
    }
}