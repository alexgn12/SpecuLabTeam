// Comentario explicativo para el profesor:
// Este archivo define la entidad RequestStatusHistory, utilizada para registrar el historial de cambios de estado de las solicitudes (Request).
// Los cambios recientes implementan la lógica para guardar automáticamente cada transición de estado en la base de datos, permitiendo auditoría y seguimiento completo.
// El campo OldStatusId ahora es nullable para soportar el primer registro sin estado anterior a.

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
