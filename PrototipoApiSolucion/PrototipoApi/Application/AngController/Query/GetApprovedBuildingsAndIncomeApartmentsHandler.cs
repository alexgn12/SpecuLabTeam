using PrototipoApi.Entities;
using PrototipoApi.Models;
using MediatR;
using PrototipoApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace PrototipoApi.Application.AngController.Query
{
    public class GetApprovedBuildingsAndIncomeApartmentsHandler : IRequestHandler<GetApprovedBuildingsAndIncomeApartmentsQuery, ApprovedBuildingsAndIncomeApartmentsDto>
    {
        private readonly IRepository<Entities.Request> _requests;
        private readonly IRepository<Entities.Status> _statuses;
        private readonly IRepository<Entities.Building> _buildings;
        private readonly IRepository<Entities.Transaction> _transactions;
        private readonly IRepository<Entities.TransactionType> _transactionTypes;
        private readonly IRepository<Entities.Apartment> _apartments;

        public GetApprovedBuildingsAndIncomeApartmentsHandler(
            IRepository<Entities.Request> requests,
            IRepository<Entities.Status> statuses,
            IRepository<Entities.Building> buildings,
            IRepository<Entities.Transaction> transactions,
            IRepository<Entities.TransactionType> transactionTypes,
            IRepository<Entities.Apartment> apartments)
        {
            _requests = requests;
            _statuses = statuses;
            _buildings = buildings;
            _transactions = transactions;
            _transactionTypes = transactionTypes;
            _apartments = apartments;
        }

        public async Task<ApprovedBuildingsAndIncomeApartmentsDto> Handle(GetApprovedBuildingsAndIncomeApartmentsQuery request, CancellationToken cancellationToken)
        {
            // 1. Edificios aprobados
            var aprobadoStatus = await _statuses.GetOneAsync(s => s.StatusType == "Aprobado");
            var approvedBuildings = new List<Entities.Building>();
            if (aprobadoStatus != null)
            {
                var approvedRequests = await _requests.SelectListAsync<Entities.Request>(
                    r => r.StatusId == aprobadoStatus.StatusId,
                    null,
                    r => r,
                    0,
                    int.MaxValue,
                    cancellationToken
                );
                var buildingIds = approvedRequests.Select(r => r.BuildingId).Distinct().ToList();
                approvedBuildings = await _buildings.SelectListAsync<Entities.Building>(
                    b => buildingIds.Contains(b.BuildingId),
                    null,
                    b => b,
                    0,
                    int.MaxValue,
                    cancellationToken
                );
            }

            // 2. Apartamentos con ingresos
            var ingresoType = await _transactionTypes.GetOneAsync(t => t.TransactionName == "INGRESO");
            var incomeApartments = new List<Entities.Apartment>();
            if (ingresoType != null)
            {
                var incomeTransactions = await _transactions.SelectListAsync<Entities.Transaction>(
                    t => t.TransactionTypeId == ingresoType.TransactionTypeId && t.ApartmentId != null,
                    null,
                    t => t,
                    0,
                    int.MaxValue,
                    cancellationToken
                );
                var apartmentIds = incomeTransactions.Select(t => t.ApartmentId.Value).Distinct().ToList();
                incomeApartments = await _apartments.SelectListAsync<Entities.Apartment>(
                    a => apartmentIds.Contains(a.ApartmentId),
                    null,
                    a => a,
                    0,
                    int.MaxValue,
                    cancellationToken
                );
            }

            // Map to DTOs
            var result = new ApprovedBuildingsAndIncomeApartmentsDto
            {
                ApprovedBuildings = approvedBuildings.Select(b => new BuildingDto
                {
                    BuildingId = b.BuildingId,
                    BuildingCode = b.BuildingCode,
                    BuildingName = b.BuildingName,
                    Street = b.Street,
                    District = b.District,
                    CreatedDate = b.CreatedDate,
                    FloorCount = b.FloorCount,
                    YearBuilt = b.YearBuilt
                }).ToList(),
                IncomeApartments = incomeApartments.Select(a => new ApartmentDto
                {
                    ApartmentId = a.ApartmentId,
                    ApartmentCode = a.ApartmentCode,
                    ApartmentDoor = a.ApartmentDoor,
                    ApartmentFloor = a.ApartmentFloor,
                    ApartmentPrice = a.ApartmentPrice,
                    NumberOfRooms = a.NumberOfRooms,
                    NumberOfBathrooms = a.NumberOfBathrooms,
                    BuildingId = a.BuildingId,
                    HasLift = a.HasLift,
                    HasGarage = a.HasGarage,
                    CreatedDate = a.CreatedDate
                }).ToList()
            };
            return result;
        }
    }
}
