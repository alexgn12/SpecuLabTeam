using MediatR;
using PrototipoApi.Models;
using PrototipoApi.Entities;
using PrototipoApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PrototipoApi.Application.Building.Queries.GetBuildingById
{
    public class GetBuildingByIdHandler : IRequestHandler<GetBuildingByIdQuery, BuildingDto?>
    {
        private readonly IRepository<Entities.Building> _repository;
        public GetBuildingByIdHandler(IRepository<Entities.Building> repository)
        {
            _repository = repository;
        }
        public async Task<BuildingDto?> Handle(GetBuildingByIdQuery request, CancellationToken cancellationToken)
        {
            var dto = await _repository.SelectOneAsync(
                b => b.BuildingId == request.BuildingId,
                b => new BuildingDto
                {
                    BuildingId = b.BuildingId,
                    BuildingCode = b.BuildingCode,
                    BuildingName = b.BuildingName,
                    Street = b.Street,
                    District = b.District,
                    CreatedDate = b.CreatedDate,
                    FloorCount = b.FloorCount,
                    ApartmentCount = b.ApartmentCount,
                    YearBuilt = b.YearBuilt
                },
                cancellationToken
            );
            return dto;
        }
    }
}
