using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace PrototipoApi.Services
{
    public interface IExternalApiService
    {
        Task PatchBuildingStatusAsync(string buildingCode, string value);
    }

    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;
        public ExternalApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task PatchBuildingStatusAsync(string buildingCode, string value)
        {
            var url = $"https://devdemoapi1.azurewebsites.net/api/building/speculab/status/bycode/{buildingCode}";
            var body = new[] {
                new {
                    path = "/statusid",
                    op = "replace",
                    value = value
                }
            };
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json-patch+json");
            var response = await _httpClient.PatchAsync(url, content);
            response.EnsureSuccessStatusCode();
        }
    }
}
