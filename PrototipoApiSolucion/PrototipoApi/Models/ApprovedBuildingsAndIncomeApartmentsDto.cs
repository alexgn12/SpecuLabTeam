using System.Collections.Generic;
using PrototipoApi.Models;

namespace PrototipoApi.Models
{
    public class ApprovedBuildingsAndIncomeApartmentsDto
    {
        public List<BuildingDto> ApprovedBuildings { get; set; } = new();
        public List<ApartmentDto> IncomeApartments { get; set; } = new();
    }
}
