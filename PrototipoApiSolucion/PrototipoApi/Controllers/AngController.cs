using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using PrototipoApi.Application.Requests.Queries.GetAllRequests;
using System.Linq;
using System.Threading.Tasks;
using PrototipoApi.Application.AngController.Query;
using PrototipoApi.Application.AngController.Query.GetRequestResumen;

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
    }
}