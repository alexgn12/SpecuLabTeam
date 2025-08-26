using AutoMapper;
using PrototipoApi.Entities;
using PrototipoApi.Models;

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
        }
    }
}
