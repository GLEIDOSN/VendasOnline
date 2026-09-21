using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VendasOnline.API.Models;

namespace VendasOnline.API.Data.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Descricao)
               .IsRequired()
               .HasMaxLength(150);

        // Índice para otimizar a busca por Descrição (FindByName)
        builder.HasIndex(p => p.Descricao)
               .HasDatabaseName("IX_Produtos_Descricao");

        builder.Property(p => p.Descricao)
               .HasMaxLength(500);

        builder.Property(p => p.Preco)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(p => p.QuantidadeEstoque)
               .IsRequired();

        builder.Property(p => p.DataCriacao)
               .IsRequired();

        builder.Property(p => p.Ativo)
               .IsRequired();
    }
}
