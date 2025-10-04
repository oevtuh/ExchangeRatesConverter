using MediatR;
using Microsoft.AspNetCore.Mvc;
using RatesConverterAPI.Common.Extensions;
using RatesConverterAPI.Models;

namespace RatesConverterAPI.Features.CurrencyConversion;

internal sealed class EndpointBuilder : IEndpointBuilder
{
    public void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("/api/currency/convert", async (
            IMediator mediator,
            CurrencyConversionRequest request,
            CancellationToken cancellationToken
        ) =>
        {
            var query = new GetCurrencyConversionQuery(request);
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("ConvertCurrency")
        .WithSummary("Convert currency with validation and logging")
        .WithDescription("Converts an amount from one currency to another with database validation and request logging")
        .Produces<CurrencyConversionResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .Accepts<CurrencyConversionRequest>("application/json");

        routeBuilder.MapGet("/api/currency/supported", () =>
        {
            var supportedCurrencies = new[] { "USD", "EUR", "GBP", "JPY" };
            return Results.Ok(supportedCurrencies);
        })
        .WithName("GetSupportedCurrencies")
        .WithSummary("Get supported currencies")
        .WithDescription("Returns a list of supported currency codes")
        .Produces<string[]>(StatusCodes.Status200OK);
    }
}