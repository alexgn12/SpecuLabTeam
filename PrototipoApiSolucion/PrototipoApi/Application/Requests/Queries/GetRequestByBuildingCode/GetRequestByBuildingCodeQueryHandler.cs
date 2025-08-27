using MediatR;
using PrototipoApi.Entities;
using PrototipoApi.Models;
using PrototipoApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace PrototipoApi.Application.Requests.Queries.GetRequestByBuildingCode
{
    public class GetRequestByBuildingCodeQueryHandler : IRequestHandler<GetRequestByBuildingCodeQuery, RequestDto?>
    {
        private readonly IRepository<Request> _requests;
        private readonly IRepository<PrototipoApi.Entities.Building> _buildings;

        public GetRequestByBuildingCodeQueryHandler(IRepository<Request> requests, IRepository<PrototipoApi.Entities.Building> buildings)
        {
            _requests = requests;
            _buildings = buildings;
        }

        public async Task<RequestDto?> Handle(GetRequestByBuildingCodeQuery request, CancellationToken cancellationToken)
        {
            // Buscar el BuildingId por BuildingCode
            var building = await _buildings.GetOneAsync(b => b.BuildingCode == request.BuildingCode);
            if (building == null)
                return null;
            // Buscar la última request asociada a ese BuildingId
            var allRequests = await _requests.GetAllAsync(null, r => r.Status);
            var req = allRequests
                .Where(r => r.BuildingId == building.BuildingId)
                .OrderByDescending(r => r.RequestDate)
                .FirstOrDefault();
            if (req == null)
                return null;
            return new RequestDto
            {
                RequestId = req.RequestId,
                BuildingAmount = req.BuildingAmount,
                MaintenanceAmount = req.MaintenanceAmount,
                Description = req.Description,
                StatusId = req.StatusId,
                StatusType = req.Status.StatusType,
                BuildingId = req.BuildingId
            };
        }
    }
}
