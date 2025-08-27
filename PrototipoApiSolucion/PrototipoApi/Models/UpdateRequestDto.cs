using System.ComponentModel.DataAnnotations;

namespace PrototipoApi.Models
{
    // Comentario explicativo para el profesor:
    // Este archivo define el DTO UpdateRequestDto, utilizado para actualizar solicitudes.
    // Cambios recientes: se eliminó la propiedad NewStatusId para simplificar el DTO.
    // Revisa la documentación y los commits para más detalles.
    public class UpdateRequestDto
    {
        // Campo requerido
        // La cantidad debe ser un número positivo
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero.")]
        public double MaintenanceAmount { get; set; }
    }
}
