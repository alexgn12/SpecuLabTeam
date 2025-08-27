using System.ComponentModel.DataAnnotations;
using PrototipoApi.Helpers;

namespace PrototipoApi.Models
{
    public class UpdateManagementBudgetDto : IValidatableObject
    {
        // Campo requerido
        // La cantidad debe ser un número positivo
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero.")]
        public double CurrentAmount { get; set; }

        public DateTime LastUpdatedDate { get; set; }
        public string LastUpdatedDateFormatted => DateFormatHelper.ToExternalFormat(LastUpdatedDate);

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validación de la fecha de actualización
            if (LastUpdatedDate > DateTime.Now)
            {
                yield return new ValidationResult("La fecha de actualización no puede ser futura.", new[] { nameof(LastUpdatedDate) });
            }
        }
    }

}
