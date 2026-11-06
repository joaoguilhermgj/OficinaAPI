
using Microsoft.EntityFrameworkCore;
using Oficina.Domain.Entities;

namespace Oficina.Infrastructure.Context
{
    public class OficinaDbContext : DbContext
    {

        public OficinaDbContext(DbContextOptions<OficinaDbContext> options) : base(options)
        {

        }

        public DbSet<Veiculo> Veiculos => Set<Veiculo>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Veiculo>(entity =>
            {
                entity.Property(v => v.Placa).IsRequired().HasMaxLength(7);
                entity.Property(v => v.Marca).IsRequired().HasMaxLength(50);
                entity.Property(v => v.Modelo).IsRequired().HasMaxLength(50);
                entity.Property(v => v.Cor).HasMaxLength(30);
                entity.Property(v => v.Tipo).HasConversion<string>().HasMaxLength(20);
                entity.Property(v => v.Status).HasConversion<string>().HasMaxLength(20);

                entity.HasIndex(v => v.Placa).IsUnique();
            });

            
        }
    }
}
