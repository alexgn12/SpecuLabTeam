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
            var response = await _httpClient.GetAsync(ApiUrl);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (root.TryGetProperty("record", out var record))
            {
                foreach (var buildingElem in record.EnumerateArray())
                {
                    if (buildingElem.TryGetProperty("BuildingCode", out var codeElem) && codeElem.GetString() == buildingCode)
                    {
                        return new Building
                        {
                            BuildingCode = buildingElem.GetProperty("BuildingCode").GetString() ?? string.Empty,
                            BuildingName = buildingElem.GetProperty("BuildingName").GetString() ?? string.Empty,
                            Street = buildingElem.GetProperty("Street").GetString() ?? string.Empty,
                            District = buildingElem.GetProperty("District").GetString() ?? string.Empty,
                            CreatedDate = buildingElem.GetProperty("CreatedDate").GetDateTime(),
                            FloorCount = buildingElem.GetProperty("FloorCount").GetInt32(),
                            YearBuilt = buildingElem.GetProperty("YearBuilt").GetInt32()
                        };
                    }
                }
            }
            return null;
        }
    }
}