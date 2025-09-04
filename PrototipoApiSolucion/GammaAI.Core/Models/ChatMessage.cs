namespace GammaAI.Core.Models
{
    public class ChatMessage
    {
        public string Role { get; set; } = string.Empty; // "system", "user", "assistant"
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
