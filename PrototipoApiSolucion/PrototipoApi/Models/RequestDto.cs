using PrototipoApi.Entities;

namespace PrototipoApi.Models
{
    public class RequestDto
    {
        public int RequestId { get; set; }
        public double BuildingAmount { get; set; }
        public double MaintenanceAmount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string StatusType { get; set; } = null!; // Relación con Status
        public int StatusId { get; set; } // Relación con Status
        public int BuildingId { get; set; } // Relación con Building
    }
}
