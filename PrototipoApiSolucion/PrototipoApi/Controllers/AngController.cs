using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Linq;
using System.Threading.Tasks;
using PrototipoApi.Application.AngController.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PrototipoApi.BaseDatos;

namespace PrototipoApi.Controllers
{
    // Controlador para endpoints relacionados con estadísticas y gestión de edificios, apartamentos y presupuesto
    [Route("api/[controller]")]
    [ApiController]
    public class AngController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ContextoBaseDatos _context;

        public AngController(IMediator mediator, ContextoBaseDatos context)
        {
            _mediator = mediator;
            _context = context;
        }

        // Endpoint para obtener un resumen de requests agrupados por estado
        // Devuelve el total general y el total por cada estado
        [HttpGet("resumen-requests")]
        public async Task<IActionResult> GetResumenRequests()
        {
            var resumen = await _mediator.Send(new GetRequestsResumenQuery());

            var totalGeneral = resumen.Values.Sum();

            return Ok(new
            {
                TotalGeneral = totalGeneral,
                PorEstado = resumen.Select(x => new { Estado = x.Key, Total = x.Value })
            });
        }

        // Endpoint para obtener el gasto mensual agrupado por tipo
        [HttpGet("gasto-mensual")]
        public async Task<IActionResult> GetGastoMensual()
        {
            var resultado = await _mediator.Send(new GastoMensualPorTipoQuery());
            return Ok(resultado);
        }

        // Endpoint para obtener la cantidad de edificios por distrito
        [HttpGet("buildings-count-by-district")]
        public async Task<IActionResult> GetBuildingsCountByDistrict()
        {
            var result = await _mediator.Send(new GetBuildingsCountByDistrictQuery());
            return Ok(result);
        }

        // Endpoint para obtener la cantidad de edificios comprados
        [HttpGet("edificios-comprados-count")]
        public async Task<IActionResult> GetEdificiosCompradosCount()
        {
            var count = await _mediator.Send(new GetEdificiosCompradosCountQuery());
            return Ok(new { EdificiosComprados = count });
        }

        // Endpoint para obtener edificios aprobados y apartamentos que generan ingresos
        [HttpGet("aprobados-e-ingresos")]
        public async Task<IActionResult> GetApprovedBuildingsAndIncomeApartments()
        {
            var result = await _mediator.Send(new GetApprovedBuildingsAndIncomeApartmentsQuery());
            return Ok(result);
        }

        // Endpoint para obtener el monto actual del presupuesto de gestión
        [HttpGet("current-amount")]
        public async Task<IActionResult> GetCurrentAmount()
        {
            var budget = await _context.ManagementBudgets.FirstOrDefaultAsync();
            if (budget == null)
                return NotFound("No existe presupuesto de gestión");
            return Ok(new { budget.CurrentAmount });
        }
    }
}