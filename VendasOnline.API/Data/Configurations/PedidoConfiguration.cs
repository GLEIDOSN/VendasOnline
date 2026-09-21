using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VendasOnline.API.Models;

namespace VendasOnline.API.Data.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.DataPedido)
               .IsRequired();

        builder.Property(p => p.ValorTotal)
               .IsRequired()
               .HasPrecision(18, 2);

        // Converte o Enum StatusPedido para texto (string) no banco de dados
        builder.Property(p => p.Status)
               .HasConversion<string>()
               .HasMaxLength(30)
               .IsRequired();

        // Relacionamento 1:N -> Cliente possui N Pedidos (impede a exclusão do cliente se houver pedidos)
        builder.HasOne(p => p.Cliente)
               .WithMany(c => c.Pedidos)
               .HasForeignKey(p => p.ClienteId)
               .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento 1:N -> Pedido possui N Itens (exclui os itens em cascata se o pedido for apagado)
        builder.HasMany(p => p.Itens)
               .WithOne(i => i.Pedido)
               .HasForeignKey(i => i.PedidoId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
