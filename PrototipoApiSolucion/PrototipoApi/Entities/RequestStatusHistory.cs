using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrototipoApi.Entities
{
    public class RequestStatusHistory
    {
        public int RequestStatusHistoryId { get; set; }
        public int RequestId { get; set; }
        [ForeignKey("RequestId")]
        public Request Request { get; set; } = null!;
        public int? OldStatusId { get; set; } // Ahora es nullable
        [ForeignKey("OldStatusId")]
        public Status? OldStatus { get; set; } // Ahora es nullable
        public int NewStatusId { get; set; }
        [ForeignKey("NewStatusId")]
        public Status NewStatus { get; set; } = null!;
        public DateTime ChangeDate { get; set; }
        public string? Comment { get; set; }
    }
}
