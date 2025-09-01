using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PrototipoApi.Entities;
using System.Text.Json;

namespace PrototipoApi.Services
{
    public class ExternalBuildingService : IExternalBuildingService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://api.jsonbin.io/v3/b/68a7092543b1c97be92445d8";

        public ExternalBuildingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Building?> GetBuildingByCodeAsync(string buildingCode)
        {
            // Construir la URL con el código de edificio según la nueva especificación
            var url = $"https://172.30.137.209:7124/api/Building/bycode/{buildingCode}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            // Deserializar el JSON directamente a la entidad Building (ignorando BuildingId)
            var building = await response.Content.ReadFromJsonAsync<Building>();
            if (building == null)
                return null;

            // Ignorar BuildingId recibido de la API externa
            building.BuildingId = 0;
            return building;
        }
    }
}
