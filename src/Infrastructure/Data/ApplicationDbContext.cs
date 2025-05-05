using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar a precisão e escala para o campo Preco
            modelBuilder.Entity<Product>()
                .Property(p => p.Preco)
                .HasColumnType("decimal(18,2)"); // Define precisão e escala
        }
    }
}