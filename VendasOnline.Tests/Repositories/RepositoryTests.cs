using Microsoft.EntityFrameworkCore;
using VendasOnline.API.Data;
using VendasOnline.API.Data.Repositories;
using VendasOnline.API.Models;

namespace VendasOnline.Tests.Repositories;

public class RepositoryTests
{
    private static VendasOnlineDbContext CriarDbContext()
    {
        var options = new DbContextOptionsBuilder<VendasOnlineDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new VendasOnlineDbContext(options);
    }

    [Fact]
    public async Task AdicionarAsync_E_SalvarAlteracoesAsync_DevePersistirEntidadeNoBanco()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new Repository<Cliente>(context);

        var cliente = new Cliente
        {
            Nome = "Maria Oliveira",
            Email = "maria@email.com",
            DataCadastro = DateTime.UtcNow,
            Ativo = true
        };

        // Act
        await repository.AdicionarAsync(cliente);
        var salvo = await repository.SalvarAlteracoesAsync();

        // Assert
        Assert.True(salvo);
        Assert.True(cliente.Id > 0);

        var clienteNoBanco = await context.Clientes.FindAsync(cliente.Id);
        Assert.NotNull(clienteNoBanco);
        Assert.Equal("Maria Oliveira", clienteNoBanco.Nome);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarEntidadeInclusa()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new Repository<Cliente>(context);

        var cliente = new Cliente { Nome = "Lucas Silva", Email = "lucas@email.com" };
        await context.Clientes.AddAsync(cliente);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObterPorIdAsync(cliente.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Lucas Silva", resultado.Nome);
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarTodosOsRegistros()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new Repository<Cliente>(context);

        await context.Clientes.AddRangeAsync(
            new Cliente { Nome = "Cliente 1", Email = "c1@email.com" },
            new Cliente { Nome = "Cliente 2", Email = "c2@email.com" }
        );
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObterTodosAsync();

        // Assert
        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public async Task BuscarAsync_DeveFiltrarCorretamentePorExpressao()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new Repository<Produto>(context);

        await context.Produtos.AddRangeAsync(
            new Produto { Descricao = "Mouse Barato", Preco = 30.00m, QuantidadeEstoque = 10 },
            new Produto { Descricao = "Teclado Caro", Preco = 350.00m, QuantidadeEstoque = 5 }
        );
        await context.SaveChangesAsync();

        // Act - Filtra produtos com preço maior que 100
        var resultado = await repository.BuscarAsync(p => p.Preco > 100.00m);

        // Assert
        Assert.Single(resultado);
        Assert.Equal("Teclado Caro", resultado.First().Descricao);
    }

    [Fact]
    public async Task ContarAsync_DeveRetornarQuantidadeExataDeRegistros()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new Repository<Produto>(context);

        await context.Produtos.AddRangeAsync(
            new Produto { Descricao = "P1", Preco = 10, QuantidadeEstoque = 1 },
            new Produto { Descricao = "P2", Preco = 20, QuantidadeEstoque = 2 },
            new Produto { Descricao = "P3", Preco = 30, QuantidadeEstoque = 3 }
        );
        await context.SaveChangesAsync();

        // Act
        var total = await repository.ContarAsync();

        // Assert
        Assert.Equal(3, total);
    }

    [Fact]
    public async Task RemoverAsync_DeveExcluirEntidadeDoBanco()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new Repository<Cliente>(context);

        var cliente = new Cliente { Nome = "Para Deletar", Email = "del@email.com" };
        await context.Clientes.AddAsync(cliente);
        await context.SaveChangesAsync();

        // Act
        await repository.RemoverAsync(cliente);
        await repository.SalvarAlteracoesAsync();

        // Assert
        var clienteNoBanco = await context.Clientes.FindAsync(cliente.Id);
        Assert.Null(clienteNoBanco);
    }
}