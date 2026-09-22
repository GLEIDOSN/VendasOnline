using Moq;
using VendasOnline.API.Data.Repositories.Interface;
using VendasOnline.API.Dtos.Request;
using VendasOnline.API.Enums;
using VendasOnline.API.Models;
using VendasOnline.API.Services;

namespace VendasOnline.Tests.Services;

public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
    private readonly PedidoService _pedidoService;

    public PedidoServiceTests()
    {
        _pedidoRepositoryMock = new Mock<IPedidoRepository>();
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _produtoRepositoryMock = new Mock<IProdutoRepository>();

        _pedidoService = new PedidoService(
            _pedidoRepositoryMock.Object,
            _clienteRepositoryMock.Object,
            _produtoRepositoryMock.Object
        );
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarListaDePedidos_QuandoExistiremPedidos()
    {
        // Arrange
        var cliente = new Cliente { Id = 1, Nome = "Carlos Silva" };
        var produto = new Produto { Id = 10, Descricao = "Teclado RGB" };

        var pedidosFakes = new List<Pedido>
        {
            new() { Id = 1, ClienteId = 1 }
        };

        var pedidoDetalhado = new Pedido
        {
            Id = 1,
            ClienteId = 1,
            Cliente = cliente,
            DataPedido = DateTime.UtcNow,
            ValorTotal = 200.00m,
            Status = StatusPedido.Pendente,
            Itens =
            [
                new() { ProdutoId = 10, Produto = produto, Quantidade = 2, PrecoUnitario = 100.00m }
            ]
        };

        _pedidoRepositoryMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(pedidosFakes);
        _pedidoRepositoryMock.Setup(r => r.ObterDetalhesPedidoAsync(1)).ReturnsAsync(pedidoDetalhado);

        // Act
        var resultado = await _pedidoService.ObterTodosAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Single(resultado);
        Assert.Equal("Carlos Silva", resultado.First().NomeCliente);
        _pedidoRepositoryMock.Verify(r => r.ObterTodosAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarListaVazia_QuandoNaoExistiremPedidos()
    {
        // Arrange
        _pedidoRepositoryMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(new List<Pedido>());

        // Act
        var resultado = await _pedidoService.ObterTodosAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);
        _pedidoRepositoryMock.Verify(r => r.ObterTodosAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarPedido_QuandoIdExistir()
    {
        // Arrange
        int pedidoId = 1;
        var cliente = new Cliente { Id = 1, Nome = "Ana Souza" };
        var pedidoDetalhado = new Pedido
        {
            Id = pedidoId,
            ClienteId = 1,
            Cliente = cliente,
            DataPedido = DateTime.UtcNow,
            ValorTotal = 150.00m,
            Status = StatusPedido.Pendente,
            Itens = new List<ItemPedido>()
        };

        _pedidoRepositoryMock.Setup(r => r.ObterDetalhesPedidoAsync(pedidoId)).ReturnsAsync(pedidoDetalhado);

        // Act
        var resultado = await _pedidoService.ObterPorIdAsync(pedidoId);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(pedidoId, resultado.Id);
        Assert.Equal("Ana Souza", resultado.NomeCliente);
        _pedidoRepositoryMock.Verify(r => r.ObterDetalhesPedidoAsync(pedidoId), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveLancarKeyNotFoundException_QuandoIdNaoExistir()
    {
        // Arrange
        int idInexistente = 99;
        _pedidoRepositoryMock.Setup(r => r.ObterDetalhesPedidoAsync(idInexistente)).ReturnsAsync((Pedido?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _pedidoService.ObterPorIdAsync(idInexistente)
        );

        Assert.Equal($"Pedido com ID {idInexistente} não foi encontrado.", ex.Message);
    }

    [Fact]
    public async Task ContarAsync_DeveRetornarTotalDePedidos()
    {
        // Arrange
        _pedidoRepositoryMock.Setup(r => r.ContarAsync()).ReturnsAsync(10);

        // Act
        var total = await _pedidoService.ContarAsync();

        // Assert
        Assert.Equal(10, total);
        _pedidoRepositoryMock.Verify(r => r.ContarAsync(), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveCriarPedidoEDeduzirEstoque_ComSucesso()
    {
        // Arrange
        int clienteId = 1;
        int produtoId = 10;

        var clienteFake = new Cliente { Id = clienteId, Nome = "João Mendes" };
        var produtoFake = new Produto { Id = produtoId, Descricao = "Mouse Sem Fio", Preco = 100.00m, QuantidadeEstoque = 20 };

        var requestDto = new PedidoRequestDto(
            clienteId,
            new List<ItemPedidoRequestDto> { new(produtoId, 5, 100.00m) }
        );

        _clienteRepositoryMock.Setup(r => r.ObterPorIdAsync(clienteId)).ReturnsAsync(clienteFake);
        _produtoRepositoryMock.Setup(r => r.ObterPorIdAsync(produtoId)).ReturnsAsync(produtoFake);
        _pedidoRepositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<Pedido>())).Returns(Task.CompletedTask);
        _pedidoRepositoryMock.Setup(r => r.SalvarAlteracoesAsync()).ReturnsAsync(true);

        _pedidoRepositoryMock
            .Setup(r => r.ObterDetalhesPedidoAsync(It.IsAny<int>()))
            .ReturnsAsync(new Pedido
            {
                Id = 1,
                ClienteId = clienteId,
                Cliente = clienteFake,
                ValorTotal = 500.00m,
                Status = StatusPedido.Pendente
            });

        // Act
        var resultado = await _pedidoService.CriarAsync(requestDto);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(15, produtoFake.QuantidadeEstoque); // 20 - 5 = 15
        _produtoRepositoryMock.Verify(r => r.AtualizarAsync(produtoFake), Times.Once);
        _pedidoRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Pedido>()), Times.Once);
        _pedidoRepositoryMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveLancarKeyNotFoundException_QuandoClienteNaoExistir()
    {
        // Arrange
        int clienteIdInexistente = 99;
        var requestDto = new PedidoRequestDto(clienteIdInexistente, new List<ItemPedidoRequestDto> { new(1, 1, 50.00m) });

        _clienteRepositoryMock.Setup(r => r.ObterPorIdAsync(clienteIdInexistente)).ReturnsAsync((Cliente?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _pedidoService.CriarAsync(requestDto)
        );

        Assert.Equal($"Cliente com ID {clienteIdInexistente} não existe.", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_DeveLancarArgumentException_QuandoListaDeItensForNulaOuVazia()
    {
        // Arrange
        int clienteId = 1;
        var clienteFake = new Cliente { Id = clienteId, Nome = "Carlos" };
        var requestDto = new PedidoRequestDto(clienteId, new List<ItemPedidoRequestDto>());

        _clienteRepositoryMock.Setup(r => r.ObterPorIdAsync(clienteId)).ReturnsAsync(clienteFake);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _pedidoService.CriarAsync(requestDto)
        );

        Assert.Equal("O pedido deve conter ao menos um item.", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_DeveLancarKeyNotFoundException_QuandoProdutoNaoExistir()
    {
        // Arrange
        int clienteId = 1;
        int produtoInexistenteId = 999;

        var clienteFake = new Cliente { Id = clienteId, Nome = "Carlos" };
        var requestDto = new PedidoRequestDto(clienteId, new List<ItemPedidoRequestDto> { new(produtoInexistenteId, 1, 10.00m) });

        _clienteRepositoryMock.Setup(r => r.ObterPorIdAsync(clienteId)).ReturnsAsync(clienteFake);
        _produtoRepositoryMock.Setup(r => r.ObterPorIdAsync(produtoInexistenteId)).ReturnsAsync((Produto?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _pedidoService.CriarAsync(requestDto)
        );

        Assert.Equal($"Produto com ID {produtoInexistenteId} não foi encontrado.", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_DeveLancarInvalidOperationException_QuandoEstoqueForInsuficiente()
    {
        // Arrange
        int clienteId = 1;
        int produtoId = 10;

        var clienteFake = new Cliente { Id = clienteId, Nome = "João Mendes" };
        var produtoSemEstoque = new Produto { Id = produtoId, Descricao = "Teclado Mecânico", Preco = 250.00m, QuantidadeEstoque = 2 };

        var requestDto = new PedidoRequestDto(clienteId, new List<ItemPedidoRequestDto> { new(produtoId, 5, 250.00m) });

        _clienteRepositoryMock.Setup(r => r.ObterPorIdAsync(clienteId)).ReturnsAsync(clienteFake);
        _produtoRepositoryMock.Setup(r => r.ObterPorIdAsync(produtoId)).ReturnsAsync(produtoSemEstoque);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _pedidoService.CriarAsync(requestDto)
        );

        Assert.Equal($"Estoque insuficiente para o produto '{produtoSemEstoque.Descricao}'. Disponível: {produtoSemEstoque.QuantidadeEstoque}.", ex.Message);
    }

    [Fact]
    public async Task AtualizarStatusAsync_DeveAtualizarStatusESalvar_QuandoValido()
    {
        // Arrange
        int pedidoId = 1;
        var novoStatus = StatusPedido.Concluido;
        var pedidoExistente = new Pedido { Id = pedidoId, Status = StatusPedido.Pendente };
        var pedidoAtualizado = new Pedido { Id = pedidoId, Status = novoStatus, Cliente = new Cliente { Nome = "Maria" } };

        _pedidoRepositoryMock.Setup(r => r.ObterPorIdAsync(pedidoId)).ReturnsAsync(pedidoExistente);
        _pedidoRepositoryMock.Setup(r => r.AtualizarAsync(pedidoExistente)).Returns(Task.CompletedTask);
        _pedidoRepositoryMock.Setup(r => r.SalvarAlteracoesAsync()).ReturnsAsync(true);
        _pedidoRepositoryMock.Setup(r => r.ObterDetalhesPedidoAsync(pedidoId)).ReturnsAsync(pedidoAtualizado);

        // Act
        var resultado = await _pedidoService.AtualizarStatusAsync(pedidoId, novoStatus);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(StatusPedido.Concluido, resultado.Status);
        _pedidoRepositoryMock.Verify(r => r.AtualizarAsync(pedidoExistente), Times.Once);
        _pedidoRepositoryMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarStatusAsync_DeveLancarArgumentException_QuandoStatusForInvalido()
    {
        // Arrange
        int pedidoId = 1;
        var statusInvalido = (StatusPedido)99;

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _pedidoService.AtualizarStatusAsync(pedidoId, statusInvalido)
        );

        Assert.Equal($"O status '{statusInvalido}' é inválido.", ex.Message);
    }

    [Fact]
    public async Task AtualizarStatusAsync_DeveLancarKeyNotFoundException_QuandoPedidoNaoExistir()
    {
        // Arrange
        int idInexistente = 99;
        _pedidoRepositoryMock.Setup(r => r.ObterPorIdAsync(idInexistente)).ReturnsAsync((Pedido?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _pedidoService.AtualizarStatusAsync(idInexistente, StatusPedido.Concluido)
        );

        Assert.Equal($"Pedido com ID {idInexistente} não foi encontrado.", ex.Message);
    }

    [Fact]
    public async Task RemoverAsync_DeveRestaurarEstoqueEDeletarPedido_ComSucesso()
    {
        // Arrange
        int pedidoId = 1;
        int produtoId = 100;
        int estoqueAtual = 10;
        int qtdItem = 3;

        var produtoFake = new Produto { Id = produtoId, Descricao = "Monitor 27", QuantidadeEstoque = estoqueAtual };
        var itemPedido = new ItemPedido { ProdutoId = produtoId, Quantidade = qtdItem, Produto = produtoFake };
        var pedidoFake = new Pedido { Id = pedidoId, Itens = new List<ItemPedido> { itemPedido } };

        _pedidoRepositoryMock.Setup(r => r.ObterDetalhesPedidoAsync(pedidoId)).ReturnsAsync(pedidoFake);
        _produtoRepositoryMock.Setup(r => r.ObterPorIdAsync(produtoId)).ReturnsAsync(produtoFake);
        _pedidoRepositoryMock.Setup(r => r.RemoverAsync(pedidoFake)).Returns(Task.CompletedTask);
        _pedidoRepositoryMock.Setup(r => r.SalvarAlteracoesAsync()).ReturnsAsync(true);

        // Act
        await _pedidoService.RemoverAsync(pedidoId);

        // Assert
        Assert.Equal(13, produtoFake.QuantidadeEstoque); // 10 + 3 = 13 (Devolvido!)
        Assert.Null(itemPedido.Produto); // Desvinculado em memória para não dar erro no ChangeTracker
        _pedidoRepositoryMock.Verify(r => r.RemoverAsync(pedidoFake), Times.Once);
        _pedidoRepositoryMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveRemoverPedidoMesmoQuandoProdutoJaFoiExcluido()
    {
        // Arrange
        int pedidoId = 1;
        int produtoIdRemovido = 200;

        var itemPedido = new ItemPedido { ProdutoId = produtoIdRemovido, Quantidade = 2 };
        var pedidoFake = new Pedido { Id = pedidoId, Itens = [itemPedido] };

        _pedidoRepositoryMock.Setup(r => r.ObterDetalhesPedidoAsync(pedidoId)).ReturnsAsync(pedidoFake);
        _produtoRepositoryMock.Setup(r => r.ObterPorIdAsync(produtoIdRemovido)).ReturnsAsync((Produto?)null);
        _pedidoRepositoryMock.Setup(r => r.RemoverAsync(pedidoFake)).Returns(Task.CompletedTask);

        // Act
        await _pedidoService.RemoverAsync(pedidoId);

        // Assert
        _pedidoRepositoryMock.Verify(r => r.RemoverAsync(pedidoFake), Times.Once);
        _pedidoRepositoryMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveLancarKeyNotFoundException_QuandoPedidoNaoExistir()
    {
        // Arrange
        int idInexistente = 99;
        _pedidoRepositoryMock.Setup(r => r.ObterDetalhesPedidoAsync(idInexistente)).ReturnsAsync((Pedido?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _pedidoService.RemoverAsync(idInexistente)
        );

        Assert.Equal($"Pedido com ID {idInexistente} não foi encontrado.", ex.Message);
    }
}