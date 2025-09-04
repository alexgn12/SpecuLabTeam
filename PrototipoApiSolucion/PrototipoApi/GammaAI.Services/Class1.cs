using OpenAI.Chat;
using GammaAI.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace GammaAI.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly ChatClient _chatClient;
        private readonly List<GammaAI.Core.Models.ChatMessage> _chatHistory;
        private readonly string _model;

        public OpenAIService(IConfiguration configuration)
        {
            var apiKey = configuration["OpenAI:ApiKey"] ?? 
                throw new InvalidOperationException("OpenAI API Key not found in configuration.");
            _model = configuration["OpenAI:Model"] ?? "gpt-4";
            _chatClient = new ChatClient(_model, apiKey);
            _chatHistory = new List<GammaAI.Core.Models.ChatMessage>();
        }

        public async Task<string> GenerateSQLQueryAsync(string naturalLanguageQuery, string databaseSchema)
        {
            var systemPrompt = $@"
Eres un experto en SQL y bases de datos. Tu trabajo es convertir preguntas en lenguaje natural a consultas SQL precisas.

ESQUEMA DE LA BASE DE DATOS:
{databaseSchema}

REGLAS IMPORTANTES:
1. SOLO genera consultas SELECT (no INSERT, UPDATE, DELETE, DROP, etc.)
2. Usa ÚNICAMENTE las tablas y columnas que existen en el esquema
3. Devuelve SOLO la consulta SQL, sin explicaciones adicionales
4. Usa nombres de tablas y columnas exactamente como aparecen en el esquema
5. Si la pregunta no se puede responder con los datos disponibles, devuelve: 'NO_DATA_AVAILABLE'
6. Usa JOINs cuando sea necesario para relacionar tablas
7. Incluye ORDER BY cuando sea apropiado para resultados más útiles

EJEMPLOS:
Pregunta: ""¿Cuántos productos hay?""
SQL: SELECT COUNT(*) as TotalProductos FROM Products

Pregunta: ""¿Cuáles son los productos más caros?""  
SQL: SELECT TOP 5 Name, Price FROM Products ORDER BY Price DESC

Pregunta: ""¿Qué categorías tenemos?""
SQL: SELECT Name, Description FROM Categories ORDER BY Name
";

            var messages = new List<OpenAI.Chat.ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage($"Convierte esta pregunta a SQL: {naturalLanguageQuery}")
            };

            var response = await _chatClient.CompleteChatAsync(messages);
            var sqlQuery = response.Value.Content[0].Text.Trim();

            // Registrar en el historial
            await AddMessageToChatAsync(new GammaAI.Core.Models.ChatMessage
            {
                Role = "user",
                Content = naturalLanguageQuery,
                Timestamp = DateTime.Now
            });

            await AddMessageToChatAsync(new GammaAI.Core.Models.ChatMessage
            {
                Role = "assistant",
                Content = $"SQL generado: {sqlQuery}",
                Timestamp = DateTime.Now
            });

            return sqlQuery;
        }

        public async Task<string> GenerateUserFriendlyResponseAsync(string originalQuery, string sqlQuery, string queryResults)
        {
            var systemPrompt = @"
Eres un asistente que ayuda a explicar resultados de bases de datos de manera amigable y comprensible.
Tu trabajo es tomar los resultados de una consulta SQL y presentarlos de forma clara y útil al usuario.

INSTRUCCIONES:
1. Responde en español
2. Sé conciso pero informativo
3. Si hay muchos resultados, resume los más importantes
4. Si no hay resultados, explica por qué y sugiere alternativas
5. Usa un tono amigable y profesional
6. No muestres la consulta SQL al usuario final
";

            var userPrompt = $@"
Pregunta original del usuario: {originalQuery}
Consulta SQL ejecutada: {sqlQuery}
Resultados obtenidos: {queryResults}

Por favor, presenta estos resultados de manera amigable y comprensible para el usuario.
";

            var messages = new List<OpenAI.Chat.ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(userPrompt)
            };

            var response = await _chatClient.CompleteChatAsync(messages);
            var friendlyResponse = response.Value.Content[0].Text.Trim();

            // Registrar respuesta final en el historial
            await AddMessageToChatAsync(new GammaAI.Core.Models.ChatMessage
            {
                Role = "assistant",
                Content = friendlyResponse,
                Timestamp = DateTime.Now
            });

            return friendlyResponse;
        }

        public async Task<System.Collections.Generic.List<GammaAI.Core.Models.ChatMessage>> GetChatHistoryAsync()
        {
            return await Task.FromResult(_chatHistory.ToList());
        }

        public async Task AddMessageToChatAsync(GammaAI.Core.Models.ChatMessage message)
        {
            _chatHistory.Add(message);
            await Task.CompletedTask;
        }

        public async Task ClearChatHistoryAsync()
        {
            _chatHistory.Clear();
            await Task.CompletedTask;
        }
    }
}
