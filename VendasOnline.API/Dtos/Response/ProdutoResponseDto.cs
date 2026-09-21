namespace VendasOnline.API.Dtos.Response;

public record ProdutoResponseDto(
    int Id,
    string Descricao,
    decimal Preco,
    int QuantidadeEstoque,
    DateTime DataCriacao,
    bool Ativo
);
