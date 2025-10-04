namespace RatesConverterAPI.Common.Extensions;

/// <summary>
/// Interface for building API endpoints
/// </summary>
public interface IEndpointBuilder
{
    /// <summary>
    /// Maps the endpoints to the route builder
    /// </summary>
    /// <param name="routeBuilder">The route builder</param>
    void Map(IEndpointRouteBuilder routeBuilder);
}