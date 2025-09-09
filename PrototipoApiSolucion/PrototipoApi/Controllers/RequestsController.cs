using MediatR;
using Microsoft.AspNetCore.Mvc;
using PrototipoApi.Models;
using Microsoft.Extensions.Logging;
using PrototipoApi.BaseDatos;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PrototipoApi.Application.Requests.Commands.CreateRequest;
using PrototipoApi.Application.Requests.Commands.UpdateRequest;
using PrototipoApi.Application.Requests.Commands.PatchRequest;
using PrototipoApi.Application.Requests.Queries.GetRequestById;
using PrototipoApi.Application.Requests.Queries.GetRequestByBuildingCode;
using PrototipoApi.Entities;

namespace PrototipoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestsController> _logger;
        private readonly ContextoBaseDatos _context;

        public RequestsController(IMediator mediator, ILogger<RequestsController> logger, ContextoBaseDatos context)
        {
            _mediator = mediator;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<RequestDto>>> Get([FromQuery] GetAllRequestsQuery query)
        {
            _logger.LogInformation("Obteniendo todas las requests");
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<RequestDto>>> GetById(int id)
        {
            try
            {
                _logger.LogInformation($"Obteniendo request con id {id}");
                var result = await _mediator.Send(new GetRequestByIdQuery(id));
                if (result == null)
                {
                    _logger.LogWarning($"Request con id {id} no encontrada");
                    return NotFound(Result<RequestDto>.Fail("No encontrado"));
                }
                return Ok(Result<RequestDto>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener request por id");
                return StatusCode(500, Result<RequestDto>.Fail("Error interno del servidor", ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult<RequestDto>> CreateRequest([FromBody] CreateRequestDto dto)
        {
            _logger.LogInformation("Creando nueva request");
            var result = await _mediator.Send(new CreateRequestCommand(dto));
            _logger.LogInformation($"Request creada con id {result.RequestId}");
            return CreatedAtAction(nameof(GetById), new { id = result.RequestId }, result);
        }

        [HttpPut("{buildingCode}/amounts")]
        public async Task<IActionResult> UpdateAmountsByBuildingCode(string buildingCode, [FromBody] UpdateRequestDto dto)
        {
            _logger.LogInformation($"Actualizando montos de la request para el edificio con código {buildingCode}");
            // Buscar la última request asociada a ese BuildingCode
            var request = await _mediator.Send(new GetRequestByBuildingCodeQuery(buildingCode));
            if (request == null)
            {
                _logger.LogWarning($"No se encontró ninguna solicitud para el edificio con código {buildingCode}");
                return NotFound($"No se encontró ninguna solicitud para el edificio con código {buildingCode}");
            }
            var success = await _mediator.Send(new UpdateRequestCommand(request.RequestId, dto));
            if (!success)
            {
                _logger.LogWarning($"No se pudo actualizar la solicitud para el edificio con código {buildingCode}");
                return NotFound($"No se pudo actualizar la solicitud para el edificio con código {buildingCode}");
            }
            _logger.LogInformation($"Montos actualizados para la request del edificio con código {buildingCode}");
            return NoContent();
        }


        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchRequest(int id, [FromBody] JsonPatchDocument<Request> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest("Se requiere el documento de operaciones PATCH.");

            // Extraer información relevante del patch
            string? comment = null;
            int? statusId = null;
            DateTime? changeDate = null;
            foreach (var op in patchDoc.Operations)
            {
                if (op.path.ToLower() == "/comment" && op.OperationType == OperationType.Replace)
                    comment = op.value?.ToString();
                if (op.path.ToLower() == "/statusid" && op.OperationType == OperationType.Replace && int.TryParse(op.value?.ToString(), out var sid))
                    statusId = sid;
                if (op.path.ToLower() == "/changedate" && op.OperationType == OperationType.Replace && DateTime.TryParse(op.value?.ToString(), out var dt))
                    changeDate = dt;
            }
            if (!statusId.HasValue)
                return BadRequest("Se requiere el campo StatusId en el patch para cambiar el estado.");

            var result = await _mediator.Send(new PatchRequestCommand(id, statusId.Value, comment, changeDate));
            if (result == null)
                return NotFound($"Request con id {id} no encontrada o StatusId inválido");
            if (result == false)
                return BadRequest("El estado ya es el mismo, no se realizó ningún cambio.");
            return Ok(new { RequestId = id, StatusId = statusId });
        }
    }
}
