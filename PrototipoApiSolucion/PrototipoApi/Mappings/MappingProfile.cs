using AutoMapper;
using PrototipoApi.Entities;
using PrototipoApi.Models;
using PrototipoApi.Services;

namespace PrototipoApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapear entidades a DTOs y viceversa
            CreateMap<Apartment, ApartmentDto>().ReverseMap();
            CreateMap<Request, RequestDto>().ReverseMap();
            CreateMap<CreateRequestDto, Request>();
            CreateMap<Transaction, TransactionDto>().ReverseMap();

            // Mapeo para datos externos de edificio
            CreateMap<ExternalBuildingDto, Building>()
                .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.buildingName))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.constructedAddress))
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.districtName))
                .ForMember(dest => dest.FloorCount, opt => opt.MapFrom(src => src.floorCount))
                .ForMember(dest => dest.YearBuilt, opt => opt.MapFrom(src => src.yearBuilt))
                .ForMember(dest => dest.ApartmentCount, opt => opt.MapFrom(src => src.apartmentCount))
                .ForMember(dest => dest.BuildingCode, opt => opt.Ignore()); // Se asigna manualmente
        }
    }
}
