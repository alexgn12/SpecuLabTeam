using MediatR;
using PrototipoApi.Entities;
using PrototipoApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using PrototipoApi.Models;

namespace PrototipoApi.Application.Requests.Commands.UpdateRequestStatus
{
    public class UpdateRequestStatusHandler : IRequestHandler<UpdateRequestStatusCommand, bool?>
    {
        private readonly IRepository<Request> _requests;
        private readonly IRepository<PrototipoApi.Entities.Status> _statuses;
        public UpdateRequestStatusHandler(IRepository<Request> requests, IRepository<PrototipoApi.Entities.Status> statuses)
        {
            _requests = requests;
            _statuses = statuses;
        }
        public async Task<bool?> Handle(UpdateRequestStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _requests.GetByIdAsync(request.RequestId);
            if (entity == null)
                return null;
            var status = await _statuses.GetOneAsync(s => s.StatusType == request.StatusType);
            if (status == null)
                return null;
            if (entity.StatusId == status.StatusId)
                return false; // Estado ya es el mismo
            int oldStatusId = entity.StatusId;
            entity.StatusId = status.StatusId;
            await _requests.UpdateAsync(entity, () =>
            {
                entity.StatusHistory.Add(new RequestStatusHistory
                {
                    RequestId = entity.RequestId,
                    OldStatusId = oldStatusId,
                    NewStatusId = status.StatusId,
                    ChangeDate = request.ChangeDate ?? DateTime.UtcNow,
                    Comment = string.IsNullOrWhiteSpace(request.Comment) ? "Cambio de estado v�a API por StatusType" : request.Comment
                });
            });
            await _requests.SaveChangesAsync();
            return true;
        }
    }
}
