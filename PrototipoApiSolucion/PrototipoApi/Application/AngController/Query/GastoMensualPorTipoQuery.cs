using MediatR;
using Microsoft.EntityFrameworkCore;
using PrototipoApi.BaseDatos;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PrototipoApi.Application.AngController.Query
{
    public class GastoMensualPorTipoQuery : IRequest<List<GastoMensualPorTipoQuery.GastoMensualPorTipoDto>>
    {
        public class GastoMensualPorTipoDto
        {
            public int Año { get; set; }
            public int Mes { get; set; }
            public string TransactionType { get; set; } = string.Empty;
            public double TotalGasto { get; set; }
        }

        public class Handler : IRequestHandler<GastoMensualPorTipoQuery, List<GastoMensualPorTipoDto>>
        {
            private readonly ContextoBaseDatos _context;
            public Handler(ContextoBaseDatos context)
            {
                _context = context;
            }

            public async Task<List<GastoMensualPorTipoDto>> Handle(GastoMensualPorTipoQuery request, CancellationToken cancellationToken)
            {
                var query = from t in _context.Transactions
                            where t.TransactionsType.TransactionName == "GASTO" || t.TransactionsType.TransactionName == "INGRESO"
                            group t by new { t.TransactionDate.Year, t.TransactionDate.Month, t.TransactionsType.TransactionName } into g
                            select new GastoMensualPorTipoDto
                            {
                                Año = g.Key.Year,
                                Mes = g.Key.Month,
                                TransactionType = g.Key.TransactionName,
                                TotalGasto = g.Key.TransactionName == "INGRESO"
                                    ? g.Sum(x => x.Amount)
                                    : g.Sum(x => x.Request.BuildingAmount + x.Request.MaintenanceAmount)
                            };

                return await query
                    .OrderBy(x => x.Año)
                    .ThenBy(x => x.Mes)
                    .ThenBy(x => x.TransactionType)
                    .ToListAsync(cancellationToken);
            }
        }
    }
}
