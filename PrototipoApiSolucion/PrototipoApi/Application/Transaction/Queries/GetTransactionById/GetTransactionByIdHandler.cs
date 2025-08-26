using MediatR;
using PrototipoApi.Models;
using PrototipoApi.Entities;
using PrototipoApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;

public class GetTransactionByIdHandler : IRequestHandler<GetTransactionByIdQuery, TransactionDto?>
{
    private readonly IRepository<Transaction> _repository;

    public GetTransactionByIdHandler(IRepository<Transaction> repository)
    {
        _repository = repository;
    }

    public async Task<TransactionDto?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _repository.GetOneAsync(
            t => t.TransactionId == request.Id,
            t => t.Request,
            t => t.TransactionsType
        );

        if (transaction == null) return null;

        return new TransactionDto
        {
            TransactionId = transaction.TransactionId,
            TransactionDate = transaction.TransactionDate,
            TransactionType = transaction.TransactionsType.TransactionName,
            TransactionTypeId = transaction.TransactionTypeId,
            BuildingAmount = (decimal)transaction.Request.BuildingAmount,
            Description = transaction.Description
            // ManagementBudgetId = transaction.ManagementBudgetId // si lo usas
        };
    }
}

