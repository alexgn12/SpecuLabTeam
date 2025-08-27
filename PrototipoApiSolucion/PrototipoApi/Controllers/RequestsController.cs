using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrototipoApi.Application.Requests.Commands.CreateRequest;
using PrototipoApi.Application.Requests.Commands.UpdateRequest;
using PrototipoApi.Application.Requests.Queries.GetRequestById;
using PrototipoApi.Application.Requests.Commands.UpdateRequestStatus;
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

    public RequestsController(IMediator mediator, ILoguer loguer)
    {
        _mediator = mediator;
        _loguer = loguer;
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

    [HttpPut("{id}/amounts")]
    public async Task<IActionResult> UpdateAmounts(int id, [FromBody] UpdateRequestDto dto)
    {
        _loguer.LogInfo($"Actualizando montos de la request con id {id}");
        var success = await _mediator.Send(new UpdateRequestCommand(id, dto));

        if (!success)
        {
            _loguer.LogWarning($"No se encontró la solicitud con ID {id} para actualizar");
            return NotFound($"No se encontró la solicitud con ID {id}");
        }

        _loguer.LogInfo($"Montos actualizados para la request con id {id}");
        return NoContent();
    }

    [HttpPut("by-buildingcode/{buildingCode}/amounts")]
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

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateRequestStatusDto dto)
    {
        _loguer.LogInfo($"Actualizando status de la request con id {id} a statusType {dto.StatusType}");
        var success = await _mediator.Send(new UpdateRequestStatusCommand(id, dto.StatusType, dto.Comment, dto.ChangeDate));
        if (success == null)
        {
            _loguer.LogWarning($"No se encontró la solicitud o el status con ID {id}/{dto.StatusType} para actualizar");
            return NotFound($"No se encontró la solicitud o el status con ID {id}/{dto.StatusType}");
        }
        if (success == false)
        {
            _loguer.LogWarning($"El estado actual ya es '{dto.StatusType}', no se realizó ningún cambio");
            return BadRequest($"El estado actual ya es '{dto.StatusType}', no se realizó ningún cambio");
        }
        _loguer.LogInfo($"Status actualizado para la request con id {id}");
        return NoContent();
    }
}
