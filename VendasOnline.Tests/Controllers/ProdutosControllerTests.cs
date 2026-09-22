using Microsoft.AspNetCore.Mvc;
using Moq;
using VendasOnline.API.Controllers;
using VendasOnline.API.Dtos.Request;
using VendasOnline.API.Dtos.Response;
using VendasOnline.API.Services.Interfaces;

namespace VendasOnline.Tests.Controllers;

public class ProdutosControllerTests
{
    private readonly Mock<IProdutoService> _produtoServiceMock;
    private readonly ProdutosController _controller;

    public ProdutosControllerTests()
    {
        _produtoServiceMock = new Mock<IProdutoService>();
        _controller = new ProdutosController(_produtoServiceMock.Object);
    }

    [Fact]
    public async Task ObterTodos_DeveRetornarOkComListaDeProdutos()
    {
        // Arrange
        var produtosDto = new List<ProdutoResponseDto>
        {
            new(1, "Teclado RGB", 200.00m, 10, DateTime.UtcNow, true),
            new(2, "Mouse Gamer", 150.00m, 15, DateTime.UtcNow, true)
        };

        _produtoServiceMock.Setup(s => s.ObterTodosAsync()).ReturnsAsync(produtosDto);

        // Act
        var result = await _controller.ObterTodos();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var retorno = Assert.IsAssignableFrom<IEnumerable<ProdutoResponseDto>>(okResult.Value);
        Assert.Equal(2, retorno.Count());
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarOkComProduto()
    {
        // Arrange
        int produtoId = 1;
        var produtoDto = new ProdutoResponseDto(produtoId, "Monitor 27", 1200.00m, 5, DateTime.UtcNow, true);

        _produtoServiceMock.Setup(s => s.ObterPorIdAsync(produtoId)).ReturnsAsync(produtoDto);

        // Act
        var result = await _controller.ObterPorId(produtoId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var retorno = Assert.IsType<ProdutoResponseDto>(okResult.Value);
        Assert.Equal(produtoId, retorno.Id);
    }

    [Fact]
    public async Task ObterPorNome_DeveRetornarOkComProdutosFiltrados()
    {
        // Arrange
        string nomeBusca = "Teclado";
        var produtosDto = new List<ProdutoResponseDto>
        {
            new(1, "Teclado Mecanico", 300.00m, 8, DateTime.UtcNow, true)
        };

        _produtoServiceMock.Setup(s => s.ObterPorNomeAsync(nomeBusca)).ReturnsAsync(produtosDto);

        // Act
        var result = await _controller.ObterPorNome(nomeBusca);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var retorno = Assert.IsAssignableFrom<IEnumerable<ProdutoResponseDto>>(okResult.Value);
        Assert.Single(retorno);
    }

    [Fact]
    public async Task Contar_DeveRetornarOkComTotalDeProdutos()
    {
        // Arrange
        _produtoServiceMock.Setup(s => s.ContarAsync()).ReturnsAsync(25);

        // Act
        var result = await _controller.Contar();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Criar_DeveRetornarCreatedAtAction_ComNovoProduto()
    {
        // Arrange
        var requestDto = new ProdutoRequestDto("Cadeira Gamer", 850.00m, 4);
        var responseDto = new ProdutoResponseDto(1, "Cadeira Gamer", 850.00m, 4, DateTime.UtcNow, true);

        _produtoServiceMock.Setup(s => s.CriarAsync(requestDto)).ReturnsAsync(responseDto);

        // Act
        var result = await _controller.Criar(requestDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(ProdutosController.ObterPorId), createdResult.ActionName);
        Assert.Equal(1, createdResult.RouteValues?["id"]);
        var retorno = Assert.IsType<ProdutoResponseDto>(createdResult.Value);
        Assert.Equal("Cadeira Gamer", retorno.Descricao);
    }

    [Fact]
    public async Task Atualizar_DeveRetornarOkComProdutoAtualizado()
    {
        // Arrange
        int produtoId = 1;
        var requestDto = new ProdutoRequestDto("Cadeira Pro", 950.00m, 3);
        var responseDto = new ProdutoResponseDto(produtoId, "Cadeira Pro", 950.00m, 3, DateTime.UtcNow, true);

        _produtoServiceMock.Setup(s => s.AtualizarAsync(produtoId, requestDto)).ReturnsAsync(responseDto);

        // Act
        var result = await _controller.Atualizar(produtoId, requestDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var retorno = Assert.IsType<ProdutoResponseDto>(okResult.Value);
        Assert.Equal("Cadeira Pro", retorno.Descricao);
    }

    [Fact]
    public async Task Remover_DeveRetornarNoContent()
    {
        // Arrange
        int produtoId = 1;
        _produtoServiceMock.Setup(s => s.RemoverAsync(produtoId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Remover(produtoId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _produtoServiceMock.Verify(s => s.RemoverAsync(produtoId), Times.Once);
    }
}
