namespace PrototipoApi.Models
{
    public class ManagementBudgetDto
    {
        public int ManagementBudgetId { get; set; }
        public double CurrentAmount { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string LastUpdatedDateFormatted => Helpers.DateFormatHelper.ToExternalFormat(LastUpdatedDate);
    }
}
