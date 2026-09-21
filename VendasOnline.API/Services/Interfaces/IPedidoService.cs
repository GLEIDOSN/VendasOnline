using VendasOnline.API.Dtos.Request;
using VendasOnline.API.Dtos.Response;
using VendasOnline.API.Enums;

namespace VendasOnline.API.Services.Interfaces;

public interface IPedidoService
{
    Task<IEnumerable<PedidoResponseDto>> ObterTodosAsync();

    Task<PedidoResponseDto> ObterPorIdAsync(int id);

    Task<int> ContarAsync();

    Task<PedidoResponseDto> CriarAsync(PedidoRequestDto dto);

    Task<PedidoResponseDto> AtualizarStatusAsync(int id, StatusPedido novoStatus);

    Task RemoverAsync(int id);
}
