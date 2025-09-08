namespace PrototipoApi.Models
{
    public class AnalyzeBuildingRequest
    {
        public int RequestId { get; set; }
    }

    public class BuildingAnalysisResult
    {
        public int RequestId { get; set; }
        public string Decision { get; set; } = "";
        public string Reason { get; set; } = "";
        public double CurrentBudget { get; set; }
        public double TotalRequestAmount { get; set; }
        public double EstimatedAnnualRentalIncome { get; set; }
        public double PaybackPeriodYears { get; set; }
        public string Assumptions { get; set; } = "";
        public string Insights { get; set; } = "";
    }

}
