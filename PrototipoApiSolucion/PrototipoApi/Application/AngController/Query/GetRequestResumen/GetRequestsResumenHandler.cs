using MediatR;
using Microsoft.EntityFrameworkCore;
using PrototipoApi.BaseDatos;
using PrototipoApi.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PrototipoApi.Application.AngController.Query.GetRequestResumen
{
    public class GetRequestsResumenHandler : IRequestHandler<GetRequestsResumenQuery, Dictionary<string, int>>
    {
        private readonly ContextoBaseDatos _context;

        public GetRequestsResumenHandler(ContextoBaseDatos context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, int>> Handle(GetRequestsResumenQuery request, CancellationToken cancellationToken)
        {
            var resumen = await _context.Requests
                .GroupBy(r => r.Status.StatusType)
                .Select(g => new { Estado = g.Key, Total = g.Count() })
                .ToListAsync(cancellationToken);

            // Asegura que todos los estados estén presentes
            var estados = new[] { "Recibido", "Pendiente", "Aceptado", "Rechazado" };
            var resultado = estados.ToDictionary(
                e => e,
                e => resumen.FirstOrDefault(x => x.Estado == e)?.Total ?? 0
            );

            return resultado;
        }
    }
}