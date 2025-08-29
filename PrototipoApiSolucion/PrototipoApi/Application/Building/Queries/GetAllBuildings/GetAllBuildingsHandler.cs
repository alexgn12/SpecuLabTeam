using MediatR;
using PrototipoApi.Entities;
using PrototipoApi.Models;
using PrototipoApi.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PrototipoApi.Application.Building.Queries.GetAllBuildings
{
    public class GetAllBuildingsHandler : IRequestHandler<GetAllBuildingsQuery, List<BuildingDto>>
    {
        private readonly IRepository<Entities.Building> _repository;

        public GetAllBuildingsHandler(IRepository<Entities.Building> repository)
        {
            _repository = repository;
        }

        public async Task<List<BuildingDto>> Handle(GetAllBuildingsQuery request, CancellationToken cancellationToken)
        {
            var buildings = await _repository.GetAllAsync();
            var filtered = buildings.AsQueryable();
            if (request.FloorCount > 0)
                filtered = filtered.Where(b => b.FloorCount == request.FloorCount);
            filtered = filtered.Skip((request.Page - 1) * request.Size).Take(request.Size);
            return filtered.Select(b => new BuildingDto
            {
                BuildingId = b.BuildingId,
                BuildingCode = b.BuildingCode,
                BuildingName = b.BuildingName,
                Street = b.Street,
                District = b.District,
                CreatedDate = b.CreatedDate,
                FloorCount = b.FloorCount,
                YearBuilt = b.YearBuilt,
                ApartmentCount = b.ApartmentCount // Añadido
            }).ToList();
        }
    }
}
