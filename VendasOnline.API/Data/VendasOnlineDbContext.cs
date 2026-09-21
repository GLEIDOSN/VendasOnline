using Microsoft.EntityFrameworkCore;
using VendasOnline.API.Models;

namespace VendasOnline.API.Data;

public class VendasOnlineDbContext(DbContextOptions<VendasOnlineDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<ItemPedido> ItensPedido { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VendasOnlineDbContext).Assembly);
    }
}
