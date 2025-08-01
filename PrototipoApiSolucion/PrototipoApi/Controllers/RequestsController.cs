﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrototipoApi.BaseDatos;
using PrototipoApi.Models;
using PrototipoApi.Entities;
using System;

[Route("api/requests")]
[ApiController]
public class RequestsController : ControllerBase
{
    private readonly ContextoBaseDatos _context;

    public RequestsController(ContextoBaseDatos context)
    {
        _context = context;
    }

    // GET: api/requests = Listar todas las solicitudes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Request>>> GetRequests()
    {
        return await _context.Requests
            .ToListAsync();
    }

    // GET: api/requests/{id} = Obtener una solicitud por ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Request>> GetRequest(int id)
    {
        var request = await _context.Requests.FindAsync(id);
        if (request == null)
        {
            return NotFound();
        }
        return request;
    }


    // POST: api/requests = Crear una nueva solicitud
    [HttpPost]
    public async Task<ActionResult<Request>> CreateRequest(CreateRequestDto dto)
    {
        // Crear el Status relacionado
        var status = new Status
        {
            StatusType = dto.Status.StatusType,
            Description = dto.Status.Description
            // No asignes StatusId
        };

        var request = new Request
        {
            RequestType = dto.RequestType,
            Description = dto.Description,
            RequestDate = DateTime.UtcNow,
            RequestAmount = dto.RequestAmount,
            Status = status // Relación directa
            // No asignes RequestId
        };

        _context.Requests.Add(request);
        await _context.SaveChangesAsync();

        return Created("", $"La solicitud de compra se ha creado correctamente con id {request.RequestId} Puede consultar su estado enviando una petición GET con ese ID");
    }
}
