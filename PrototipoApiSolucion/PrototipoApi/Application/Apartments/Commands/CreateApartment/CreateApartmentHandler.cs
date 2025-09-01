using MediatR;
using PrototipoApi.Models;
using PrototipoApi.Entities;
using PrototipoApi.Repositories.Interfaces;
using PrototipoApi.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace PrototipoApi.Application.Apartments.Commands.CreateApartment
{
    public class CreateApartmentHandler : IRequestHandler<CreateApartmentCommand, ApartmentDto>
    {
        private readonly IRepository<Apartment> _apartments;
        private readonly IRepository<PrototipoApi.Entities.Building> _buildings;
        private readonly ILoguer _loguer;
        public CreateApartmentHandler(IRepository<Apartment> apartments, IRepository<PrototipoApi.Entities.Building> buildings, ILoguer loguer)
        {
            _apartments = apartments;
            _buildings = buildings;
            _loguer = loguer;
        }
        public async Task<ApartmentDto> Handle(CreateApartmentCommand request, CancellationToken cancellationToken)
        {
            _loguer.LogInfo("Creando nuevo apartamento desde handler");
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
            _loguer.LogInfo($"Apartamento creado con id {entity.ApartmentId}");
            return dto;
        }
    }
}
