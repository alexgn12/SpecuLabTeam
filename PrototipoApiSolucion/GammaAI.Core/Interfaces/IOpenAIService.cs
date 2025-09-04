using GammaAI.Core.Models;

namespace GammaAI.Core.Interfaces
{
    public interface IOpenAIService
    {
        Task<string> GenerateSQLQueryAsync(string naturalLanguageQuery, string databaseSchema);
        Task<string> GenerateUserFriendlyResponseAsync(string originalQuery, string sqlQuery, string queryResults);
        Task<List<ChatMessage>> GetChatHistoryAsync();
        Task AddMessageToChatAsync(ChatMessage message);
        Task ClearChatHistoryAsync();
    }
}
