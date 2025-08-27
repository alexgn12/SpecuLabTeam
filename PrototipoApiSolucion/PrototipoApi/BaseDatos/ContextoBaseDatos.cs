using Microsoft.EntityFrameworkCore;
using PrototipoApi.Entities;
using PrototipoApi.Models;

namespace PrototipoApi.BaseDatos
{
    public class ContextoBaseDatos : DbContext
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
        }
    }
}
