using MediatR;
using System.Collections.Generic;

namespace PrototipoApi.Application.AngController.Query
{
    public record GastoMensualPorTipoQuery() : IRequest<List<GastoMensualPorTipoDto>>;

    public class GastoMensualPorTipoDto
    {
        public int Año { get; set; }
        public int Mes { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public double TotalGasto { get; set; }
    }
}
