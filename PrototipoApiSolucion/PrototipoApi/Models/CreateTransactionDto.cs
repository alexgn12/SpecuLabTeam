namespace PrototipoApi.Models
{
    public class CreateTransactionDto
    {
        // public string TransactionType { get; set; } = string.Empty; // Eliminado para forzar INGRESO
        public string Description { get; set; } = string.Empty;
        // Relaciones
        public string ApartmentCode { get; set; } = string.Empty; // Relación con Request
        public decimal Amount { get; set; }
        //public int ManagementBudgetId { get; set; }
    }
}
