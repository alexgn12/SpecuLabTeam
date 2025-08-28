using MediatR;
using PrototipoApi.Entities;
using PrototipoApi.Models;
using PrototipoApi.Repositories.Interfaces;
using PrototipoApi.Services;
using System.Threading;
using System.Threading.Tasks;

public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    private readonly IRepository<Transaction> _repository;
    private readonly IRepository<Apartment> _apartmentRepository;
    private readonly IExternalApartmentService _externalApartmentService;

    public CreateTransactionHandler(
        IRepository<Transaction> repository,
        IRepository<Apartment> apartmentRepository,
        IExternalApartmentService externalApartmentService)
    {
        _repository = repository;
        _apartmentRepository = apartmentRepository;
        _externalApartmentService = externalApartmentService;
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
            TransactionsType = new TransactionType { TransactionName = "INGRESO" },
        };

        await _repository.AddAsync(transaction);
        await _repository.SaveChangesAsync();

        return new TransactionDto
        {
            TransactionId = transaction.TransactionId,
            TransactionDate = transaction.TransactionDate,
            TransactionType = transaction.TransactionsType.TransactionName,
            Description = transaction.Description,
            Amount = (decimal)transaction.Amount,
        };
    }
}

