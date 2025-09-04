namespace PrototipoApi.Services
{
    public class ExternalBuildingDto
    {
        public string buildingName { get; set; } = string.Empty;
        public string constructedAddress { get; set; } = string.Empty;
        public string districtName { get; set; } = string.Empty;
        public int floorCount { get; set; }
        public int yearBuilt { get; set; }
        public int apartmentCount { get; set; }
    }
}
