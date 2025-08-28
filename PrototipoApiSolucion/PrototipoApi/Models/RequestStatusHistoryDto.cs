using System;

namespace PrototipoApi.Models
{
    public class RequestStatusHistoryDto
    {
        public int RequestId { get; set; }
        public string RequestDescription { get; set; } = string.Empty;
        public int? OldStatusId { get; set; }
        public string? OldStatusType { get; set; }
        public int NewStatusId { get; set; }
        public string NewStatusType { get; set; } = string.Empty;
        public DateTime ChangeDate { get; set; }
        public string? Comment { get; set; }
    }
}
