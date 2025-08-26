using MediatR;
using PrototipoApi.Models;
using PrototipoApi.Repositories.Interfaces;
using PrototipoApi.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;

public class GetAllTransactionsHandler : IRequestHandler<GetAllTransactionsQuery, (List<TransactionDto> Items, int Total)>
{
    private readonly IRepository<Transaction> _repository;

    public GetAllTransactionsHandler(IRepository<Transaction> repository)
    {
        _repository = repository;
    }

    public async Task<(List<TransactionDto> Items, int Total)> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page;

        Expression<Func<Transaction, bool>>? filter = null;
        // Filtro compuesto por tipo, año y mes
        if (!string.IsNullOrWhiteSpace(request.TransactionType) || request.Year.HasValue || request.Month.HasValue)
        {
            filter = t =>
                (string.IsNullOrWhiteSpace(request.TransactionType) || t.TransactionsType.TransactionName == request.TransactionType) &&
                (!request.Year.HasValue || t.TransactionDate.Year == request.Year.Value) &&
                (!request.Month.HasValue || t.TransactionDate.Month == request.Month.Value);
        }

        Expression<Func<Transaction, TransactionDto>> selector = t => new TransactionDto
        {
            TransactionId = t.TransactionId,
            TransactionDate = t.TransactionDate,
            TransactionType = t.TransactionsType.TransactionName,
            TransactionTypeId = t.TransactionTypeId,
            Description = t.Description,
            BuildingAmount = t.Request != null ? (decimal)t.Request.BuildingAmount : 0
        };

        Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>> orderBy = q =>
            q.OrderByDescending(t => t.TransactionDate).ThenBy(t => t.TransactionId);

        // Obtener total
        var total = await _repository.CountAsync(filter, cancellationToken);
        // Obtener items paginados
        var items = await _repository.SelectListAsync(
            filter: filter,
            orderBy: orderBy,
            selector: selector,
            skip: (page - 1) * request.Size,
            take: request.Size,
            ct: cancellationToken
        );

        return (items, total);
    }
}

