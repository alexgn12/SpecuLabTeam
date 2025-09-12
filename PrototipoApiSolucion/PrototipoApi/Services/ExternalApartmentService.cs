using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PrototipoApi.Entities;
using PrototipoApi.Models;
using PrototipoApi.Repositories.Interfaces;
using System.Text.Json;

namespace PrototipoApi.Services
{
    public class ExternalApartmentService : IExternalApartmentService
    {
        private readonly HttpClient _httpClient;
        private readonly IRepository<Building> _buildingRepository;
        private const string ApiUrl = "https://api.jsonbin.io/v3/b/68b00077d0ea881f4068f70f"; // Cambia esta URL por la real

        public ExternalApartmentService(HttpClient httpClient, IRepository<Building> buildingRepository)
        {
            _httpClient = httpClient;
            _buildingRepository = buildingRepository;
        }

        public async Task<ApartmentDto?> GetApartmentByCodeAsync(string apartmentCode)
        {
            // Construir la URL con el código de apartamento
            var url = $"https://devdemoapi4.azurewebsites.net/api/ELApartmentGet/{apartmentCode}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            // Deserializar el JSON directamente a un objeto dinámico para extraer buildingCode
            var apartmentJson = await response.Content.ReadAsStringAsync();
            var apartmentData = JsonSerializer.Deserialize<JsonElement>(apartmentJson);
            if (!apartmentData.TryGetProperty("buildingCode", out var buildingCodeElement))
                return null;
            var buildingCode = buildingCodeElement.GetString();
            if (string.IsNullOrEmpty(buildingCode))
                return null;

            // Buscar el BuildingId usando el buildingCode
            var building = await _buildingRepository.GetOneAsync(b => b.BuildingCode == buildingCode);
            if (building == null)
                return null;

            // Mapear los datos recibidos al ApartmentDto
            var dto = new ApartmentDto
            {
                ApartmentId = 0, // Ignorar el id externo
                ApartmentCode = apartmentData.GetProperty("apartmentCode").GetString() ?? string.Empty,
                ApartmentDoor = apartmentData.GetProperty("apartmentDoor").GetString() ?? string.Empty,
                ApartmentFloor = apartmentData.GetProperty("apartmentFloor").GetString() ?? string.Empty,
                ApartmentPrice = apartmentData.GetProperty("apartmentPrice").GetDecimal(),
                NumberOfRooms = apartmentData.GetProperty("numberOfRooms").GetInt32(),
                NumberOfBathrooms = apartmentData.GetProperty("numberOfBathrooms").GetInt32(),
                BuildingCode = buildingCode,
                HasLift = apartmentData.GetProperty("hasLift").GetBoolean(),
                HasGarage = apartmentData.GetProperty("hasGarage").GetBoolean(),
                CreatedDate = apartmentData.GetProperty("createdDate").GetDateTime()
            };
            return dto;
        }
    }
}