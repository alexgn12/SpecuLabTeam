namespace PrototipoApi.Models
{
    public class TransactionDto
    {
        public int TransactionId { get; set; } 
        public DateTime TransactionDate { get; set; }
        public string TransactionDateFormatted => Helpers.DateFormatHelper.ToExternalFormat(TransactionDate);
        public int TransactionTypeId { get; set; } 
        public string TransactionType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Relaciones
        public decimal BuildingAmount { get; set; } // Relación con Request
        public decimal Amount { get; set; }
        //public int ManagementBudgetId { get; set; }
    }
}
