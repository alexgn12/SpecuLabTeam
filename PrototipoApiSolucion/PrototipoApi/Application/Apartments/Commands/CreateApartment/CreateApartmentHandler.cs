using MediatR;
using PrototipoApi.Models;
using PrototipoApi.Entities;
using PrototipoApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PrototipoApi.Application.Apartments.Commands.CreateApartment
{
    public class CreateApartmentHandler : IRequestHandler<CreateApartmentCommand, ApartmentDto>
    {
        private readonly IRepository<Apartment> _apartments;
        private readonly IRepository<PrototipoApi.Entities.Building> _buildings;
        private readonly ILogger<CreateApartmentHandler> _logger;
        public CreateApartmentHandler(IRepository<Apartment> apartments, IRepository<PrototipoApi.Entities.Building> buildings, ILogger<CreateApartmentHandler> logger)
        {
            _apartments = apartments;
            _buildings = buildings;
            _logger = logger;
        }
        public async Task<ApartmentDto> Handle(CreateApartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creando nuevo apartamento desde handler");
                var dto = request.Dto;
                var building = await _buildings.GetOneAsync(b => b.BuildingCode == dto.BuildingCode);
                if (building == null)
                    throw new System.Exception($"No se encontró el edificio con código {dto.BuildingCode}");
                var entity = new Apartment
                {
                    ApartmentCode = dto.ApartmentCode,
                    ApartmentDoor = dto.ApartmentDoor,
                    ApartmentFloor = dto.ApartmentFloor,
                    ApartmentPrice = dto.ApartmentPrice,
                    NumberOfRooms = dto.NumberOfRooms,
                    NumberOfBathrooms = dto.NumberOfBathrooms,
                    BuildingId = building.BuildingId,
                    HasLift = dto.HasLift,
                    HasGarage = dto.HasGarage,
                    CreatedDate = dto.CreatedDate
                };
                await _apartments.AddAsync(entity);
                await _apartments.SaveChangesAsync();
                dto.ApartmentId = entity.ApartmentId;
                _logger.LogInformation($"Apartamento creado con id {entity.ApartmentId}");
                return dto;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error en CreateApartmentHandler");
                throw;
            }
        }
    }
}
