using MediatR;
using PrototipoApi.Entities;
using PrototipoApi.Models;
using PrototipoApi.Repositories.Interfaces;
using PrototipoApi.Services;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using PrototipoApi.Infrastructure.RealTime;

public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    private readonly IRepository<Transaction> _repository;
    private readonly IRepository<Apartment> _apartmentRepository;
    private readonly IExternalApartmentService _externalApartmentService;
    private readonly IRepository<ManagementBudget> _budgetRepository;
    private readonly IRealTimeNotifier _realTimeNotifier;

    public CreateTransactionHandler(
        IRepository<Transaction> repository,
        IRepository<Apartment> apartmentRepository,
        IExternalApartmentService externalApartmentService,
        IRepository<ManagementBudget> budgetRepository,
        IRealTimeNotifier realTimeNotifier)
    {
        _repository = repository;
        _apartmentRepository = apartmentRepository;
        _externalApartmentService = externalApartmentService;
        _budgetRepository = budgetRepository;
        _realTimeNotifier = realTimeNotifier;
    }

    public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        int? apartmentId = null;
        if (!string.IsNullOrWhiteSpace(request.ApartmentCode))
        {
            var apartment = await _apartmentRepository.GetOneAsync(a => a.ApartmentCode == request.ApartmentCode);
            if (apartment == null)
            {
                // Buscar en API externa y guardar si existe
                var external = await _externalApartmentService.GetApartmentByCodeAsync(request.ApartmentCode);
                if (external != null)
                {
                    await _apartmentRepository.AddAsync(external);
                    await _apartmentRepository.SaveChangesAsync();
                    apartmentId = external.ApartmentId;
                }
            }
            else
            {
                apartmentId = apartment.ApartmentId;
            }
        }

        var transaction = new Transaction
        {
            TransactionDate = request.TransactionDate,
            Description = request.Description,
            Amount = (double)request.Amount,
            ApartmentId = apartmentId,
            TransactionsType = new TransactionType { TransactionName = "INGRESO" }, // Por defecto INGRESO
        };

        await _repository.AddAsync(transaction);
        await _repository.SaveChangesAsync();

        // Recargar la transacción con el tipo real desde la base de datos (por si fue creada como GASTO en otro flujo)
        var savedTransaction = await _repository.GetOneAsync(t => t.TransactionId == transaction.TransactionId, t => t.TransactionsType);
        var transactionTypeName = savedTransaction?.TransactionsType?.TransactionName ?? "INGRESO";

        // Notificación SignalR
        var liveDto = new TransactionLiveDto(
            transaction.TransactionId,
            transactionTypeName,
            (decimal)transaction.Amount,
            transaction.Description,
            transaction.TransactionDate
        );
        await _realTimeNotifier.NotifyTransactionCreated(liveDto, cancellationToken);

        // Actualizar el presupuesto global
        var budget = (await _budgetRepository.GetAllAsync()).FirstOrDefault();
        if (budget != null)
        {
            if (transactionTypeName == "GASTO")
                budget.CurrentAmount -= transaction.Amount;
            else
                budget.CurrentAmount += transaction.Amount;
            budget.LastUpdatedDate = transaction.TransactionDate;
            await _budgetRepository.UpdateAsync(budget);
            await _budgetRepository.SaveChangesAsync();
        }

        return new TransactionDto
        {
            TransactionId = transaction.TransactionId,
            TransactionDate = transaction.TransactionDate,
            TransactionType = transactionTypeName,
            Description = transaction.Description,
            Amount = (decimal)transaction.Amount,
        };
    }
}

