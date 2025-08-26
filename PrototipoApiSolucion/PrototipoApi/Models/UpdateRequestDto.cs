using System.ComponentModel.DataAnnotations;

namespace PrototipoApi.Models
{
    // Comentario explicativo para el profesor:
    // Este archivo define el DTO UpdateRequestDto, utilizado para actualizar solicitudes.
    // Cambios recientes: se añadió la propiedad NewStatusId para permitir el cambio de estado y registrar el historial.
    // Revisa la documentación y los commits para más detalles.
    public class UpdateRequestDto
    {
        // Campo requerido
        // La cantidad debe ser un número positivo
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero.")]
        public double MaintenanceAmount { get; set; }

        // Nuevo campo para el cambio de estado
        public int NewStatusId { get; set; } // 0 si no se cambia el estado
    }
}
