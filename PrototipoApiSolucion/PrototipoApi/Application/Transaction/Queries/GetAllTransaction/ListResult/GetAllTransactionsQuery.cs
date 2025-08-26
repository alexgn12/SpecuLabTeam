using MediatR;
using PrototipoApi.Models;
using System.Collections.Generic;

namespace PrototipoApi.Application.Transaction.Queries.GetAllTransaction.ListResult
{
    // A�adimos Year y Month como par�metros opcionales para el filtro
    // Retorna TransactionListResultDto para paginaci�n
    public record GetAllTransactionsQuery(
        string? TransactionType,
        int Page,
        int Size,
        int? Year = null,
        int? Month = null
    ) : IRequest<TransactionListResultDto>;
}
