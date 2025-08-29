namespace PrototipoApi.Models
{
    public class BuildingDto
    {
        public int BuildingId { get; set; }
        public string BuildingCode { get; set; }
        public string BuildingName { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public int FloorCount { get; set; }
        public int YearBuilt { get; set; }
        public int ApartmentCount { get; set; }
    }
}
