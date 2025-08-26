using MediatR;
using PrototipoApi.Models;
using System.Collections.Generic;

namespace PrototipoApi.Application.Transaction.Queries.GetAllTransaction.ListResult
{
    // Añadimos Year y Month como parámetros opcionales para el filtro
    // Retorna TransactionListResultDto para paginación
    public record GetAllTransactionsQuery(
        string? TransactionType,
        int Page,
        int Size,
        int? Year = null,
        int? Month = null
    ) : IRequest<TransactionListResultDto>;
}
