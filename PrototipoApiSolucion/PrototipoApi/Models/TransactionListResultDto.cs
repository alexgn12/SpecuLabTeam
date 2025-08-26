using System.Collections.Generic;

namespace PrototipoApi.Models
{
    public class TransactionListResultDto
    {
        public List<TransactionDto> Items { get; set; } = new();
        public int Total { get; set; }
    }
}
