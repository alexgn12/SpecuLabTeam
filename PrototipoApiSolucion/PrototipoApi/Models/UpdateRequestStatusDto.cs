namespace PrototipoApi.Models
{
    public class UpdateRequestStatusDto
    {
        public string StatusType { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public DateTime? ChangeDate { get; set; }
    }
}
