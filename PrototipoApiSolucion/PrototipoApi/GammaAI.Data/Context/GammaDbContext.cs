using Microsoft.EntityFrameworkCore;
using GammaAI.Core.Models;

namespace GammaAI.Data.Context
{
    public class GammaDbContext : DbContext
    {
        public GammaDbContext(DbContextOptions<GammaDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplicar configuraciones
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GammaDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=GammaAIDb;Trusted_Connection=true;MultipleActiveResultSets=true;");
            }
        }
    }
}
