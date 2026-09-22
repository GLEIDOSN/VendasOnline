using VendasOnline.API.Data.Repositories.Interface;
using VendasOnline.API.Dtos.Request;
using VendasOnline.API.Dtos.Response;
using VendasOnline.API.Enums;
using VendasOnline.API.Models;
using VendasOnline.API.Services.Interfaces;

namespace VendasOnline.API.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IProdutoRepository _produtoRepository;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IClienteRepository clienteRepository,
        IProdutoRepository produtoRepository)
    {
        _pedidoRepository = pedidoRepository;
        _clienteRepository = clienteRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<IEnumerable<PedidoResponseDto>> ObterTodosAsync()
    {
        var pedidos = await _pedidoRepository.ObterTodosAsync();
        var lista = new List<PedidoResponseDto>();
        foreach (var p in pedidos)
        {
            var detalhado = await _pedidoRepository.ObterDetalhesPedidoAsync(p.Id);
            if (detalhado != null) lista.Add(MapToResponse(detalhado));
        }
        return lista;
    }

    public async Task<PedidoResponseDto> ObterPorIdAsync(int id)
    {
        var pedido = await _pedidoRepository.ObterDetalhesPedidoAsync(id)
            ?? throw new KeyNotFoundException($"Pedido com ID {id} não foi encontrado.");

        return MapToResponse(pedido);
    }

    public async Task<int> ContarAsync()
    {
        return await _pedidoRepository.ContarAsync();
    }

    public async Task<PedidoResponseDto> CriarAsync(PedidoRequestDto dto)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(dto.ClienteId)
            ?? throw new KeyNotFoundException($"Cliente com ID {dto.ClienteId} não existe.");

        if (dto.Itens == null || !dto.Itens.Any())
            throw new ArgumentException("O pedido deve conter ao menos um item.");

        var pedido = new Pedido
        {
            ClienteId = dto.ClienteId,
            DataPedido = DateTime.UtcNow,
            Status = StatusPedido.Pendente,
            ValorTotal = 0
        };

        decimal valorTotalCalculado = 0;

        foreach (var itemDto in dto.Itens)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(itemDto.ProdutoId)
                ?? throw new KeyNotFoundException($"Produto com ID {itemDto.ProdutoId} não foi encontrado.");

            if (produto.QuantidadeEstoque < itemDto.Quantidade)
                throw new InvalidOperationException($"Estoque insuficiente para o produto '{produto.Descricao}'. Disponível: {produto.QuantidadeEstoque}.");

            // Baixa no estoque
            produto.QuantidadeEstoque -= itemDto.Quantidade;
            await _produtoRepository.AtualizarAsync(produto);

            var itemPedido = new ItemPedido
            {
                ProdutoId = produto.Id,
                Quantidade = itemDto.Quantidade,
                PrecoUnitario = itemDto.PrecoUnitario
            };

            pedido.Itens.Add(itemPedido);
            valorTotalCalculado += itemPedido.Quantidade * itemPedido.PrecoUnitario;
        }

        pedido.ValorTotal = valorTotalCalculado;

        await _pedidoRepository.AdicionarAsync(pedido);
        await _pedidoRepository.SalvarAlteracoesAsync();

        var pedidoSalvo = await _pedidoRepository.ObterDetalhesPedidoAsync(pedido.Id);
        return MapToResponse(pedidoSalvo!);
    }

    public async Task<PedidoResponseDto> AtualizarStatusAsync(int id, StatusPedido novoStatus)
    {
        if (!Enum.IsDefined(typeof(StatusPedido), novoStatus))
            throw new ArgumentException($"O status '{novoStatus}' é inválido.");

        var pedido = await _pedidoRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException($"Pedido com ID {id} não foi encontrado.");

        pedido.Status = novoStatus;
        await _pedidoRepository.AtualizarAsync(pedido);
        await _pedidoRepository.SalvarAlteracoesAsync();

        var pedidoAtualizado = await _pedidoRepository.ObterDetalhesPedidoAsync(id);
        return MapToResponse(pedidoAtualizado!);
    }

    public async Task RemoverAsync(int id)
    {
        var pedido = await _pedidoRepository.ObterDetalhesPedidoAsync(id)
            ?? throw new KeyNotFoundException($"Pedido com ID {id} não foi encontrado.");

        foreach (var item in pedido.Itens)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(item.ProdutoId);
            produto?.QuantidadeEstoque += item.Quantidade;

            item.Produto = null!;
        }

        await _pedidoRepository.RemoverAsync(pedido);

        await _pedidoRepository.SalvarAlteracoesAsync();
    }

    private static PedidoResponseDto MapToResponse(Pedido p) =>
        new(
            p.Id,
            p.ClienteId,
            p.Cliente?.Nome ?? string.Empty,
            p.DataPedido,
            p.ValorTotal,
            p.Status,
            p.Itens.Select(i => new ItemPedidoResponseDto(
                i.ProdutoId,
                i.Produto?.Descricao ?? string.Empty,
                i.Quantidade,
                i.PrecoUnitario,
                i.Quantidade * i.PrecoUnitario
            )).ToList()
        );
}
