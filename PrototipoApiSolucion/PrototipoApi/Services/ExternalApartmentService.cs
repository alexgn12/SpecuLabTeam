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
            // Construir la URL con el código de apartamento
            var url = $"https://devdemoapi4.azurewebsites.net/api/ELApartmentGet/{apartmentCode}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            // Deserializar el JSON directamente a la entidad Apartment (ignorando ApartmentId)
            var apartment = await response.Content.ReadFromJsonAsync<Apartment>();
            if (apartment == null)
                return null;

            // Ignorar ApartmentId recibido de la API externa
            apartment.ApartmentId = 0;
            return apartment;
        }
    }
}
