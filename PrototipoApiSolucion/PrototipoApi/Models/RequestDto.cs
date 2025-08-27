using PrototipoApi.Entities;

namespace PrototipoApi.Models
{
    public class RequestDto
    {
        public int RequestId { get; set; }
        public double BuildingAmount { get; set; }
        public double MaintenanceAmount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string RequestDateFormatted => Helpers.DateFormatHelper.ToExternalFormat(RequestDate);
        public int StatusId { get; set; }
        public string StatusType { get; set; } = string.Empty;
        public int BuildingId { get; set; }
        public string? BuildingStreet { get; set; }
    }
}
