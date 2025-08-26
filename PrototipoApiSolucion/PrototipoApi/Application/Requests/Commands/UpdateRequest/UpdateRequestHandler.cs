// Comentario explicativo para el profesor:
// Este archivo implementa la lógica para actualizar solicitudes y registrar el historial de cambios de estado.
// Cambios recientes: se registra el historial solo si el estado cambia, usando RequestStatusHistory.
// Revisa la documentación y los commits para más detalles.

using MediatR;
using PrototipoApi.BaseDatos;
using PrototipoApi.Entities;
using PrototipoApi.Repositories.Interfaces;

namespace PrototipoApi.Application.Requests.Commands.UpdateRequest
{
    public class UpdateRequestHandler : IRequestHandler<UpdateRequestCommand, bool>
    {
        private readonly IRepository<Request> _repository;
        private readonly IRepository<RequestStatusHistory> _requestStatusHistory;

        public UpdateRequestHandler(IRepository<Request> repository, IRepository<RequestStatusHistory> requestStatusHistory)
        {
            _repository = repository;
            _requestStatusHistory = requestStatusHistory;
        }

        public async Task<bool> Handle(UpdateRequestCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);

            if (entity == null)
                return false;

            int oldStatusId = entity.StatusId;

            entity.MaintenanceAmount = request.Dto.MaintenanceAmount;
            // Si el estado cambia, registrar el historial
            if (request.Dto.NewStatusId != 0 && request.Dto.NewStatusId != entity.StatusId)
            {
                entity.StatusId = request.Dto.NewStatusId;
                await _requestStatusHistory.AddAsync(new RequestStatusHistory
                {
                    RequestId = entity.RequestId,
                    OldStatusId = oldStatusId,
                    NewStatusId = entity.StatusId,
                    ChangeDate = DateTime.UtcNow,
                    Comment = "Cambio de estado de la solicitud"
                });
                await _requestStatusHistory.SaveChangesAsync();
            }

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
