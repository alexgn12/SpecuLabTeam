using Microsoft.EntityFrameworkCore;
using PrototipoApi.BaseDatos;
using PrototipoApi.Data;
using PrototipoApi.Entities;

public static class DbInitializer
{
    public static async Task SeedAsync(ContextoBaseDatos context)
    {
        //// 0. TransactionTypes
        //if (!context.TransactionsTypes.Any())
        //{
        //    var transactionTypes = Seeder.GenerateTransactionTypes();
        //    context.TransactionsTypes.AddRange(transactionTypes);
        //    await context.SaveChangesAsync();
        //}
        //var transactionTypeList = await context.TransactionsTypes.ToListAsync();

        //// 1. Buildings
        //if (!context.Buildings.Any())
        //{
        //    var buildings = Seeder.GenerateBuildings(20);
        //    context.Buildings.AddRange(buildings);
        //    await context.SaveChangesAsync();
        //}
        //var buildingList = await context.Buildings.ToListAsync();

        //// 1b. Apartments
        //if (!context.Apartments.Any())
        //{
        //    var apartments = Seeder.GenerateApartments(60, buildingList);
        //    context.Apartments.AddRange(apartments);
        //    await context.SaveChangesAsync();
        //}

        // 2. Statuses
        if (!context.Statuses.Any())
        {
            var statuses = Seeder.GenerateStatuses();
            context.Statuses.AddRange(statuses);
            await context.SaveChangesAsync();
        }
        var statusList = await context.Statuses.ToListAsync();

        //// 2b. Requests
        //if (!context.Requests.Any())
        //{
        //    var requests = Seeder.GenerateRequests(40, buildingList, statusList);
        //    context.Requests.AddRange(requests);
        //    await context.SaveChangesAsync();
        //}
        //var requestList = await context.Requests.ToListAsync();

        //// 3. Transactions (must come before ManagementBudgets)
        //if (!context.Transactions.Any())
        //{
        //    var transactions = Seeder.GenerateTransactions(100, requestList, transactionTypeList);
        //    context.Transactions.AddRange(transactions);
        //    await context.SaveChangesAsync();
        //}
        //var transactionList = await context.Transactions.ToListAsync();

        //// 4. ManagementBudgets (must reference existing Transactions)
        //if (!context.ManagementBudgets.Any())
        //{
        //    var managementBudgets = Seeder.GenerateManagementBudgets(5);
        //    // Assign a valid TransactionId to each ManagementBudget
        //    for (int i = 0; i < managementBudgets.Count; i++)
        //    {
        //        var transaction = transactionList.ElementAtOrDefault(i % transactionList.Count);
        //        if (transaction != null)
        //        {
        //            managementBudgets[i].TransactionId = transaction.TransactionId;
        //        }
        //    }
        //    context.ManagementBudgets.AddRange(managementBudgets);
        //    await context.SaveChangesAsync();
        //}
    }
}
