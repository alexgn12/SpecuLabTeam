using Bogus;
using PrototipoApi.Entities;

namespace PrototipoApi.Data
{
    public static class Seeder
    {
        private static readonly string[] StatusTypes = new[]
        {
            "Recibido", "Pendiente", "Aprobado", "Rechazado"
        };

        private static readonly string[] TransactionsTypes = new[]
        {
            "INGRESO", "GASTO"
        };

        // 0. TransactionTypes
        public static List<TransactionType> GenerateTransactionTypes()
        {
            return TransactionsTypes.Select(name => new TransactionType
            {
                TransactionName = name
            }).ToList();
        }

        // 1. Buildings
        public static List<Building> GenerateBuildings(int count)
        {
            var faker = new Faker<Building>()
                .RuleFor(b => b.BuildingCode, f => f.Random.String2(6, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"))
                .RuleFor(b => b.BuildingName, f => f.Company.CompanyName())
                .RuleFor(b => b.Street, f => f.Address.StreetAddress())
                .RuleFor(b => b.District, f => f.Address.City())
                .RuleFor(b => b.CreatedDate, f => f.Date.Past(10))
                .RuleFor(b => b.FloorCount, f => f.Random.Int(1, 20))
                .RuleFor(b => b.YearBuilt, f => f.Date.Past(50).Year);

            return faker.Generate(count);
        }

        // 1b. Apartments
        public static List<Apartment> GenerateApartments(int count, List<Building> buildings)
        {
            var faker = new Faker<Apartment>()
                .RuleFor(a => a.ApartmentCode, f => f.Random.String2(4, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"))
                .RuleFor(a => a.ApartmentDoor, f => f.Address.BuildingNumber())
                .RuleFor(a => a.ApartmentFloor, f => f.Random.Int(1, 10).ToString())
                .RuleFor(a => a.ApartmentPrice, f => f.Finance.Amount(50000, 300000))
                .RuleFor(a => a.NumberOfRooms, f => f.Random.Int(1, 5))
                .RuleFor(a => a.NumberOfBathrooms, f => f.Random.Int(1, 3))
                .RuleFor(a => a.BuildingId, f => f.PickRandom(buildings).BuildingId)
                .RuleFor(a => a.HasLift, f => f.Random.Bool())
                .RuleFor(a => a.HasGarage, f => f.Random.Bool())
                .RuleFor(a => a.CreatedDate, f => f.Date.Past(5));
            return faker.Generate(count);
        }

        // 2. Requests y Status únicos
        public static List<Status> GenerateStatuses()
        {
            var now = DateTime.UtcNow;
            return StatusTypes.Select(type => new Status
            {
                StatusType = type,
                Description = $"Estado {type}",
                CreatedAt = now,
                UpdatedAt = now
            }).ToList();
        }

        public static List<Request> GenerateRequests(int count, List<Building> buildings, List<Status> statuses)
        {
            var requestFaker = new Faker<Request>()
                .RuleFor(r => r.BuildingAmount, f => (double)f.Finance.Amount(50000, 500000))
                .RuleFor(r => r.MaintenanceAmount, f => 0) // MaintenanceAmount siempre será 0
                .RuleFor(r => r.Description, f => f.Lorem.Sentence(6))
                .RuleFor(r => r.RequestDate, f => f.Date.Past(1))
                .RuleFor(r => r.BuildingId, f => f.PickRandom(buildings).BuildingId)
                .RuleFor(r => r.StatusId, f => f.PickRandom(statuses).StatusId);

            return requestFaker.Generate(count);
        }

        // 3. ManagementBudgets
        public static List<ManagementBudget> GenerateManagementBudgets(int count)
        {
            var faker = new Faker<ManagementBudget>()
                .RuleFor(mb => mb.CurrentAmount, f => 50000 + (double)f.Finance.Amount(-20000, 40000))
                .RuleFor(mb => mb.LastUpdatedDate, f => f.Date.Recent(30));
            return faker.Generate(count);
        }

        // 4. Transactions
        public static List<Transaction> GenerateTransactions(
            int count,
            List<Request> requests,
            List<TransactionType> transactionTypes)
        {
            var transactionFaker = new Faker<Transaction>()
                .RuleFor(t => t.TransactionDate, f => f.Date.Recent(365))
                .RuleFor(t => t.Amount, f => (double)Math.Round(f.Random.Decimal(100, 50000), 2))
                .RuleFor(t => t.Description, f => f.Lorem.Sentence(5))
                .RuleFor(t => t.RequestId, f => f.PickRandom(requests).RequestId)
                .RuleFor(t => t.TransactionTypeId, f => f.PickRandom(transactionTypes).TransactionTypeId);
            return transactionFaker.Generate(count);
        }
    }
}
