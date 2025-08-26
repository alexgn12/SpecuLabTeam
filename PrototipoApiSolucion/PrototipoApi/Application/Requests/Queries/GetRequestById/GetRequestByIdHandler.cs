using MediatR;
using PrototipoApi.Models;
using PrototipoApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PrototipoApi.Application.Requests.Queries.GetRequestById
{
    public class GetRequestByIdHandler : IRequestHandler<GetRequestByIdQuery, RequestDto?>
    {
        private readonly IRepository<Entities.Request> _repository;

        public GetRequestByIdHandler(IRepository<Entities.Request> repository)
        {
            _repository = repository;
        }

        public async Task<RequestDto?> Handle(GetRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var dto = await _repository.SelectOneAsync(
                r => r.RequestId == request.id,
                r => new RequestDto
                {
                    RequestId = r.RequestId,
                    BuildingAmount = r.BuildingAmount,
                    MaintenanceAmount = r.MaintenanceAmount,
                    Description = r.Description,
                    StatusId = r.StatusId,
                    StatusType = r.Status.StatusType,
                    BuildingId = r.BuildingId
                },
                cancellationToken
            );
            return dto;
        }
    }
}