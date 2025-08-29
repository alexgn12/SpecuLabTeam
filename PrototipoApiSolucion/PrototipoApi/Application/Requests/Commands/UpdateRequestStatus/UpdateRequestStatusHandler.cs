using MediatR;
using PrototipoApi.Entities;
using PrototipoApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using PrototipoApi.Models;
using System;
using System.Linq;


namespace PrototipoApi.Application.Requests.Commands.UpdateRequestStatus
{
    public class UpdateRequestStatusHandler : IRequestHandler<UpdateRequestStatusCommand, bool?>
    {
        private readonly IRepository<Request> _requests;
        private readonly IRepository<PrototipoApi.Entities.Status> _statuses;
        private readonly IRepository<PrototipoApi.Entities.Transaction> _transactions;
        private readonly IRepository<TransactionType> _transactionTypes;
        private readonly IRepository<PrototipoApi.Entities.ManagementBudget> _budgetRepository;
        public UpdateRequestStatusHandler(
            IRepository<Request> requests,
            IRepository<PrototipoApi.Entities.Status> statuses,
            IRepository<PrototipoApi.Entities.Transaction> transactions,
            IRepository<TransactionType> transactionTypes,
            IRepository<PrototipoApi.Entities.ManagementBudget> budgetRepository)
        {
            _requests = requests;
            _statuses = statuses;
            _transactions = transactions;
            _transactionTypes = transactionTypes;
            _budgetRepository = budgetRepository;
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
                        TransactionsType = gastoType, // Asignar la navegación también
                        TransactionDate = DateTime.UtcNow,
                        Amount = amount,
                        Description = $"Gasto generado automáticamente al aprobar la solicitud {entity.RequestId}"
                    };
                    await _transactions.AddAsync(transaction);
                    await _transactions.SaveChangesAsync();

                    // Actualizar el presupuesto global (restar el gasto)
                    var budget = (await _budgetRepository.GetAllAsync()).FirstOrDefault();
                    if (budget != null)
                    {
                        budget.CurrentAmount -= amount;
                        budget.LastUpdatedDate = transaction.TransactionDate;
                        await _budgetRepository.UpdateAsync(budget);
                        await _budgetRepository.SaveChangesAsync();
                    }
                }
            }
            return true;
        }
    }
}

