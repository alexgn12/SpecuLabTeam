using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PrototipoApi.Entities;
using System.Text.Json;

namespace PrototipoApi.Services
{
    public class ExternalApartmentService : IExternalApartmentService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://api.jsonbin.io/v3/b/68b00077d0ea881f4068f70f"; // Cambia esta URL por la real

        public ExternalApartmentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
                        return new Apartment
                        {
                            ApartmentCode = apartmentElem.GetProperty("ApartmentCode").GetString() ?? string.Empty,
                            ApartmentDoor = apartmentElem.GetProperty("ApartmentDoor").GetString() ?? string.Empty,
                            ApartmentFloor = apartmentElem.GetProperty("ApartmentFloor").GetString() ?? string.Empty,
                            ApartmentPrice = apartmentElem.GetProperty("ApartmentPrice").GetDecimal(),
                            NumberOfRooms = apartmentElem.GetProperty("NumberOfRooms").GetInt32(),
                            NumberOfBathrooms = apartmentElem.GetProperty("NumberOfBathrooms").GetInt32(),
                            BuildingId = apartmentElem.GetProperty("BuildingId").GetInt32(),
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
