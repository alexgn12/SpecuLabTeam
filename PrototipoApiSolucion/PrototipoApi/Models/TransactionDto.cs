namespace PrototipoApi.Models
{
    public class TransactionDto
    {
        public int TransactionId { get; set; } 
        public DateTime TransactionDate { get; set; }
        public int TransactionTypeId { get; set; } 
        public string TransactionType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Relaciones
        public decimal BuildingAmount { get; set; } // Relación con Request
        public decimal Amount { get; set; }
        public int? ApartmentId { get; set; } // Relación con Apartment
        public int? BuildingId { get; set; } // Relación con Building
        //public int ManagementBudgetId { get; set; }
    }
}
