namespace GammaAI.Core.DTOs
{
    public class QueryRequest
    {
        public string UserQuery { get; set; } = string.Empty;
        public string? Context { get; set; }
        public bool IncludeSchema { get; set; } = true;
    }

    public class QueryResponse
    {
        public string OriginalQuery { get; set; } = string.Empty;
        public string GeneratedSQL { get; set; } = string.Empty;
        public string Results { get; set; } = string.Empty;
        public string UserFriendlyResponse { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
