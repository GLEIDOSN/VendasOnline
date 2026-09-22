using Moq;
using VendasOnline.API.Data.Repositories.Interface;
using VendasOnline.API.Dtos.Request;
using VendasOnline.API.Models;
using VendasOnline.API.Services;

namespace VendasOnline.Tests.Services;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
    private readonly ProdutoService _produtoService;

    public ProdutoServiceTests()
    {
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        _produtoService = new ProdutoService(_produtoRepositoryMock.Object);
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarListaDeProdutosResponseDto_QuandoExistiremProdutos()
    {
        // Arrange
        var produtosFakes = new List<Produto>
        {
            new() { Id = 1, Descricao = "Teclado RGB", Preco = 200.00m, QuantidadeEstoque = 10, Ativo = true },
            new() { Id = 2, Descricao = "Mouse Gamer", Preco = 150.00m, QuantidadeEstoque = 15, Ativo = true }
        };

        _produtoRepositoryMock
            .Setup(r => r.ObterTodosAsync())
            .ReturnsAsync(produtosFakes);

        // Act
        var resultado = await _produtoService.ObterTodosAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
        _produtoRepositoryMock.Verify(r => r.ObterTodosAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarListaVazia_QuandoNaoExistiremProdutos()
    {
        // Arrange
        _produtoRepositoryMock
            .Setup(r => r.ObterTodosAsync())
            .ReturnsAsync([]);

        // Act
        var resultado = await _produtoService.ObterTodosAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);
        _produtoRepositoryMock.Verify(r => r.ObterTodosAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarProduto_QuandoIdExistir()
    {
        // Arrange
        int produtoId = 10;
        var produtoFake = new Produto
        {
            Id = produtoId,
            Descricao = "Monitor 27 Polegadas",
            Preco = 1200.00m,
            QuantidadeEstoque = 5,
            DataCriacao = DateTime.UtcNow,
            Ativo = true
        };

        _produtoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(produtoId))
            .ReturnsAsync(produtoFake);

        // Act
        var resultado = await _produtoService.ObterPorIdAsync(produtoId);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(produtoId, resultado.Id);
        Assert.Equal("Monitor 27 Polegadas", resultado.Descricao);
        Assert.Equal(1200.00m, resultado.Preco);
        Assert.Equal(5, resultado.QuantidadeEstoque);
        Assert.True(resultado.Ativo);

        _produtoRepositoryMock.Verify(r => r.ObterPorIdAsync(produtoId), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveLancarKeyNotFoundException_QuandoIdNaoExistir()
    {
        // Arrange
        int idInexistente = 99;

        _produtoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(idInexistente))
            .ReturnsAsync((Produto?)null);

        // Act & Assert
        var excecao = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _produtoService.ObterPorIdAsync(idInexistente)
        );

        Assert.Equal($"Produto com ID {idInexistente} não foi encontrado.", excecao.Message);
        _produtoRepositoryMock.Verify(r => r.ObterPorIdAsync(idInexistente), Times.Once);
    }

    [Fact]
    public async Task ObterPorNomeAsync_DeveRetornarProdutos_QuandoNomeEncontrado()
    {
        // Arrange
        string termoBusca = "Cadeira";
        var produtosFakes = new List<Produto>
        {
            new() { Id = 1, Descricao = "Cadeira Gamer Ergonomica", Preco = 900.00m, QuantidadeEstoque = 3 }
        };

        _produtoRepositoryMock
            .Setup(r => r.ObterPorNomeAsync(termoBusca))
            .ReturnsAsync(produtosFakes);

        // Act
        var resultado = await _produtoService.ObterPorNomeAsync(termoBusca);

        // Assert
        Assert.NotNull(resultado);
        Assert.Single(resultado);
        Assert.Equal("Cadeira Gamer Ergonomica", resultado.First().Descricao);
        _produtoRepositoryMock.Verify(r => r.ObterPorNomeAsync(termoBusca), Times.Once);
    }

    [Fact]
    public async Task ObterPorNomeAsync_DeveRetornarListaVazia_QuandoNomeNaoEncontrado()
    {
        // Arrange
        string termoInexistente = "ProdutoInexistente";

        _produtoRepositoryMock
            .Setup(r => r.ObterPorNomeAsync(termoInexistente))
            .ReturnsAsync(new List<Produto>());

        // Act
        var resultado = await _produtoService.ObterPorNomeAsync(termoInexistente);

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);
        _produtoRepositoryMock.Verify(r => r.ObterPorNomeAsync(termoInexistente), Times.Once);
    }

    [Fact]
    public async Task ContarAsync_DeveRetornarTotalDeProdutos()
    {
        // Arrange
        _produtoRepositoryMock
            .Setup(r => r.ContarAsync())
            .ReturnsAsync(42);

        // Act
        var total = await _produtoService.ContarAsync();

        // Assert
        Assert.Equal(42, total);
        _produtoRepositoryMock.Verify(r => r.ContarAsync(), Times.Once);
    }

    [Fact]
    public async Task ContarAsync_DeveRetornarZero_QuandoBancoEstiverVazio()
    {
        // Arrange
        _produtoRepositoryMock
            .Setup(r => r.ContarAsync())
            .ReturnsAsync(0);

        // Act
        var total = await _produtoService.ContarAsync();

        // Assert
        Assert.Equal(0, total);
        _produtoRepositoryMock.Verify(r => r.ContarAsync(), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveAdicionarProdutoESalvar()
    {
        // Arrange
        var requestDto = new ProdutoRequestDto("SSD NVMe 1TB", 450.00m, 20);

        _produtoRepositoryMock
            .Setup(r => r.AdicionarAsync(It.IsAny<Produto>()))
            .Returns(Task.CompletedTask);

        _produtoRepositoryMock
            .Setup(r => r.SalvarAlteracoesAsync())
            .ReturnsAsync(true);

        // Act
        var resultado = await _produtoService.CriarAsync(requestDto);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("SSD NVMe 1TB", resultado.Descricao);
        Assert.Equal(450.00m, resultado.Preco);
        Assert.Equal(20, resultado.QuantidadeEstoque);
        Assert.True(resultado.Ativo);

        _produtoRepositoryMock.Verify(r => r.AdicionarAsync(It.Is<Produto>(p =>
            p.Descricao == requestDto.Descricao &&
            p.Preco == requestDto.Preco &&
            p.QuantidadeEstoque == requestDto.QuantidadeEstoque &&
            p.Ativo == true
        )), Times.Once);

        _produtoRepositoryMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarProdutoESalvar_QuandoIdExistir()
    {
        // Arrange
        int produtoId = 5;
        var produtoExistente = new Produto
        {
            Id = produtoId,
            Descricao = "Headset Basico",
            Preco = 100.00m,
            QuantidadeEstoque = 10
        };

        var updateDto = new ProdutoRequestDto("Headset Pro 7.1", 250.00m, 8);

        _produtoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(produtoId))
            .ReturnsAsync(produtoExistente);

        _produtoRepositoryMock
            .Setup(r => r.AtualizarAsync(produtoExistente))
            .Returns(Task.CompletedTask);

        _produtoRepositoryMock
            .Setup(r => r.SalvarAlteracoesAsync())
            .ReturnsAsync(true);

        // Act
        var resultado = await _produtoService.AtualizarAsync(produtoId, updateDto);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Headset Pro 7.1", resultado.Descricao);
        Assert.Equal(250.00m, resultado.Preco);
        Assert.Equal(8, resultado.QuantidadeEstoque);

        _produtoRepositoryMock.Verify(r => r.AtualizarAsync(produtoExistente), Times.Once);
        _produtoRepositoryMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarKeyNotFoundException_QuandoIdNaoExistir()
    {
        // Arrange
        int idInexistente = 99;
        var updateDto = new ProdutoRequestDto("Produto Inexistente", 50.00m, 1);

        _produtoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(idInexistente))
            .ReturnsAsync((Produto?)null);

        // Act & Assert
        var excecao = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _produtoService.AtualizarAsync(idInexistente, updateDto)
        );

        Assert.Equal($"Produto com ID {idInexistente} não foi encontrado.", excecao.Message);
        _produtoRepositoryMock.Verify(r => r.ObterPorIdAsync(idInexistente), Times.Once);
        _produtoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Produto>()), Times.Never);
        _produtoRepositoryMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Never);
    }

    [Fact]
    public async Task RemoverAsync_DeveRemoverProdutoESalvar_QuandoIdExistir()
    {
        // Arrange
        int produtoId = 3;
        var produtoExistente = new Produto { Id = produtoId, Descricao = "Gabinete ATX" };

        _produtoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(produtoId))
            .ReturnsAsync(produtoExistente);

        _produtoRepositoryMock
            .Setup(r => r.RemoverAsync(produtoExistente))
            .Returns(Task.CompletedTask);

        _produtoRepositoryMock
            .Setup(r => r.SalvarAlteracoesAsync())
            .ReturnsAsync(true);

        // Act
        await _produtoService.RemoverAsync(produtoId);

        // Assert
        _produtoRepositoryMock.Verify(r => r.RemoverAsync(produtoExistente), Times.Once);
        _produtoRepositoryMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveLancarKeyNotFoundException_QuandoIdNaoExistir()
    {
        // Arrange
        int idInexistente = 99;

        _produtoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(idInexistente))
            .ReturnsAsync((Produto?)null);

        // Act & Assert
        var excecao = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _produtoService.RemoverAsync(idInexistente)
        );

        Assert.Equal($"Produto com ID {idInexistente} não foi encontrado.", excecao.Message);
        _produtoRepositoryMock.Verify(r => r.ObterPorIdAsync(idInexistente), Times.Once);
        _produtoRepositoryMock.Verify(r => r.RemoverAsync(It.IsAny<Produto>()), Times.Never);
        _produtoRepositoryMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Never);
    }
}