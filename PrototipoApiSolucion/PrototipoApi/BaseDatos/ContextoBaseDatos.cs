using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PrototipoApi.Entities;
using PrototipoApi.Models;

namespace PrototipoApi.BaseDatos
{
    // Unificado: ahora hereda de IdentityDbContext<AppUser>
    public class ContextoBaseDatos : IdentityDbContext<AppUser>
    {
        public ContextoBaseDatos(DbContextOptions<ContextoBaseDatos> options) : base(options)
        {
        }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<ManagementBudget> ManagementBudgets { get; set; } = default!;
        public DbSet<Transaction> Transactions { get; set; } = default!;
        public DbSet<TransactionType> TransactionsTypes { get; set; } = default!;
        public DbSet<Apartment> Apartments { get; set; } = default!;
        public DbSet<RequestStatusHistory> RequestStatusHistories { get; set; } = default!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RequestStatusHistory>()
                .HasOne(rsh => rsh.OldStatus)
                .WithMany()
                .HasForeignKey(rsh => rsh.OldStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestStatusHistory>()
                .HasOne(rsh => rsh.NewStatus)
                .WithMany()
                .HasForeignKey(rsh => rsh.NewStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices para RefreshToken
            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.UserId);
            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => new { rt.Token, rt.IsRevoked });
        }
    }
}
