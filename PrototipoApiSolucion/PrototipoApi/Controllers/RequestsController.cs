using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
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

        var request = await _context.Requests.Include(r => r.Status).FirstOrDefaultAsync(r => r.RequestId == id);
        if (request == null)
            return NotFound($"Request con id {id} no encontrada");

        var oldStatusId = request.StatusId;
        string? comment = null;

        // Extrae y elimina la operación /comment del patch
        var commentOp = patchDoc.Operations.FirstOrDefault(op => op.path.ToLower() == "/comment" && op.OperationType == OperationType.Replace);
        if (commentOp != null)
        {
            comment = commentOp.value?.ToString();
            patchDoc.Operations.Remove(commentOp); // Elimina la operación para evitar el error
        }

        // Detecta si hay un replace a /statusid con valor 3
        var statusReplaceOp = patchDoc.Operations.FirstOrDefault(op => op.path.ToLower() == "/statusid" && op.OperationType == OperationType.Replace && op.value != null && int.TryParse(op.value.ToString(), out var v) && v == 3);
        bool aprobar = statusReplaceOp != null;

        patchDoc.ApplyTo(request, (error) =>
        {
            ModelState.AddModelError(error.AffectedObject?.ToString() ?? "patch", error.ErrorMessage);
        });
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var status = await _context.Statuses.FindAsync(request.StatusId);
        if (status == null)
            return NotFound($"Status con id '{request.StatusId}' no encontrado");

        var history = new RequestStatusHistory
        {
            RequestId = request.RequestId,
            OldStatusId = oldStatusId,
            NewStatusId = request.StatusId,
            ChangeDate = DateTime.UtcNow,
            Comment = comment
        };
        _context.RequestStatusHistories.Add(history);

        _context.Requests.Update(request);
        await _context.SaveChangesAsync();

        // Si se aprobó (statusid == 3), crear transacción de gasto
        if (aprobar)
        {
            var gastoType = await _context.TransactionsTypes.FirstOrDefaultAsync(t => t.TransactionName == "GASTO");
            if (gastoType == null)
            {
                gastoType = new TransactionType { TransactionName = "GASTO" };
                _context.TransactionsTypes.Add(gastoType);
                await _context.SaveChangesAsync();
            }
            var amount = request.BuildingAmount + request.MaintenanceAmount;
            var transaction = new PrototipoApi.Entities.Transaction
            {
                RequestId = request.RequestId,
                TransactionTypeId = gastoType.TransactionTypeId,
                TransactionsType = gastoType,
                TransactionDate = DateTime.UtcNow,
                Amount = amount,
                Description = $"Gasto generado automáticamente al aprobar la solicitud {request.RequestId}"
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        // Devuelve siempre el ID del historial creado
        return Ok(new { RequestStatusHistoryId = history.RequestStatusHistoryId });
    }
}
