using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using VendasOnline.API.Controllers;
using VendasOnline.API.Dtos.Request;
using VendasOnline.API.Dtos.Response;
using VendasOnline.API.Services.Interfaces;

namespace VendasOnline.Tests.Controllers;

public class ClientesControllerTests
{
    private readonly Mock<IClienteService> _clienteServiceMock;
    private readonly ClientesController _controller;

    public ClientesControllerTests()
    {
        _clienteServiceMock = new Mock<IClienteService>();
        _controller = new ClientesController(_clienteServiceMock.Object);
    }

    [Fact]
    public async Task ObterTodos_DeveRetornarOkComListaDeClientes()
    {
        // Arrange
        var clientesDto = new List<ClienteResponseDto>
        {
            new(1, "Carlos Silva", "carlos@email.com", "12345678900", "11999998888", DateTime.UtcNow, true),
            new(2, "Ana Souza", "ana@email.com", "98765432100", "11977776666", DateTime.UtcNow, true)
        };

        _clienteServiceMock.Setup(s => s.ObterTodosAsync()).ReturnsAsync(clientesDto);

        // Act
        var result = await _controller.ObterTodos();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var retorno = Assert.IsAssignableFrom<IEnumerable<ClienteResponseDto>>(okResult.Value);
        Assert.Equal(2, retorno.Count());
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarOkComCliente()
    {
        // Arrange
        int clienteId = 1;
        var clienteDto = new ClienteResponseDto(clienteId, "Carlos Silva", "carlos@email.com", "12345678900", "11999998888", DateTime.UtcNow, true);

        _clienteServiceMock.Setup(s => s.ObterPorIdAsync(clienteId)).ReturnsAsync(clienteDto);

        // Act
        var result = await _controller.ObterPorId(clienteId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var retorno = Assert.IsType<ClienteResponseDto>(okResult.Value);
        Assert.Equal(clienteId, retorno.Id);
    }

    [Fact]
    public async Task ObterPorNome_DeveRetornarOkComClientesFiltrados()
    {
        // Arrange
        string nomeBusca = "Maria";
        var clientesDto = new List<ClienteResponseDto>
        {
            new(1, "Maria Oliveira", "maria@email.com", null, null, DateTime.UtcNow, true)
        };

        _clienteServiceMock.Setup(s => s.ObterPorNomeAsync(nomeBusca)).ReturnsAsync(clientesDto);

        // Act
        var result = await _controller.ObterPorNome(nomeBusca);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var retorno = Assert.IsAssignableFrom<IEnumerable<ClienteResponseDto>>(okResult.Value);
        Assert.Single(retorno);
    }

    [Fact]
    public async Task Contar_DeveRetornarOkComTotalDeClientes()
    {
        // Arrange
        _clienteServiceMock.Setup(s => s.ContarAsync()).ReturnsAsync(10);

        // Act
        var result = await _controller.Contar();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Criar_DeveRetornarCreatedAtAction_ComNovoCliente()
    {
        // Arrange
        var requestDto = new ClienteRequestDto("Fernanda Costa", "fernanda@email.com", "11122233344", "11988887777");
        var responseDto = new ClienteResponseDto(1, "Fernanda Costa", "fernanda@email.com", "11122233344", "11988887777", DateTime.UtcNow, true);

        _clienteServiceMock.Setup(s => s.CriarAsync(requestDto)).ReturnsAsync(responseDto);

        // Act
        var result = await _controller.Criar(requestDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(ClientesController.ObterPorId), createdResult.ActionName);
        Assert.Equal(1, createdResult.RouteValues?["id"]);
        var retorno = Assert.IsType<ClienteResponseDto>(createdResult.Value);
        Assert.Equal("Fernanda Costa", retorno.Nome);
    }

    [Fact]
    public async Task Atualizar_DeveRetornarOkComClienteAtualizado()
    {
        // Arrange
        int clienteId = 1;
        var requestDto = new ClienteRequestDto("Carlos Atualizado", "carlos@email.com", "12345678900", "11999998888");
        var responseDto = new ClienteResponseDto(clienteId, "Carlos Atualizado", "carlos@email.com", "12345678900", "11999998888", DateTime.UtcNow, true);

        _clienteServiceMock.Setup(s => s.AtualizarAsync(clienteId, requestDto)).ReturnsAsync(responseDto);

        // Act
        var result = await _controller.Atualizar(clienteId, requestDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var retorno = Assert.IsType<ClienteResponseDto>(okResult.Value);
        Assert.Equal("Carlos Atualizado", retorno.Nome);
    }

    [Fact]
    public async Task Remover_DeveRetornarNoContent()
    {
        // Arrange
        int clienteId = 1;
        _clienteServiceMock.Setup(s => s.RemoverAsync(clienteId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Remover(clienteId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _clienteServiceMock.Verify(s => s.RemoverAsync(clienteId), Times.Once);
    }
}
