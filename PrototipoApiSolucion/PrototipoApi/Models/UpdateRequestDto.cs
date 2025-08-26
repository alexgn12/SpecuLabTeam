using System.ComponentModel.DataAnnotations;

namespace PrototipoApi.Models
{
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
