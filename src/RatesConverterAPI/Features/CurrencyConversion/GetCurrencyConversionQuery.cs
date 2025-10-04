using MediatR;
using RatesConverterAPI.Models;

namespace RatesConverterAPI.Features.CurrencyConversion;

/// <summary>
/// Query to perform currency conversion with validation and logging
/// </summary>
public record GetCurrencyConversionQuery(CurrencyConversionRequest Request) : IRequest<Result<CurrencyConversionResponse>>;