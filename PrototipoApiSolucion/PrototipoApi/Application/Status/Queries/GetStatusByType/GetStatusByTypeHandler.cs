using MediatR;
using PrototipoApi.Models;
using PrototipoApi.Entities;
using PrototipoApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PrototipoApi.Application.Status.Queries.GetStatusByType
{
    public class GetStatusByTypeHandler : IRequestHandler<GetStatusByTypeQuery, StatusDto?>
    {
        private readonly IRepository<PrototipoApi.Entities.Status> _repository;

        public GetStatusByTypeHandler(IRepository<PrototipoApi.Entities.Status> repository)
        {
            _repository = repository;
        }

        public async Task<StatusDto?> Handle(GetStatusByTypeQuery request, CancellationToken cancellationToken)
        {
            var status = await _repository.GetOneAsync(s => s.StatusType == request.StatusType);
            if (status == null)
                return null;
            return new StatusDto
            {
                StatusId = status.StatusId,
                StatusType = status.StatusType,
                Description = status.Description
            };
        }
    }
}
