using MediatR;
using Microsoft.AspNetCore.Mvc;
using PrototipoApi.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using PrototipoApi.Application.Transaction.Queries.GetAllTransaction.ListResult;

namespace PrototipoApi.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(IMediator mediator, ILogger<TransactionsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<ActionResult<object>> GetTransactions(
            [FromQuery] string? transactionType,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] int? year = null,
            [FromQuery] int? month = null)
        {
            _logger.LogInformation($"Obteniendo transacciones. Tipo: {transactionType}, Página: {page}, Tamaño: {size}, Año: {year}, Mes: {month}");
            var query = new GetAllTransactionsQuery(transactionType, page, size, year, month);
            var result = await _mediator.Send(query);
            return Ok(new { items = result.Items, total = result.Total });
        }

        // GET: api/transactions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
        {
            _logger.LogInformation($"Obteniendo transacción con id {id}");
            var transaction = await _mediator.Send(new GetTransactionByIdQuery(id));
            if (transaction == null)
            {
                _logger.LogWarning($"Transacción con id {id} no encontrada");
                return NotFound();
            }
            return Ok(transaction);
        }

        // POST: api/transactions
        [HttpPost]
        public async Task<ActionResult<TransactionDto>> CreateTransaction([FromBody] CreateTransactionDto dto)
        {
            _logger.LogInformation("Creando nueva transacción");
            var command = new CreateTransactionCommand(
                DateTime.UtcNow, // O usa un campo de fecha si lo agregas al DTO
                dto.Description,
                dto.Amount,
                dto.ApartmentCode
            );
            var createdTransaction = await _mediator.Send(command);
            _logger.LogInformation($"Transacción creada con id {createdTransaction.TransactionId}");
            return CreatedAtAction(nameof(GetTransaction), new { id = createdTransaction.TransactionId }, createdTransaction);
        }

        private async Task<int> GetIngresoTransactionTypeId()
        {
            // Aquí deberías obtener el TransactionTypeId correspondiente a "INGRESO" desde la base de datos o un servicio
            // Por simplicidad, se retorna 1, pero deberías reemplazarlo por la lógica real
            return 1;
        }

    }
}
