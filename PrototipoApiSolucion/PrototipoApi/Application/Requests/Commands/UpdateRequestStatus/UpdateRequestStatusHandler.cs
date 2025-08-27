using MediatR;
using PrototipoApi.Entities;
using PrototipoApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using PrototipoApi.Models;
using System;

namespace PrototipoApi.Application.Requests.Commands.UpdateRequestStatus
{
    public class UpdateRequestStatusHandler : IRequestHandler<UpdateRequestStatusCommand, bool?>
    {
        private readonly IRepository<Request> _requests;
        private readonly IRepository<Status> _statuses;
        private readonly IRepository<PrototipoApi.Entities.Transaction> _transactions;
        private readonly IRepository<TransactionType> _transactionTypes;
        public UpdateRequestStatusHandler(
            IRepository<Request> requests,
            IRepository<Status> statuses,
            IRepository<PrototipoApi.Entities.Transaction> transactions,
            IRepository<TransactionType> transactionTypes)
        {
            _requests = requests;
            _statuses = statuses;
            _transactions = transactions;
            _transactionTypes = transactionTypes;
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
                    Comment = string.IsNullOrWhiteSpace(request.Comment) ? "Cambio de estado vía API por StatusType" : request.Comment
                });
            });
            await _requests.SaveChangesAsync();

            // Si el nuevo estado es "Aprobado", crear transacción de gasto
            if (status.StatusType == "Aprobado")
            {
                // Buscar el TransactionTypeId para "GASTO"
                var gastoType = await _transactionTypes.GetOneAsync(t => t.TransactionName == "GASTO");
                if (gastoType != null)
                {
                    var amount = entity.BuildingAmount + entity.MaintenanceAmount;
                    var transaction = new PrototipoApi.Entities.Transaction
                    {
                        RequestId = entity.RequestId,
                        TransactionTypeId = gastoType.TransactionTypeId,
                        TransactionDate = DateTime.UtcNow,
                        Amount = amount,
                        Description = $"Gasto generado automáticamente al aprobar la solicitud {entity.RequestId}"
                    };
                    await _transactions.AddAsync(transaction);
                    await _transactions.SaveChangesAsync();
                }
            }
            return true;
        }
    }
}
