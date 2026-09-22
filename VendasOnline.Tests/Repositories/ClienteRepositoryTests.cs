using Microsoft.EntityFrameworkCore;
using VendasOnline.API.Data;
using VendasOnline.API.Data.Repositories;
using VendasOnline.API.Models;

namespace VendasOnline.Tests.Repositories;

public class ClienteRepositoryTests
{
    private static VendasOnlineDbContext CriarDbContext()
    {
        var options = new DbContextOptionsBuilder<VendasOnlineDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new VendasOnlineDbContext(options);
    }

    [Fact]
    public async Task ObterPorNomeAsync_DeveRetornarClientes_QuandoNomeCoincidirParcialmenteOuSemCaseSensitivity()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ClienteRepository(context);

        await context.Clientes.AddRangeAsync(
            new Cliente { Nome = "Maria Eduarda Silva", Email = "maria@email.com", Ativo = true },
            new Cliente { Nome = "Maria Clara Santos", Email = "mclara@email.com", Ativo = true },
            new Cliente { Nome = "João Pedro", Email = "joao@email.com", Ativo = true }
        );
        await context.SaveChangesAsync();

        // Act - Busca ignorando maiúsculas/minúsculas
        var resultado = await repository.ObterPorNomeAsync("maria");

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
        Assert.All(resultado, c => Assert.Contains("Maria", c.Nome));
    }

    [Fact]
    public async Task ObterPorNomeAsync_DeveRetornarListaVazia_QuandoNomeNaoExistir()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ClienteRepository(context);

        await context.Clientes.AddAsync(
            new Cliente { Nome = "Carlos Eduardo", Email = "carlos@email.com" }
        );
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObterPorNomeAsync("Roberto");

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);
    }

    [Fact]
    public async Task ObterComPedidosAsync_DeveCarregarClienteEListaDePedidos_QuandoClientePossuirPedidos()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ClienteRepository(context);

        var cliente = new Cliente
        {
            Nome = "Fernanda Costa",
            Email = "fernanda@email.com",
            Pedidos =
            [
                new() { ValorTotal = 150.00m, DataPedido = DateTime.UtcNow },
                new() { ValorTotal = 300.00m, DataPedido = DateTime.UtcNow }
            ]
        };

        await context.Clientes.AddAsync(cliente);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObterComPedidosAsync(cliente.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Fernanda Costa", resultado.Nome);
        Assert.Equal(2, resultado.Pedidos.Count);
    }

    [Fact]
    public async Task ObterComPedidosAsync_DeveCarregarClienteComListaVazia_QuandoClienteNaoPossuirPedidos()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ClienteRepository(context);

        var clienteSemPedidos = new Cliente { Nome = "Ana Paula", Email = "ana@email.com" };
        await context.Clientes.AddAsync(clienteSemPedidos);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObterComPedidosAsync(clienteSemPedidos.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Ana Paula", resultado.Nome);
        Assert.Empty(resultado.Pedidos);
    }

    [Fact]
    public async Task ObterComPedidosAsync_DeveRetornarNulo_QuandoIdNaoExistir()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ClienteRepository(context);

        // Act
        var resultado = await repository.ObterComPedidosAsync(999);

        // Assert
        Assert.Null(resultado);
    }


    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarCliente_QuandoIdExistir()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ClienteRepository(context);

        var cliente = new Cliente { Nome = "Lucas Silva", Email = "lucas@email.com" };
        await context.Clientes.AddAsync(cliente);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObterPorIdAsync(cliente.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Lucas Silva", resultado.Nome);
        Assert.Equal("lucas@email.com", resultado.Email);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNulo_QuandoIdNaoExistir()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ClienteRepository(context);

        // Act
        var resultado = await repository.ObterPorIdAsync(999);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarTodosOsClientesCadastrados()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ClienteRepository(context);

        await context.Clientes.AddRangeAsync(
            new Cliente { Nome = "Cliente 1", Email = "c1@email.com" },
            new Cliente { Nome = "Cliente 2", Email = "c2@email.com" },
            new Cliente { Nome = "Cliente 3", Email = "c3@email.com" }
        );
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObterTodosAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(3, resultado.Count());
    }

    [Fact]
    public async Task AdicionarAsync_DevePersistirNovoClienteNoBanco()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ClienteRepository(context);

        var novoCliente = new Cliente
        {
            Nome = "Rodrigo Lima",
            Email = "rodrigo@email.com",
            CpfCnpj = "12345678900",
            Telefone = "11988887777",
            DataCadastro = DateTime.UtcNow,
            Ativo = true
        };

        // Act
        await repository.AdicionarAsync(novoCliente);
        var salvo = await repository.SalvarAlteracoesAsync();

        // Assert
        Assert.True(salvo);
        Assert.True(novoCliente.Id > 0);

        var noBanco = await context.Clientes.FindAsync(novoCliente.Id);
        Assert.NotNull(noBanco);
        Assert.Equal("Rodrigo Lima", noBanco.Nome);
        Assert.Equal("12345678900", noBanco.CpfCnpj);
    }

    [Fact]
    public async Task AtualizarAsync_DeveModificarDadosDoClienteExistente()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ClienteRepository(context);

        var cliente = new Cliente { Nome = "Nome Antigo", Email = "antigo@email.com", Telefone = "11111111" };
        await context.Clientes.AddAsync(cliente);
        await context.SaveChangesAsync();

        // Act - Altera nome, e-mail e telefone
        cliente.Nome = "Nome Atualizado";
        cliente.Email = "novo@email.com";
        cliente.Telefone = "22222222";

        await repository.AtualizarAsync(cliente);
        await repository.SalvarAlteracoesAsync();

        // Assert
        var atualizado = await context.Clientes.FindAsync(cliente.Id);
        Assert.NotNull(atualizado);
        Assert.Equal("Nome Atualizado", atualizado.Nome);
        Assert.Equal("novo@email.com", atualizado.Email);
        Assert.Equal("22222222", atualizado.Telefone);
    }

    [Fact]
    public async Task RemoverAsync_DeveExcluirClienteDoBanco()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ClienteRepository(context);

        var cliente = new Cliente { Nome = "Cliente Para Deletar", Email = "del@email.com" };
        await context.Clientes.AddAsync(cliente);
        await context.SaveChangesAsync();

        // Act
        await repository.RemoverAsync(cliente);
        await repository.SalvarAlteracoesAsync();

        // Assert
        var excluido = await context.Clientes.FindAsync(cliente.Id);
        Assert.Null(excluido);
    }

    [Fact]
    public async Task ContarAsync_DeveRetornarTotalExatoDeClientes()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ClienteRepository(context);

        await context.Clientes.AddRangeAsync(
            new Cliente { Nome = "Cliente A", Email = "a@email.com" },
            new Cliente { Nome = "Cliente B", Email = "b@email.com" },
            new Cliente { Nome = "Cliente C", Email = "c@email.com" },
            new Cliente { Nome = "Cliente D", Email = "d@email.com" },
            new Cliente { Nome = "Cliente E", Email = "e@email.com" }
        );
        await context.SaveChangesAsync();

        // Act
        var total = await repository.ContarAsync();

        // Assert
        Assert.Equal(5, total);
    }

    [Fact]
    public async Task BuscarAsync_DeveFiltrarClientesPorCondicaoCustomizada()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ClienteRepository(context);

        await context.Clientes.AddRangeAsync(
            new Cliente { Nome = "Ativo 1", Email = "a1@email.com", Ativo = true },
            new Cliente { Nome = "Ativo 2", Email = "a2@email.com", Ativo = true },
            new Cliente { Nome = "Inativo 1", Email = "i1@email.com", Ativo = false }
        );
        await context.SaveChangesAsync();

        // Act - Filtra apenas clientes inativos
        var inativos = await repository.BuscarAsync(c => !c.Ativo);

        // Assert
        Assert.Single(inativos);
        Assert.Equal("Inativo 1", inativos.First().Nome);
    }
}
