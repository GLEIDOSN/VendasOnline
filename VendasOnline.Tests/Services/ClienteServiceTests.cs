using Moq;
using VendasOnline.API.Data.Repositories.Interface;
using VendasOnline.API.Dtos.Request;
using VendasOnline.API.Models;
using VendasOnline.API.Services;

namespace VendasOnline.Tests.Services;

public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly ClienteService _clienteService;

    public ClienteServiceTests()
    {
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _clienteService = new ClienteService(_clienteRepositoryMock.Object);
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarListaDeClientesResponseDto()
    {
        // Arrange
        var clientesFakes = new List<Cliente>
        {
            new() { Id = 1, Nome = "Carlos Silva", Email = "carlos@email.com", Ativo = true },
            new() { Id = 2, Nome = "Ana Souza", Email = "ana@email.com", Ativo = true }
        };

        _clienteRepositoryMock
            .Setup(r => r.ObterTodosAsync())
            .ReturnsAsync(clientesFakes);

        // Act
        var resultado = await _clienteService.ObterTodosAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
        _clienteRepositoryMock.Verify(r => r.ObterTodosAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarCliente_QuandoIdExistir()
    {
        // Arrange
        int clienteId = 1;
        var clienteFake = new Cliente
        {
            Id = clienteId,
            Nome = "Carlos Silva",
            Email = "carlos@email.com",
            CpfCnpj = "12345678900",
            Telefone = "11999998888",
            DataCadastro = DateTime.UtcNow,
            Ativo = true
        };

        _clienteRepositoryMock
            .Setup(r => r.ObterPorIdAsync(clienteId))
            .ReturnsAsync(clienteFake);

        // Act
        var resultado = await _clienteService.ObterPorIdAsync(clienteId);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(clienteId, resultado.Id);
        Assert.Equal("Carlos Silva", resultado.Nome);
        Assert.Equal("carlos@email.com", resultado.Email);
        _clienteRepositoryMock.Verify(r => r.ObterPorIdAsync(clienteId), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveLancarKeyNotFoundException_QuandoIdNaoExistir()
    {
        // Arrange
        int idInexistente = 99;

        _clienteRepositoryMock
            .Setup(r => r.ObterPorIdAsync(idInexistente))
            .ReturnsAsync((Cliente?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _clienteService.ObterPorIdAsync(idInexistente)
        );
    }

    [Fact]
    public async Task ObterPorNomeAsync_DeveRetornarClientes_QuandoNomeEncontrado()
    {
        // Arrange
        string termoBusca = "Maria";
        var clientesFakes = new List<Cliente>
        {
            new Cliente { Id = 1, Nome = "Maria Oliveira", Email = "maria@email.com" },
            new Cliente { Id = 2, Nome = "Maria Clara", Email = "mclara@email.com" }
        };

        _clienteRepositoryMock
            .Setup(r => r.ObterPorNomeAsync(termoBusca))
            .ReturnsAsync(clientesFakes);

        // Act
        var resultado = await _clienteService.ObterPorNomeAsync(termoBusca);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
        Assert.All(resultado, c => Assert.Contains("Maria", c.Nome));
        _clienteRepositoryMock.Verify(r => r.ObterPorNomeAsync(termoBusca), Times.Once);
    }

    [Fact]
    public async Task ContarAsync_DeveRetornarTotalDeClientes()
    {
        // Arrange
        _clienteRepositoryMock
            .Setup(r => r.ContarAsync())
            .ReturnsAsync(15);

        // Act
        var total = await _clienteService.ContarAsync();

        // Assert
        Assert.Equal(15, total);
        _clienteRepositoryMock.Verify(r => r.ContarAsync(), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveAdicionarClienteESalvar()
    {
        // Arrange
        var requestDto = new ClienteRequestDto("Fernanda Costa", "fernanda@email.com", "98765432100", "11977776666");

        _clienteRepositoryMock
            .Setup(r => r.AdicionarAsync(It.IsAny<Cliente>()))
            .Returns(Task.CompletedTask);

        _clienteRepositoryMock
            .Setup(r => r.SalvarAlteracoesAsync())
            .ReturnsAsync(true);

        // Act
        var resultado = await _clienteService.CriarAsync(requestDto);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Fernanda Costa", resultado.Nome);
        Assert.Equal("fernanda@email.com", resultado.Email);
        Assert.Equal("98765432100", resultado.CpfCnpj);
        Assert.Equal("11977776666", resultado.Telefone);
        Assert.True(resultado.Ativo);

        _clienteRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Cliente>()), Times.Once);
        _clienteRepositoryMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarClienteESalvar_QuandoIdExistir()
    {
        // Arrange
        int clienteId = 5;
        var clienteExistente = new Cliente
        {
            Id = clienteId,
            Nome = "Nome Antigo",
            Email = "antigo@email.com",
            CpfCnpj = "00000000000",
            Telefone = "1100000000"
        };

        var updateDto = new ClienteRequestDto("Nome Atualizado", "novo@email.com", "11122233344", "11988887777");

        _clienteRepositoryMock
            .Setup(r => r.ObterPorIdAsync(clienteId))
            .ReturnsAsync(clienteExistente);

        _clienteRepositoryMock
            .Setup(r => r.AtualizarAsync(clienteExistente))
            .Returns(Task.CompletedTask);

        _clienteRepositoryMock
            .Setup(r => r.SalvarAlteracoesAsync())
            .ReturnsAsync(true);

        // Act
        var resultado = await _clienteService.AtualizarAsync(clienteId, updateDto);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Nome Atualizado", resultado.Nome);
        Assert.Equal("novo@email.com", resultado.Email);
        Assert.Equal("11122233344", resultado.CpfCnpj);
        Assert.Equal("11988887777", resultado.Telefone);

        _clienteRepositoryMock.Verify(r => r.AtualizarAsync(clienteExistente), Times.Once);
        _clienteRepositoryMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarKeyNotFoundException_QuandoIdNaoExistir()
    {
        // Arrange
        int idInexistente = 99;
        var updateDto = new ClienteRequestDto("Cliente Teste", "teste@email.com", null, null);

        _clienteRepositoryMock
            .Setup(r => r.ObterPorIdAsync(idInexistente))
            .ReturnsAsync((Cliente?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _clienteService.AtualizarAsync(idInexistente, updateDto)
        );
    }

    [Fact]
    public async Task RemoverAsync_DeveRemoverClienteESalvar_QuandoIdExistir()
    {
        // Arrange
        int clienteId = 3;
        var clienteExistente = new Cliente { Id = clienteId, Nome = "Cliente Para Remocao" };

        _clienteRepositoryMock
            .Setup(r => r.ObterPorIdAsync(clienteId))
            .ReturnsAsync(clienteExistente);

        _clienteRepositoryMock
            .Setup(r => r.RemoverAsync(clienteExistente))
            .Returns(Task.CompletedTask);

        _clienteRepositoryMock
            .Setup(r => r.SalvarAlteracoesAsync())
            .ReturnsAsync(true);

        // Act
        await _clienteService.RemoverAsync(clienteId);

        // Assert
        _clienteRepositoryMock.Verify(r => r.RemoverAsync(clienteExistente), Times.Once);
        _clienteRepositoryMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveLancarKeyNotFoundException_QuandoIdNaoExistir()
    {
        // Arrange
        int idInexistente = 99;

        _clienteRepositoryMock
            .Setup(r => r.ObterPorIdAsync(idInexistente))
            .ReturnsAsync((Cliente?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _clienteService.RemoverAsync(idInexistente)
        );
    }
}