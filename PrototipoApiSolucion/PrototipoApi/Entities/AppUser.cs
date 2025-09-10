using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace PrototipoApi.Entities
{
    public class AppUser : IdentityUser
    {
        public string? TenantId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navegación
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
