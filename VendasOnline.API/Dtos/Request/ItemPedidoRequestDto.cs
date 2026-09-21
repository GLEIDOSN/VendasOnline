using System.ComponentModel.DataAnnotations;

namespace VendasOnline.API.Dtos.Request;

public record ItemPedidoRequestDto(
    [Required] int ProdutoId,
    [Range(1, 1000, ErrorMessage = "A quantidade deve ser entre 1 e 1000.")] int Quantidade,
    [Required] decimal PrecoUnitario
);
