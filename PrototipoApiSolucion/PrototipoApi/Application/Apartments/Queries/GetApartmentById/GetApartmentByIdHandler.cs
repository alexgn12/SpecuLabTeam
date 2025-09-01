using MediatR;
using PrototipoApi.Models;
using PrototipoApi.Entities;
using PrototipoApi.Repositories.Interfaces;
using PrototipoApi.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace PrototipoApi.Application.Apartments.Queries.GetApartmentById
{
    public class GetApartmentByIdHandler : IRequestHandler<GetApartmentByIdQuery, ApartmentDto?>
    {
        private readonly IRepository<Apartment> _apartments;
        private readonly ILoguer _loguer;
        public GetApartmentByIdHandler(IRepository<Apartment> apartments, ILoguer loguer)
        {
            _apartments = apartments;
            _loguer = loguer;
        }
        public async Task<ApartmentDto?> Handle(GetApartmentByIdQuery request, CancellationToken cancellationToken)
        {
            _loguer.LogInfo($"Handler: Obteniendo apartamento con id {request.ApartmentId}");
            var result = await _apartments.SelectOneAsync<ApartmentDto>(
                a => a.ApartmentId == request.ApartmentId,
                a => new ApartmentDto
                {
                    ApartmentId = a.ApartmentId,
                    ApartmentCode = a.ApartmentCode,
                    ApartmentDoor = a.ApartmentDoor,
                    ApartmentFloor = a.ApartmentFloor,
                    ApartmentPrice = a.ApartmentPrice,
                    NumberOfRooms = a.NumberOfRooms,
                    NumberOfBathrooms = a.NumberOfBathrooms,
                    BuildingCode = a.Building.BuildingCode,
                    HasLift = a.HasLift,
                    HasGarage = a.HasGarage,
                    CreatedDate = a.CreatedDate
                },
                cancellationToken
            );
            if (result == null)
                _loguer.LogWarning($"Handler: Apartamento con id {request.ApartmentId} no encontrado");
            return result;
        }
    }
}
