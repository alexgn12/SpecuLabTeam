using MediatR;
using PrototipoApi.Models;
using System;

public record CreateTransactionCommand(
    DateTime TransactionDate,
    string Description,
    decimal Amount,
    string ApartmentCode
    // Puedes agregar más campos si lo necesitas
) : IRequest<TransactionDto>;

