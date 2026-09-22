using Microsoft.EntityFrameworkCore;
using VendasOnline.API.Data;
using VendasOnline.API.Data.Repositories;
using VendasOnline.API.Enums;
using VendasOnline.API.Models;

namespace VendasOnline.Tests.Repositories;

public class PedidoRepositoryTests
{
    private static VendasOnlineDbContext CriarDbContext()
    {
        var options = new DbContextOptionsBuilder<VendasOnlineDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new VendasOnlineDbContext(options);
    }

    [Fact]
    public async Task ObterDetalhesPedidoAsync_DeveTrazerClienteEItensComProduto()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new PedidoRepository(context);

        var cliente = new Cliente { Nome = "Roberto Alves", Email = "roberto@email.com" };
        var produto = new Produto { Descricao = "Notebook i7", Preco = 4500.00m, QuantidadeEstoque = 5 };

        var pedido = new Pedido
        {
            Cliente = cliente,
            DataPedido = DateTime.UtcNow,
            Status = StatusPedido.Pendente,
            ValorTotal = 4500.00m,
            Itens =
            [
                new() { Produto = produto, Quantidade = 1, PrecoUnitario = 4500.00m }
            ]
        };

        await context.Pedidos.AddAsync(pedido);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObterDetalhesPedidoAsync(pedido.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Roberto Alves", resultado.Cliente.Nome);
        Assert.Single(resultado.Itens);
        Assert.Equal("Notebook i7", resultado.Itens.First().Produto.Descricao);
    }

    [Fact]
    public async Task ObterPedidosPorClienteAsync_DeveRetornarApenasPedidosDoClienteInformado()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new PedidoRepository(context);

        var cliente1 = new Cliente { Nome = "Cliente A", Email = "a@email.com" };
        var cliente2 = new Cliente { Nome = "Cliente B", Email = "b@email.com" };

        await context.Clientes.AddRangeAsync(cliente1, cliente2);
        await context.SaveChangesAsync();

        await context.Pedidos.AddRangeAsync(
            new() { ClienteId = cliente1.Id, ValorTotal = 100.00m },
            new() { ClienteId = cliente1.Id, ValorTotal = 200.00m },
            new() { ClienteId = cliente2.Id, ValorTotal = 300.00m }
        );
        await context.SaveChangesAsync();

        // Act
        var pedidosCliente1 = await repository.ObterPedidosPorClienteAsync(cliente1.Id);

        // Assert
        Assert.Equal(2, pedidosCliente1.Count());
        Assert.All(pedidosCliente1, p => Assert.Equal(cliente1.Id, p.ClienteId));
    }
}