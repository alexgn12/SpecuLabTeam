using System.Collections.Generic;
namespace PrototipoApi.Entities
{
    public class Building
    {
        public int BuildingId { get; set; }
        public string BuildingCode { get; set; }
        public string BuildingName { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public int FloorCount { get; set; }
        public int YearBuilt { get; set; }
        public string ApartmentCount { get; set; } = string.Empty;

        // Relación inversa: un edificio tiene muchos apartamentos
        public ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
    }
}