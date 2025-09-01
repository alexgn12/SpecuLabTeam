using System.ComponentModel.DataAnnotations.Schema;

namespace PrototipoApi.Entities
{
    public class ManagementBudget
    {
        public int ManagementBudgetId { get; set; }
        public double CurrentAmount { get; set; }
        public DateTime LastUpdatedDate { get; set; }

    }
}
