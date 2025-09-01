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

        [HttpGet("gasto-mensual")]
        public async Task<IActionResult> GetGastoMensual()
        {
            var resultado = await _mediator.Send(new GastoMensualPorTipoQuery());
            return Ok(resultado);
        }

        [HttpGet("buildings-count-by-district")]
        public async Task<IActionResult> GetBuildingsCountByDistrict()
        {
            var result = await _mediator.Send(new GetBuildingsCountByDistrictQuery());
            return Ok(result);
        }

        [HttpGet("edificios-comprados-count")]
        public async Task<IActionResult> GetEdificiosCompradosCount()
        {
            var count = await _mediator.Send(new GetEdificiosCompradosCountQuery());
            return Ok(new { EdificiosComprados = count });
        }

        [HttpGet("aprobados-e-ingresos")]
        public async Task<IActionResult> GetApprovedBuildingsAndIncomeApartments()
        {
            var result = await _mediator.Send(new GetApprovedBuildingsAndIncomeApartmentsQuery());
            return Ok(result);
        }

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