using VendasOnline.API.Enums;

namespace VendasOnline.API.Dtos.Response;

public record PedidoResponseDto(
    int Id,
    int ClienteId,
    string NomeCliente,
    DateTime DataPedido,
    decimal ValorTotal,
    StatusPedido Status,
    List<ItemPedidoResponseDto> Itens
);
