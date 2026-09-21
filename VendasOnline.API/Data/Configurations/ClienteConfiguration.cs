using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VendasOnline.API.Models;

namespace VendasOnline.API.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.Email)
               .IsRequired()
               .HasMaxLength(150);

        // Índice para otimizar o endpoint de busca por nome (FindByName)
        builder.HasIndex(c => c.Nome)
               .HasDatabaseName("IX_Clientes_Nome");

        // Relacionamento 1:N com Pedidos
        builder.HasMany(c => c.Pedidos)
               .WithOne(p => p.Cliente)
               .HasForeignKey(p => p.ClienteId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
