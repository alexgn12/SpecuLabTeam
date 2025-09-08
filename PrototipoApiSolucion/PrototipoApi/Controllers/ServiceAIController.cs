using GammaAI.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GammaAI.Services;
using PrototipoApi.Models;
using OpenAI.Chat;
using PrototipoApi.BaseDatos;
using Microsoft.EntityFrameworkCore;

namespace PrototipoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceAIController : ControllerBase
    {
        private readonly IOpenAIService _openAIService;
        private readonly ContextoBaseDatos _context;

        public ServiceAIController(IOpenAIService openAIService, ContextoBaseDatos context)
        {
            _openAIService = openAIService;
            _context = context;
        }

        [HttpPost("analyze-json")]
        public async Task<IActionResult> AnalyzeJsonWithPrompt([FromBody] OpenAIAnalysisRequest request)
        {
            // Obtén los datos de las tablas
            var transactions = await _context.Transactions.AsNoTracking().ToListAsync();
            var requests = await _context.Requests.AsNoTracking().ToListAsync();
            var buildings = await _context.Buildings.AsNoTracking().ToListAsync();
            var apartments = await _context.Apartments.AsNoTracking().ToListAsync();

            // Serializa a JSON
            var data = new
            {
                transactions,
                requests,
                buildings,
                apartments
            };
            var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            var systemPrompt = string.IsNullOrWhiteSpace(request.SystemPrompt)
                ? "Eres un analista financiero. Tu trabajo es analizar los datos en formato JSON que se te proporcionan. " +
                "Debes responder únicamente en base a esos datos, calculando métricas como beneficios, gastos, ingresos o cualquier información solicitada en el prompt del usuario. " +
                "No inventes información fuera de los datos."
                : request.SystemPrompt;

            var userPrompt = $"{request.UserPrompt}\n\nDatos JSON:\n{jsonData}";

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(userPrompt)
            };

            var response = await ((OpenAIService)_openAIService).SendCustomChatAsync(messages);

            return Ok(new { answer = response });
        }
    }
}
