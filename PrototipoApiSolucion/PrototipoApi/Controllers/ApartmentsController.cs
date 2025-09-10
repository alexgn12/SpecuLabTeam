using MediatR;
using Microsoft.AspNetCore.Mvc;
using PrototipoApi.Models;
using PrototipoApi.Application.Apartments.Queries;
using PrototipoApi.Application.Apartments.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;
using PrototipoApi.Application.Apartments.Queries.GetAllApartments;
using PrototipoApi.Application.Apartments.Queries.GetApartmentById;
using PrototipoApi.Application.Apartments.Commands.CreateApartment;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace PrototipoApi.Controllers
{
    // Controlador para la gestión de apartamentos: consulta, detalle y creación
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ApartmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ApartmentsController> _logger;

        public ApartmentsController(IMediator mediator, ILogger<ApartmentsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // Endpoint para obtener una lista paginada de apartamentos
        // Permite ordenar y filtrar por página, tamaño y campo de orden
        [HttpGet]
        public async Task<ActionResult<Result<List<ApartmentDto>>>> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? orderBy = "CreatedDate", [FromQuery] bool desc = true)
        {
            try
            {
                _logger.LogInformation($"Obteniendo apartamentos. Página: {page}, Tamaño: {size}, Orden: {orderBy}, Desc: {desc}");
                var result = await _mediator.Send(new GetAllApartmentsQuery(page, size, orderBy, desc));
                return Ok(Result<List<ApartmentDto>>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener apartamentos");
                return StatusCode(500, Result<List<ApartmentDto>>.Fail("Error interno del servidor", ex));
            }
        }

        // Endpoint para obtener el detalle de un apartamento por su id
        [HttpGet("{id}")]
        public async Task<ActionResult<Result<ApartmentDto>>> GetById(int id)
        {
            try
            {
                _logger.LogInformation($"Obteniendo apartamento con id {id}");
                var result = await _mediator.Send(new GetApartmentByIdQuery(id));
                if (result == null)
                {
                    _logger.LogWarning($"Apartamento con id {id} no encontrado");
                    return NotFound(Result<ApartmentDto>.Fail("No encontrado"));
                }
                return Ok(Result<ApartmentDto>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener apartamento por id");
                return StatusCode(500, Result<ApartmentDto>.Fail("Error interno del servidor", ex));
            }
        }

        // Endpoint para crear un nuevo apartamento
        // Recibe los datos del apartamento en el cuerpo de la petición
        [HttpPost]
        public async Task<ActionResult<Result<ApartmentDto>>> Create([FromBody] ApartmentDto dto)
        {
            try
            {
                _logger.LogInformation("Creando nuevo apartamento");
                var result = await _mediator.Send(new CreateApartmentCommand(dto));
                _logger.LogInformation($"Apartamento creado con id {result.ApartmentId}");
                return Ok(Result<ApartmentDto>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear apartamento");
                return StatusCode(500, Result<ApartmentDto>.Fail("Error interno del servidor", ex));
            }
        }
    }
}
