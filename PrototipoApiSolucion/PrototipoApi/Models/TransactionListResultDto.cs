using System.Collections.Generic;
using PrototipoApi.Helpers;

namespace PrototipoApi.Models
{
    public class TransactionListResultDto
    {
        public List<TransactionDto> Items { get; set; } = new();
        public int Total { get; set; }
        public int TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionDateFormatted => DateFormatHelper.ToExternalFormat(TransactionDate);
        public int TransactionTypeId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BuildingAmount { get; set; }
        public decimal Amount { get; set; }
    }
}
