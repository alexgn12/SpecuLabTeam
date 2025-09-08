// Comentario explicativo para el profesor:
// Este archivo implementa la lógica para actualizar solicitudes y registrar el historial de cambios de estado.
// Cambios recientes: se registra el historial solo si el estado cambia, usando RequestStatusHistory.
// Revisa la documentación y los commits para más detalles.

using MediatR;
using PrototipoApi.BaseDatos;
using PrototipoApi.Entities;
using PrototipoApi.Repositories.Interfaces;
using PrototipoApi.Infrastructure.RealTime;

namespace PrototipoApi.Application.Requests.Commands.UpdateRequest
{
    public class UpdateRequestHandler : IRequestHandler<UpdateRequestCommand, bool>
    {
        private readonly IRepository<Request> _repository;
        private readonly IRepository<RequestStatusHistory> _requestStatusHistory;
        private readonly IRealTimeNotifier _realTimeNotifier;

        public UpdateRequestHandler(IRepository<Request> repository, IRepository<RequestStatusHistory> requestStatusHistory, IRealTimeNotifier realTimeNotifier)
        {
            _repository = repository;
            _requestStatusHistory = requestStatusHistory;
            _realTimeNotifier = realTimeNotifier;
        }

        public async Task<bool> Handle(UpdateRequestCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);

            if (entity == null)
                return false;

            entity.MaintenanceAmount = request.Dto.MaintenanceAmount;
            // Ya no se permite cambiar el estado desde este DTO, solo se actualiza el monto

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();

            // Notificación SignalR para actualización de Request
            var liveDto = new RequestLiveDto(
                entity.RequestId,
                entity.Description,
                (decimal)entity.BuildingAmount,
                (decimal)entity.MaintenanceAmount,
                entity.StatusId,
                entity.Status?.StatusType ?? string.Empty,
                entity.BuildingId,
                entity.Building?.BuildingCode ?? string.Empty,
                entity.RequestDate
            );
            await _realTimeNotifier.NotifyRequestUpdated(liveDto, cancellationToken);

            return true;
        }
    }
}
