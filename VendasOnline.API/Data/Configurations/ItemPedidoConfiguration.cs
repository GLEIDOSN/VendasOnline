using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VendasOnline.API.Models;

namespace VendasOnline.API.Data.Configurations;

public class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
{
    public void Configure(EntityTypeBuilder<ItemPedido> builder)
    {
        builder.ToTable("ItensPedido");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Quantidade)
               .IsRequired();

        builder.Property(i => i.PrecoUnitario)
               .IsRequired()
               .HasPrecision(18, 2);

        // Relacionamento com Pedido
        builder.HasOne(i => i.Pedido)
               .WithMany(p => p.Itens)
               .HasForeignKey(i => i.PedidoId)
               .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento com Produto (impede a exclusão do produto se ele estiver vinculado a um item de pedido)
        builder.HasOne(i => i.Produto)
               .WithMany()
               .HasForeignKey(i => i.ProdutoId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
