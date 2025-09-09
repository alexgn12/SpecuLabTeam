using MediatR;
using PrototipoApi.Models;
using PrototipoApi.Entities;
using PrototipoApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PrototipoApi.Application.Apartments.Queries.GetApartmentById
{
    public class GetApartmentByIdHandler : IRequestHandler<GetApartmentByIdQuery, ApartmentDto>
    {
        private readonly IRepository<Apartment> _apartments;
        private readonly ILogger<GetApartmentByIdHandler> _logger;
        public GetApartmentByIdHandler(IRepository<Apartment> apartments, ILogger<GetApartmentByIdHandler> logger)
        {
            _apartments = apartments;
            _logger = logger;
        }
        public async Task<ApartmentDto> Handle(GetApartmentByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Obteniendo apartamento con id {request.ApartmentId}");
                var apartment = await _apartments.GetByIdAsync(request.ApartmentId);
                if (apartment == null)
                {
                    _logger.LogWarning($"Apartamento con id {request.ApartmentId} no encontrado");
                    return null;
                }
                return new ApartmentDto
                {
                    ApartmentId = apartment.ApartmentId,
                    ApartmentCode = apartment.ApartmentCode,
                    ApartmentDoor = apartment.ApartmentDoor,
                    ApartmentFloor = apartment.ApartmentFloor,
                    ApartmentPrice = apartment.ApartmentPrice,
                    NumberOfRooms = apartment.NumberOfRooms,
                    NumberOfBathrooms = apartment.NumberOfBathrooms,
                    BuildingCode = apartment.Building.BuildingCode,
                    HasLift = apartment.HasLift,
                    HasGarage = apartment.HasGarage,
                    CreatedDate = apartment.CreatedDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetApartmentByIdHandler");
                throw;
            }
        }
    }
}
