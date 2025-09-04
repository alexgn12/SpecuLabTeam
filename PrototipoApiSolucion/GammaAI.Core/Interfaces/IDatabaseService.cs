namespace GammaAI.Core.Interfaces
{
    public interface IDatabaseService
    {
        Task<string> GetDatabaseSchemaAsync();
        Task<string> ExecuteQueryAsync(string sqlQuery);
        Task<bool> ValidateQueryAsync(string sqlQuery);
        Task SeedDatabaseAsync();
    }
}
