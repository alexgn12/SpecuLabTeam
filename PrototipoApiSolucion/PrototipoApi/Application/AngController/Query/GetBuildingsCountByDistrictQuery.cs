using MediatR;
using PrototipoApi.Repositories.Interfaces;
using PrototipoApi.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PrototipoApi.Application.AngController.Query
{
    public class GetBuildingsCountByDistrictQuery : IRequest<List<GetBuildingsCountByDistrictQuery.BuildingsCountByDistrictDto>>
    {
        public class BuildingsCountByDistrictDto
        {
            public string District { get; set; } = string.Empty;
            public int Count { get; set; }
        }

        public class Handler : IRequestHandler<GetBuildingsCountByDistrictQuery, List<BuildingsCountByDistrictDto>>
        {
            private readonly IRepository<Request> _requests;
            public Handler(IRepository<Request> requests)
            {
                _requests = requests;
            }

            public async Task<List<BuildingsCountByDistrictDto>> Handle(GetBuildingsCountByDistrictQuery request, CancellationToken cancellationToken)
            {
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
}
