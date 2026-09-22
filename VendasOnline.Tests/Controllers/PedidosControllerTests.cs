using Microsoft.AspNetCore.Mvc;
using Moq;
using VendasOnline.API.Controllers;
using VendasOnline.API.Dtos.Request;
using VendasOnline.API.Dtos.Response;
using VendasOnline.API.Enums;
using VendasOnline.API.Services.Interfaces;

namespace VendasOnline.Tests.Controllers;

public class PedidosControllerTests
{
    private readonly Mock<IPedidoService> _pedidoServiceMock;
    private readonly PedidosController _controller;

    public PedidosControllerTests()
    {
        _pedidoServiceMock = new Mock<IPedidoService>();
        _controller = new PedidosController(_pedidoServiceMock.Object);
    }

    [Fact]
    public async Task ObterTodos_DeveRetornarOkComListaDePedidos()
    {
        // Arrange
        var itensDto = new List<ItemPedidoResponseDto> { new(1, "Mouse", 2, 100.00m, 200.00m) };
        var pedidosDto = new List<PedidoResponseDto>
        {
            new(1, 10, "Carlos Silva", DateTime.UtcNow, 200.00m, StatusPedido.Pendente, itensDto)
        };

        _pedidoServiceMock.Setup(s => s.ObterTodosAsync()).ReturnsAsync(pedidosDto);

        // Act
        var result = await _controller.ObterTodos();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var retorno = Assert.IsAssignableFrom<IEnumerable<PedidoResponseDto>>(okResult.Value);
        Assert.Single(retorno);
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarOkComPedido()
    {
        // Arrange
        int pedidoId = 1;
        var itensDto = new List<ItemPedidoResponseDto> { new(1, "Mouse", 1, 100.00m, 100.00m) };
        var pedidoDto = new PedidoResponseDto(pedidoId, 10, "Carlos Silva", DateTime.UtcNow, 100.00m, StatusPedido.Pendente, itensDto);

        _pedidoServiceMock.Setup(s => s.ObterPorIdAsync(pedidoId)).ReturnsAsync(pedidoDto);

        // Act
        var result = await _controller.ObterPorId(pedidoId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var retorno = Assert.IsType<PedidoResponseDto>(okResult.Value);
        Assert.Equal(pedidoId, retorno.Id);
    }

    [Fact]
    public async Task Contar_DeveRetornarOkComTotalDePedidos()
    {
        // Arrange
        _pedidoServiceMock.Setup(s => s.ContarAsync()).ReturnsAsync(8);

        // Act
        var result = await _controller.Contar();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Criar_DeveRetornarCreatedAtAction_ComNovoPedido()
    {
        // Arrange
        var requestDto = new PedidoRequestDto(10, [new(1, 2, 5)]);
        var itensDto = new List<ItemPedidoResponseDto> { new(1, "Teclado", 2, 150.00m, 300.00m) };
        var responseDto = new PedidoResponseDto(1, 10, "Carlos Silva", DateTime.UtcNow, 300.00m, StatusPedido.Pendente, itensDto);

        _pedidoServiceMock.Setup(s => s.CriarAsync(requestDto)).ReturnsAsync(responseDto);

        // Act
        var result = await _controller.Criar(requestDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(PedidosController.ObterPorId), createdResult.ActionName);
        Assert.Equal(1, createdResult.RouteValues?["id"]);
        var retorno = Assert.IsType<PedidoResponseDto>(createdResult.Value);
        Assert.Equal(300.00m, retorno.ValorTotal);
        Assert.Equal(300.00m, retorno.ValorTotal);
    }

    [Fact]
    public async Task AtualizarStatus_DeveRetornarOkComPedidoConcluido()
    {
        // Arrange
        int pedidoId = 1;
        var novoStatus = StatusPedido.Concluido;
        var itensDto = new List<ItemPedidoResponseDto> { new(1, "Teclado", 2, 150.00m, 300.00m) };
        var responseDto = new PedidoResponseDto(pedidoId, 10, "Carlos Silva", DateTime.UtcNow, 300.00m, novoStatus, itensDto);

        _pedidoServiceMock.Setup(s => s.AtualizarStatusAsync(pedidoId, novoStatus)).ReturnsAsync(responseDto);

        // Act
        var result = await _controller.AtualizarStatus(pedidoId, novoStatus);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var retorno = Assert.IsType<PedidoResponseDto>(okResult.Value);
        Assert.Equal(StatusPedido.Concluido, retorno.Status);
    }

    [Fact]
    public async Task Remover_DeveRetornarNoContent()
    {
        // Arrange
        int pedidoId = 1;
        _pedidoServiceMock.Setup(s => s.RemoverAsync(pedidoId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Remover(pedidoId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _pedidoServiceMock.Verify(s => s.RemoverAsync(pedidoId), Times.Once);
    }
}
