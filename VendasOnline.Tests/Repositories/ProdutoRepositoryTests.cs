using Microsoft.EntityFrameworkCore;
using VendasOnline.API.Data;
using VendasOnline.API.Data.Repositories;
using VendasOnline.API.Models;

namespace VendasOnline.Tests.Repositories;

public class ProdutoRepositoryTests
{
    private static VendasOnlineDbContext CriarDbContext()
    {
        var options = new DbContextOptionsBuilder<VendasOnlineDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new VendasOnlineDbContext(options);
    }

    [Fact]
    public async Task ObterPorNomeAsync_DeveRetornarProdutos_QuandoDescricaoCoincidirParcialmente()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ProdutoRepository(context);

        await context.Produtos.AddRangeAsync(
            new Produto { Descricao = "Teclado Mecânico RGB", Preco = 250.00m, QuantidadeEstoque = 10 },
            new Produto { Descricao = "Teclado Membrana", Preco = 80.00m, QuantidadeEstoque = 15 },
            new Produto { Descricao = "Mouse Sem Fio", Preco = 120.00m, QuantidadeEstoque = 5 }
        );
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObterPorNomeAsync("Teclado");

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
        Assert.All(resultado, p => Assert.Contains("Teclado", p.Descricao));
    }

    [Fact]
    public async Task ObterPorNomeAsync_DeveRetornarListaVazia_QuandoNomeNaoExistir()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ProdutoRepository(context);

        await context.Produtos.AddAsync(
            new Produto { Descricao = "Monitor 24 Polegadas", Preco = 800.00m, QuantidadeEstoque = 4 }
        );
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObterPorNomeAsync("Cadeira");

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarProduto_QuandoIdExistir()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ProdutoRepository(context);

        var produto = new Produto { Descricao = "Headset Gamer", Preco = 300.00m, QuantidadeEstoque = 8 };
        await context.Produtos.AddAsync(produto);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObterPorIdAsync(produto.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Headset Gamer", resultado.Descricao);
        Assert.Equal(300.00m, resultado.Preco);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNulo_QuandoIdNaoExistir()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ProdutoRepository(context);

        // Act
        var resultado = await repository.ObterPorIdAsync(999);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarTodosOsProdutosCadastrados()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ProdutoRepository(context);

        await context.Produtos.AddRangeAsync(
            new Produto { Descricao = "Webcam Full HD", Preco = 200.00m, QuantidadeEstoque = 12 },
            new Produto { Descricao = "Microfone USB", Preco = 350.00m, QuantidadeEstoque = 7 }
        );
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObterTodosAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public async Task AdicionarAsync_DevePersistirNovoProdutoNoBanco()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ProdutoRepository(context);

        var novoProduto = new Produto
        {
            Descricao = "Processador Octa Core",
            Preco = 1500.00m,
            QuantidadeEstoque = 20,
            DataCriacao = DateTime.UtcNow,
            Ativo = true
        };

        // Act
        await repository.AdicionarAsync(novoProduto);
        var salvo = await repository.SalvarAlteracoesAsync();

        // Assert
        Assert.True(salvo);
        Assert.True(novoProduto.Id > 0);

        var noBanco = await context.Produtos.FindAsync(novoProduto.Id);
        Assert.NotNull(noBanco);
        Assert.Equal("Processador Octa Core", noBanco.Descricao);
    }

    [Fact]
    public async Task AtualizarAsync_DeveModificarDadosDoProdutoExistente()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ProdutoRepository(context);

        var produto = new Produto { Descricao = "Placa de Vídeo 8GB", Preco = 2200.00m, QuantidadeEstoque = 5 };
        await context.Produtos.AddAsync(produto);
        await context.SaveChangesAsync();

        // Act - Altera preço e quantidade em estoque
        produto.Preco = 1999.90m;
        produto.QuantidadeEstoque = 3;

        await repository.AtualizarAsync(produto);
        await repository.SalvarAlteracoesAsync();

        // Assert
        var atualizado = await context.Produtos.FindAsync(produto.Id);
        Assert.NotNull(atualizado);
        Assert.Equal(1999.90m, atualizado.Preco);
        Assert.Equal(3, atualizado.QuantidadeEstoque);
    }

    [Fact]
    public async Task RemoverAsync_DeveExcluirProdutoDoBanco()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ProdutoRepository(context);

        var produto = new Produto { Descricao = "Hub USB-C", Preco = 90.00m, QuantidadeEstoque = 15 };
        await context.Produtos.AddAsync(produto);
        await context.SaveChangesAsync();

        // Act
        await repository.RemoverAsync(produto);
        await repository.SalvarAlteracoesAsync();

        // Assert
        var excluido = await context.Produtos.FindAsync(produto.Id);
        Assert.Null(excluido);
    }

    [Fact]
    public async Task ContarAsync_DeveRetornarAQuantidadeTotalDeProdutos()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ProdutoRepository(context);

        await context.Produtos.AddRangeAsync(
            new Produto { Descricao = "Item A", Preco = 10, QuantidadeEstoque = 1 },
            new Produto { Descricao = "Item B", Preco = 20, QuantidadeEstoque = 2 },
            new Produto { Descricao = "Item C", Preco = 30, QuantidadeEstoque = 3 },
            new Produto { Descricao = "Item D", Preco = 40, QuantidadeEstoque = 4 }
        );
        await context.SaveChangesAsync();

        // Act
        var total = await repository.ContarAsync();

        // Assert
        Assert.Equal(4, total);
    }

    [Fact]
    public async Task BuscarAsync_DeveFiltrarProdutosPorCondicaoCustomizada()
    {
        // Arrange
        using var context = CriarDbContext();
        var repository = new ProdutoRepository(context);

        await context.Produtos.AddRangeAsync(
            new Produto { Descricao = "SSD 1TB", Preco = 400.00m, QuantidadeEstoque = 2 },
            new Produto { Descricao = "HD Externo 2TB", Preco = 350.00m, QuantidadeEstoque = 15 },
            new Produto { Descricao = "Pendrive 64GB", Preco = 45.00m, QuantidadeEstoque = 0 }
        );
        await context.SaveChangesAsync();

        var semEstoque = await repository.BuscarAsync(p => p.QuantidadeEstoque == 0);

        // Assert
        Assert.Single(semEstoque);
        Assert.Equal("Pendrive 64GB", semEstoque.First().Descricao);
    }
}