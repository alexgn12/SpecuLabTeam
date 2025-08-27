using MediatR;
using System.Threading;
using System.Threading.Tasks;
using PrototipoApi.BaseDatos;
using Microsoft.EntityFrameworkCore;

namespace PrototipoApi.Application.AngController.Query
{
    public class GetEdificiosCompradosCountQuery : IRequest<int> { }

    public class Handler : IRequestHandler<GetEdificiosCompradosCountQuery, int>
    {
        private readonly ContextoBaseDatos _context;
        public Handler(ContextoBaseDatos context)
        {
            _context = context;
        }

        public async Task<int> Handle(GetEdificiosCompradosCountQuery request, CancellationToken cancellationToken)
        {
            // Cuenta la cantidad de transacciones con tipo 'GASTO'
            return await _context.Transactions.CountAsync(t => t.TransactionsType.TransactionName == "GASTO", cancellationToken);
        }
    }
}
