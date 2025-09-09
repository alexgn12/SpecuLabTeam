using MediatR;
using Microsoft.AspNetCore.Mvc;
using PrototipoApi.Models;
using PrototipoApi.Application.Building.Queries.GetAllBuildings;
using PrototipoApi.Application.Building.Queries.GetBuildingById;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrototipoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BuildingController> _logger;

        public BuildingController(IMediator mediator, ILogger<BuildingController> logger)
        {
            _mediator = mediator;
            this._logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<BuildingDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] int floorCount = 0)
        {
            var result = await _mediator.Send(new GetAllBuildingsQuery(page, size, floorCount));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BuildingDto>> GetById(int id)
        {
            _logger.LogInformation($"Iniciando búsqueda de Building con ID: {id}");
            try
            {
                var result = await _mediator.Send(new GetBuildingByIdQuery(id));
                if (result == null)
                {
                    _logger.LogInformation($"Building con ID: {id} no encontrado");
                    return NotFound();
                }
                _logger.LogInformation($"Building con ID: {id} encontrado exitosamente");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener Building con ID: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BuildingDto>> Create([FromBody] BuildingDto dto)
        {
            var result = await _mediator.Send(new CreateBuildingCommand(dto));
            return CreatedAtAction(nameof(GetAll), new { id = result.BuildingId }, result);
        }
    }
}
