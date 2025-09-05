using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using PrototipoApi.Application.Requests.Commands.CreateRequest;
using PrototipoApi.Application.Requests.Commands.UpdateRequest;
using PrototipoApi.Application.Requests.Commands.PatchRequest;
using PrototipoApi.Application.Requests.Queries.GetRequestById;
using PrototipoApi.Application.Requests.Queries.GetRequestByBuildingCode;
using PrototipoApi.BaseDatos;
using PrototipoApi.Entities;
using PrototipoApi.Models;
using PrototipoApi.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

[Route("api/requests")]
[ApiController]
public class RequestsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILoguer _loguer;
    private readonly ContextoBaseDatos _context;

    public RequestsController(IMediator mediator, ILoguer loguer, ContextoBaseDatos context)
    {
        _mediator = mediator;
        _loguer = loguer;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<RequestDto>>> Get([FromQuery] GetAllRequestsQuery query)
    {
        _loguer.LogInfo("Obteniendo todas las requests");
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RequestDto>> GetById(int id)
    {
        _loguer.LogInfo($"Obteniendo request con id {id}");
        var result = await _mediator.Send(new GetRequestByIdQuery(id));
        if (result == null)
        {
            _loguer.LogWarning($"Request con id {id} no encontrada");
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<RequestDto>> CreateRequest([FromBody] CreateRequestDto dto)
    {
        _loguer.LogInfo("Creando nueva request");
        var result = await _mediator.Send(new CreateRequestCommand(dto));
        _loguer.LogInfo($"Request creada con id {result.RequestId}");
        return CreatedAtAction(nameof(GetById), new { id = result.RequestId }, result);
    }

    [HttpPut("{buildingCode}/amounts")]
    public async Task<IActionResult> UpdateAmountsByBuildingCode(string buildingCode, [FromBody] UpdateRequestDto dto)
    {
        _loguer.LogInfo($"Actualizando montos de la request para el edificio con código {buildingCode}");
        // Buscar la última request asociada a ese BuildingCode
        var request = await _mediator.Send(new GetRequestByBuildingCodeQuery(buildingCode));
        if (request == null)
        {
            _loguer.LogWarning($"No se encontró ninguna solicitud para el edificio con código {buildingCode}");
            return NotFound($"No se encontró ninguna solicitud para el edificio con código {buildingCode}");
        }
        var success = await _mediator.Send(new UpdateRequestCommand(request.RequestId, dto));
        if (!success)
        {
            _loguer.LogWarning($"No se pudo actualizar la solicitud para el edificio con código {buildingCode}");
            return NotFound($"No se pudo actualizar la solicitud para el edificio con código {buildingCode}");
        }
        _loguer.LogInfo($"Montos actualizados para la request del edificio con código {buildingCode}");
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
