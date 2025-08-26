using MediatR;
using PrototipoApi.Models;
using PrototipoApi.Repositories.Interfaces;
using PrototipoApi.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;

using TransactionEntity = PrototipoApi.Entities.Transaction;

namespace PrototipoApi.Application.Transaction.Queries.GetAllTransaction.ListResult
{
    public class GetAllTransactionsHandler : IRequestHandler<GetAllTransactionsQuery, TransactionListResultDto>
    {
        private readonly IRepository<TransactionEntity> _repository;

        public GetAllTransactionsHandler(IRepository<TransactionEntity> repository)
        {
            _repository = repository;
        }

        public async Task<TransactionListResultDto> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
        {
            var page = request.Page;

            Expression<Func<TransactionEntity, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(request.TransactionType) || request.Year.HasValue || request.Month.HasValue)
            {
                filter = t =>
                    (string.IsNullOrWhiteSpace(request.TransactionType) || t.TransactionsType.TransactionName == request.TransactionType) &&
                    (!request.Year.HasValue || t.TransactionDate.Year == request.Year.Value) &&
                    (!request.Month.HasValue || t.TransactionDate.Month == request.Month.Value);
            }

            Expression<Func<TransactionEntity, TransactionDto>> selector = t => new TransactionDto
            {
                TransactionId = t.TransactionId,
                TransactionDate = t.TransactionDate,
                TransactionType = t.TransactionsType.TransactionName,
                TransactionTypeId = t.TransactionTypeId,
                Description = t.Description,
                BuildingAmount = t.Request != null ? (decimal)t.Request.BuildingAmount : 0,
                Amount = (decimal)t.Amount
            };

            Func<IQueryable<TransactionEntity>, IOrderedQueryable<TransactionEntity>> orderBy = q =>
                q.OrderByDescending(t => t.TransactionDate).ThenBy(t => t.TransactionId);

            var total = await _repository.CountAsync(filter, cancellationToken);
            var items = await _repository.SelectListAsync(
                filter: filter,
                orderBy: orderBy,
                selector: selector,
                skip: (page - 1) * request.Size,
                take: request.Size,
                ct: cancellationToken
            );

            return new TransactionListResultDto
            {
                Items = items,
                Total = total
            };
        }
    }
}
