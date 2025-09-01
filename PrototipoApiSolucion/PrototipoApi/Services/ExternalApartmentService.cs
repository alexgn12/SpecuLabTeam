using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PrototipoApi.Entities;
using System.Text.Json;
using PrototipoApi.Repositories.Interfaces;

namespace PrototipoApi.Services
{
    public class ExternalApartmentService : IExternalApartmentService
    {
        private readonly HttpClient _httpClient;
        private readonly IRepository<Building> _buildings;
        private const string ApiUrl = "https://api.jsonbin.io/v3/b/68b00077d0ea881f4068f70f"; // Cambia esta URL por la real

        public ExternalApartmentService(HttpClient httpClient, IRepository<Building> buildings)
        {
            _httpClient = httpClient;
            _buildings = buildings;
        }

        public async Task<Apartment?> GetApartmentByCodeAsync(string apartmentCode)
        {
            var response = await _httpClient.GetAsync(ApiUrl);
            if (!response.IsSuccessStatusCode)
                return null;
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (root.TryGetProperty("record", out var record))
            {
                foreach (var apartmentElem in record.EnumerateArray())
                {
                    if (apartmentElem.TryGetProperty("ApartmentCode", out var codeElem) && codeElem.GetString() == apartmentCode)
                    {
                        // Obtener el BuildingCode del JSON
                        var buildingCode = apartmentElem.GetProperty("BuildingCode").GetString() ?? string.Empty;
                        // Buscar el BuildingId en la base de datos
                        var building = await _buildings.GetOneAsync(b => b.BuildingCode == buildingCode);
                        int buildingId = building?.BuildingId ?? 0;
                        return new Apartment
                        {
                            ApartmentCode = apartmentElem.GetProperty("ApartmentCode").GetString() ?? string.Empty,
                            ApartmentDoor = apartmentElem.GetProperty("ApartmentDoor").GetString() ?? string.Empty,
                            ApartmentFloor = apartmentElem.GetProperty("ApartmentFloor").GetString() ?? string.Empty,
                            ApartmentPrice = apartmentElem.GetProperty("ApartmentPrice").GetDecimal(),
                            NumberOfRooms = apartmentElem.GetProperty("NumberOfRooms").GetInt32(),
                            NumberOfBathrooms = apartmentElem.GetProperty("NumberOfBathrooms").GetInt32(),
                            BuildingId = buildingId,
                            HasLift = apartmentElem.GetProperty("HasLift").GetBoolean(),
                            HasGarage = apartmentElem.GetProperty("HasGarage").GetBoolean(),
                            CreatedDate = apartmentElem.GetProperty("CreatedDate").GetDateTime()
                        };
                    }
                }
            }
            return null;
        }
    }
}