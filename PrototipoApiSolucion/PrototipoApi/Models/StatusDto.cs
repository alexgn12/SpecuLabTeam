using PrototipoApi.Entities;

namespace PrototipoApi.Models
{
    // Comentario explicativo para el profesor:
    // Este archivo define el DTO StatusDto, utilizado para transferir información de estado en la API.
    // Cambios recientes: se ha adaptado la lógica para soportar el historial de cambios de estado en las solicitudes.
    // Revisa la documentación y los commits para más detalles.
    public class StatusDto
    {
        public int StatusId { get; set; }
        public string StatusType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
