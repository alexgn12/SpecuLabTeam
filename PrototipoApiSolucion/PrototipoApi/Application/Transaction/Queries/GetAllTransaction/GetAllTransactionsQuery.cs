using MediatR;
using PrototipoApi.Models;
using System.Collections.Generic;

// Añadimos Year y Month como parámetros opcionales para el filtro
public record GetAllTransactionsQuery(
    string? TransactionType,
    int Page,
    int Size,
    int? Year = null,
    int? Month = null
) : IRequest<List<TransactionDto>>;

