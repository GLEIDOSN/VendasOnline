namespace VendasOnline.API.Dtos.Response;

public record ItemPedidoResponseDto(
    int ProdutoId,
    string DescricaoProduto,
    int Quantidade,
    decimal PrecoUnitario,
    decimal Subtotal
);
