using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrototipoApi.BaseDatos;
using PrototipoApi.Models.GammaAI;


namespace PrototipoApi.Controllers.GammaAI
{
    public static class RentabilidadEndpoints
    {
        public static IEndpointRouteBuilder MapRentabilidadEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/rentabilidad", async (ContextoBaseDatos db) =>
            {
                var query = db.Set<PrototipoApi.Entities.Transaction>()
                    .Include(t => t.TransactionsType) // necesario para acceder a Name
                    .AsNoTracking();

                var ingresos = await query
                    .Where(t => t.TransactionsType.TransactionName == "INGRESO")
                    .SumAsync(t => (double?)t.Amount) ?? 0d;

                var gastos = await query
                    .Where(t => t.TransactionsType.TransactionName == "GASTO")
                    .SumAsync(t => (double?)t.Amount) ?? 0d;

                var rentabilidad = ingresos > 0
                    ? ((ingresos - gastos) / ingresos) * 100.0
                    : 0.0;

                var result = new RentabilidadResponse
                {
                    Ingresos = Math.Round(ingresos, 2),
                    Gastos = Math.Round(gastos, 2),
                    Rentabilidad = Math.Round(rentabilidad, 2)
                };

                return Results.Ok(result);
            })
            .WithName("GetRentabilidadActual")
            .WithSummary("Devuelve la rentabilidad actual (porcentaje de beneficios)")
            .WithDescription("Calcula (Ingresos - Gastos) / Ingresos * 100 a partir de la tabla Transactions.")
            .Produces<RentabilidadResponse>(StatusCodes.Status200OK);

            return app;
        }
    }
}
