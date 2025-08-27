using System.ComponentModel.DataAnnotations;

namespace PrototipoApi.Models
{
    public class PatchStatusByDescriptionDto
    {
        [Required]
        public string Description { get; set; } = string.Empty;
        public string? Comment { get; set; }
    }
}
