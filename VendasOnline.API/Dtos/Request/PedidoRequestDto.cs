using System.ComponentModel.DataAnnotations;

namespace VendasOnline.API.Dtos.Request;

public record PedidoRequestDto(
    [Required] int ClienteId,
    [Required] List<ItemPedidoRequestDto> Itens
);
