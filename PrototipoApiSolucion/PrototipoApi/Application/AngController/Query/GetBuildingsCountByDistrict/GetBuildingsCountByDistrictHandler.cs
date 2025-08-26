using MediatR;
using PrototipoApi.Repositories.Interfaces;
using PrototipoApi.Entities;
using PrototipoApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PrototipoApi.Application.AngController.Query.GetBuildingsCountByDistrict
{
    public class GetBuildingsCountByDistrictHandler : IRequestHandler<GetBuildingsCountByDistrictQuery, List<BuildingsCountByDistrictDto>>
    {
        private readonly IRepository<Request> _requests;

        public GetBuildingsCountByDistrictHandler(IRepository<Request> requests)
        {
            _requests = requests;
        }

        public async Task<List<BuildingsCountByDistrictDto>> Handle(GetBuildingsCountByDistrictQuery request, CancellationToken cancellationToken)
        {
            // Selecciona el distrito de cada request
            var districts = await _requests.SelectListAsync(
                selector: r => r.Building.District,
                filter: null,
                orderBy: null,
                skip: null,
                take: null,
                ct: cancellationToken
            );

            var grouped = districts
                .GroupBy(d => d)
                .Select(g => new BuildingsCountByDistrictDto
                {
                    District = g.Key,
                    Count = g.Count()
                })
                .ToList();

            return grouped;
        }
    }
}
