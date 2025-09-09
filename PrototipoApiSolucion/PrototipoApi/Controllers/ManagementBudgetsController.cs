using MediatR;
using Microsoft.AspNetCore.Mvc;
using PrototipoApi.Application.ManagementBudget.Queries;
using PrototipoApi.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrototipoApi.Controllers
{
    [Route("api/managementbudgets")]
    [ApiController]
    public class ManagementBudgetsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ManagementBudgetsController> _logger;

        public ManagementBudgetsController(IMediator mediator, ILogger<ManagementBudgetsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<Result<IEnumerable<ManagementBudgetDto>>>> GetManagementBudgets()
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los management budgets");
                var result = await _mediator.Send(new GetAllManagementBudgetsQuery());
                return Ok(Result<IEnumerable<ManagementBudgetDto>>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener management budgets");
                return StatusCode(500, Result<IEnumerable<ManagementBudgetDto>>.Fail("Error interno del servidor", ex));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _logger.LogInformation($"Obteniendo management budget con id {id}");
                var result = await _mediator.Send(new GetManagementBudgetByIdQuery(id));

                if (result == null)
                {
                    _logger.LogWarning($"Management budget con id {id} no encontrado");
                    return NotFound(Result<ManagementBudgetDto>.Fail("No encontrado"));
                }

                return Ok(Result<ManagementBudgetDto>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener management budget por id");
                return StatusCode(500, Result<ManagementBudgetDto>.Fail("Error interno del servidor", ex));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateManagementBudgetDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _logger.LogInformation($"Actualizando management budget con id {id}");
                var result = await _mediator.Send(new UpdateManagementBudgetCommand(
                    id,
                    dto.CurrentAmount,
                    dto.LastUpdatedDate
                ));

                if (result == null)
                {
                    _logger.LogWarning($"Management budget con id {id} no encontrado para actualizar");
                    return NotFound(Result<ManagementBudgetDto>.Fail("No encontrado"));
                }

                _logger.LogInformation($"Management budget actualizado con id {id}");
                return Ok(Result<ManagementBudgetDto>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar management budget");
                return StatusCode(500, Result<ManagementBudgetDto>.Fail("Error interno del servidor", ex));
            }
        }
    }
}
