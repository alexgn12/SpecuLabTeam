using MediatR;
using PrototipoApi.Entities;
using PrototipoApi.Models;
using PrototipoApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;

public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    private readonly IRepository<Transaction> _repository;

    public CreateTransactionHandler(IRepository<Transaction> repository)
    {
        _repository = repository;
    }

    public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = new Transaction
        {
            TransactionDate = request.TransactionDate,
            TransactionTypeId = request.TransactionTypeId,
            RequestId = request.RequestId
            // ManagementBudgetId = request.ManagementBudgetId
        };

        await _repository.AddAsync(transaction);
        await _repository.SaveChangesAsync();


        return new TransactionDto
        {
            TransactionId = transaction.TransactionId,
            TransactionDate = transaction.TransactionDate,
            TransactionTypeId = transaction.TransactionTypeId,
            BuildingAmount = (decimal) transaction.Request.BuildingAmount
            // ManagementBudgetId = transaction.ManagementBudgetId
        };
    }
}

