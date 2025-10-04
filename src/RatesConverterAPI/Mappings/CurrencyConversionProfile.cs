using AutoMapper;
using RatesConverterAPI.Business.Interfaces;
using RatesConverterAPI.Models;

namespace RatesConverterAPI.Mappings;

/// <summary>
/// AutoMapper profile for currency conversion mappings
/// </summary>
public class CurrencyConversionProfile : Profile
{
    public CurrencyConversionProfile()
    {
        // Map tuple (request + result) to full conversion response
        // This is the only mapping we actually use in the application
        CreateMap<(CurrencyConversionRequest request, ConversionResult result), CurrencyConversionResponse>()
            .ForMember(dest => dest.FromCurrency, opt => opt.MapFrom(src => src.request.FromCurrency))
            .ForMember(dest => dest.ToCurrency, opt => opt.MapFrom(src => src.request.ToCurrency))
            .ForMember(dest => dest.RequestedAmount, opt => opt.MapFrom(src => src.request.Amount))
            .ForMember(dest => dest.ConvertedAmount, opt => opt.MapFrom(src => src.result.ConvertedAmount))
            .ForMember(dest => dest.ExchangeRate, opt => opt.MapFrom(src => src.result.ExchangeRate))
            .ForMember(dest => dest.RateSource, opt => opt.MapFrom(src => src.result.RateSource))
            .ForMember(dest => dest.ProcessingTimeMs, opt => opt.MapFrom(src => src.result.ProcessingTimeMs))
            .ForMember(dest => dest.IsSuccessful, opt => opt.MapFrom(src => src.result.IsSuccessful))
            .ForMember(dest => dest.ErrorMessage, opt => opt.MapFrom(src => src.result.ErrorMessage))
            .ForMember(dest => dest.ConvertedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}