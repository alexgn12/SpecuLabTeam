using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PrototipoApi.Entities;
using System.Text.Json;
using AutoMapper;

namespace PrototipoApi.Services
{
    public class ExternalBuildingService : IExternalBuildingService
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public ExternalBuildingService(HttpClient httpClient, IMapper mapper)
        {
            _httpClient = httpClient;
            _mapper = mapper;
        }

        public async Task<Building?> GetBuildingByCodeAsync(string buildingCode)
        {
            var url = $"https://devdemoapi1.azurewebsites.net/api/Building/bycode/{buildingCode}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var externalDto = await response.Content.ReadFromJsonAsync<ExternalBuildingDto>();
            if (externalDto == null)
                return null;

            var building = _mapper.Map<Building>(externalDto);
            building.BuildingCode = buildingCode;
            return building;
        }
    }
}