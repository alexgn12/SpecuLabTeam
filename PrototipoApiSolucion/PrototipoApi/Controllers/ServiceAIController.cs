using GammaAI.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GammaAI.Services;
using PrototipoApi.Models;
using OpenAI.Chat;
using PrototipoApi.BaseDatos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace PrototipoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceAIController : ControllerBase
    {
        private readonly IOpenAIService _openAIService;
        private readonly ContextoBaseDatos _context;
        private readonly ILogger<ServiceAIController> _logger;

        public ServiceAIController(IOpenAIService openAIService, ContextoBaseDatos context, ILogger<ServiceAIController> logger)
        {
            _openAIService = openAIService;
            _context = context;
            _logger = logger;
        }

        [HttpPost("analyze-json")]
        public async Task<ActionResult<Result<string>>> AnalyzeJsonWithPrompt([FromBody] OpenAIAnalysisRequest request)
        {
            try
            {
                var transactions = await _context.Transactions.AsNoTracking().ToListAsync();
                var requests = await _context.Requests.AsNoTracking().ToListAsync();
                var buildings = await _context.Buildings.AsNoTracking().ToListAsync();
                var apartments = await _context.Apartments.AsNoTracking().ToListAsync();

                var data = new { transactions, requests, buildings, apartments };
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
                return Ok(Result<string>.Ok(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en AnalyzeJsonWithPrompt");
                return StatusCode(500, Result<string>.Fail("Error interno del servidor", ex));
            }
        }

        [HttpPost("analyze-building-request")]
        public async Task<ActionResult<Result<string>>> AnalyzeBuildingRequest([FromBody] AnalyzeBuildingRequest incoming)
        {
            try
            {
                var request = await _context.Requests
                    .Include(r => r.Building)
                    .Include(r => r.Building.Apartments)
                    .FirstOrDefaultAsync(r => r.RequestId == incoming.RequestId);
                if (request == null) return NotFound(Result<string>.Fail("Solicitud no encontrada"));

                var budget = await _context.ManagementBudgets
                    .OrderByDescending(b => b.LastUpdatedDate)
                    .FirstOrDefaultAsync();
                if (budget == null) return NotFound(Result<string>.Fail("No hay presupuesto disponible"));

                var apartments = request.Building.Apartments;
                double annualRentalIncome = apartments.Sum(a => (double)a.ApartmentPrice) * 12;
                double totalRequestAmount = request.BuildingAmount + request.MaintenanceAmount;
                double netAnnualIncome = annualRentalIncome - request.MaintenanceAmount;
                double paybackPeriod = netAnnualIncome > 0 ? totalRequestAmount / netAnnualIncome : 0;

                string prompt = $@"Eres un agente inmobiliario financiero experto.
Analiza la siguiente propuesta y toma una decisión con métricas numéricas:

- Edificio: {request.Building.BuildingName}, Distrito: {request.Building.District}
- Solicitud: {request.BuildingAmount} compra + {request.MaintenanceAmount} mantenimiento
- Presupuesto actual: {budget.CurrentAmount}
- Apartamentos: {apartments.Count}, Ingreso alquiler anual estimado: {annualRentalIncome}
- Payback estimado: {paybackPeriod:N2} años

Toma una decisión (COMPRAR o NO COMPRAR), justifica, indica supuestos, riesgos y oportunidades. Devuelve SOLO el siguiente JSON:
{{
  ""requestId"": {request.RequestId},
  ""decision"": ""COMPRAR|NO COMPRAR"",
  ""reason"": ""explicación técnica"",
  ""currentBudget"": {budget.CurrentAmount},
  ""totalRequestAmount"": {totalRequestAmount},
  ""estimatedAnnualRentalIncome"": {annualRentalIncome},
  ""paybackPeriodYears"": {paybackPeriod:N2},
  ""assumptions"": ""Describe los supuestos usados"",
  ""insights"": ""Ideas adicionales y posibles riesgos/oportunidades""
}}".Replace("'", "\"");

                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage("Eres un asesor financiero inmobiliario. Analiza y responde estrictamente con el JSON dado."),
                    new UserChatMessage(prompt)
                };
                var response = await ((OpenAIService)_openAIService).SendCustomChatAsync(messages);
                return Ok(Result<string>.Ok(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en AnalyzeBuildingRequest");
                return StatusCode(500, Result<string>.Fail("Error interno del servidor", ex));
            }
        }
    }
}
