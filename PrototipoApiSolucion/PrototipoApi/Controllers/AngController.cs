using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using PrototipoApi.Application.Requests.Queries.GetAllRequests;
using System.Linq;
using System.Threading.Tasks;

namespace PrototipoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AngController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AngController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("resumen-requests")]
        public async Task<IActionResult> GetResumenRequests()
        {
            // Obtener todas las solicitudes (sin paginación ni filtro de estado)
            var requests = await _mediator.Send(new GetAllRequestsQuery(Page: 1, Size: int.MaxValue));

            // Agrupar por tipo de estado y contar
            var resumen = requests
                .GroupBy(r => r.StatusType)
                .Select(g => new
                {
                    Estado = g.Key,
                    Total = g.Count()
                })
                .ToList();

            // Para asegurar que todos los estados estén presentes en el resumen
            var estados = new[] { "Recibido", "Pendiente", "Aceptado", "Rechazado" };
            var resultado = estados.Select(e => new
            {
                Estado = e,
                Total = resumen.FirstOrDefault(x => x.Estado == e)?.Total ?? 0
            });

            // Total general
            var totalGeneral = requests.Count;

            return Ok(new
            {
                TotalGeneral = totalGeneral,
                PorEstado = resultado
            });
        }
    }
}
