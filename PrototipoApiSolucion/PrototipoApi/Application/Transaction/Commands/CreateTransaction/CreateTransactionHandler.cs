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
    private readonly IRepository<Building> _buildingRepository;

    public CreateTransactionHandler(
        IRepository<Transaction> repository,
        IRepository<Apartment> apartmentRepository,
        IExternalApartmentService externalApartmentService,
        IRepository<ManagementBudget> budgetRepository,
        IRealTimeNotifier realTimeNotifier,
        IRepository<Building> buildingRepository)
    {
        _repository = repository;
        _apartmentRepository = apartmentRepository;
        _externalApartmentService = externalApartmentService;
        _budgetRepository = budgetRepository;
        _realTimeNotifier = realTimeNotifier;
        _buildingRepository = buildingRepository;
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
                    // Buscar el BuildingId usando el BuildingCode
                    var building = await _buildingRepository.GetOneAsync(b => b.BuildingCode == external.BuildingCode);
                    var apartmentEntity = new Apartment
                    {
                        ApartmentCode = external.ApartmentCode,
                        ApartmentDoor = external.ApartmentDoor,
                        ApartmentFloor = external.ApartmentFloor,
                        ApartmentPrice = external.ApartmentPrice,
                        NumberOfRooms = external.NumberOfRooms,
                        NumberOfBathrooms = external.NumberOfBathrooms,
                        HasLift = external.HasLift,
                        HasGarage = external.HasGarage,
                        CreatedDate = external.CreatedDate,
                        BuildingId = building?.BuildingId ?? 0
                    };
                    await _apartmentRepository.AddAsync(apartmentEntity);
                    await _apartmentRepository.SaveChangesAsync();
                    // Buscar el ApartmentId por ApartmentCode tras guardar
                    var savedApartment = await _apartmentRepository.GetOneAsync(a => a.ApartmentCode == apartmentEntity.ApartmentCode);
                    apartmentId = savedApartment?.ApartmentId;
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
            ApartmentId = apartmentId
        };
    }
}

