using MediatR;
using Microsoft.AspNetCore.Mvc;
using PrototipoApi.Models;
using PrototipoApi.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrototipoApi.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoguer _loguer;

        public TransactionsController(IMediator mediator, ILoguer loguer)
        {
            _mediator = mediator;
            _loguer = loguer;
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<ActionResult<List<TransactionDto>>> GetTransactions(
            [FromQuery] string? transactionType,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] int? year = null,
            [FromQuery] int? month = null)
        {
            _loguer.LogInfo($"Obteniendo transacciones. Tipo: {transactionType}, Página: {page}, Tamaño: {size}, Año: {year}, Mes: {month}");
            var query = new GetAllTransactionsQuery(transactionType, page, size, year, month);
            var transactions = await _mediator.Send(query);
            return Ok(transactions);
        }

        // GET: api/transactions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
        {
            _loguer.LogInfo($"Obteniendo transacción con id {id}");
            var transaction = await _mediator.Send(new GetTransactionByIdQuery(id));
            if (transaction == null)
            {
                _loguer.LogWarning($"Transacción con id {id} no encontrada");
                return NotFound();
            }
            return Ok(transaction);
        }

        // POST: api/transactions
        [HttpPost]
        public async Task<ActionResult<TransactionDto>> CreateTransaction(TransactionDto dto)
        {
            _loguer.LogInfo("Creando nueva transacción");
            var createdTransaction = await _mediator.Send(
                new CreateTransactionCommand(dto.TransactionDate, dto.TransactionTypeId, dto.RequestId)
            );
            _loguer.LogInfo($"Transacción creada con id {createdTransaction.TransactionId}");
            return CreatedAtAction(nameof(GetTransaction), new { id = createdTransaction.TransactionId }, createdTransaction);
        }

    }

}
