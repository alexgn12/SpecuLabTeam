using MediatR;
using Microsoft.AspNetCore.Mvc;
using PrototipoApi.Models;
using PrototipoApi.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrototipoApi.Controllers
{
    [Route("api/status")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoguer _loguer;

        public StatusController(IMediator mediator, ILoguer loguer)
        {
            _mediator = mediator;
            _loguer = loguer;
        }

        // GET: api/status
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetStatuses()
        {
            _loguer.LogInfo("Obteniendo todos los status");
            var statuses = await _mediator.Send(new GetAllStatusQuery());
            return Ok(statuses);
        }

        // GET: api/status/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StatusDto>> GetStatus(int id)
        {
            _loguer.LogInfo($"Obteniendo status con id {id}");
            var status = await _mediator.Send(new GetStatusByIdQuery(id));
            if (status == null)
            {
                _loguer.LogWarning($"Status con id {id} no encontrado");
                return NotFound();
            }

            return Ok(status);
        }

        // GET: api/status/type/{statusType}
        [HttpGet("type/{statusType}")]
        public async Task<ActionResult<int>> GetStatusIdByType(string statusType)
        {
            _loguer.LogInfo($"Obteniendo statusId con tipo {statusType}");
            var status = await _mediator.Send(new PrototipoApi.Application.Status.Queries.GetStatusByType.GetStatusByTypeQuery(statusType));
            if (status == null)
            {
                _loguer.LogWarning($"Status con tipo {statusType} no encontrado");
                return NotFound();
            }
            return Ok(status.StatusId);
        }


        // POST: api/status
        [HttpPost]
        public async Task<ActionResult<StatusDto>> PostStatus(CreateStatusDto dto)
        {
            _loguer.LogInfo("Creando nuevo status");
            var createdStatus = await _mediator.Send(new CreateStatusCommand(dto.StatusType, dto.Description));
            _loguer.LogInfo($"Status creado con id {createdStatus.StatusId}");
            return CreatedAtAction(nameof(GetStatus), new { id = createdStatus.StatusId }, createdStatus);
        }
    }
}
