using System.ComponentModel.DataAnnotations;

namespace VendasOnline.API.Dtos.Request;

public record ProdutoRequestDto(
    [Required(ErrorMessage = "A descrição do produto é obrigatória.")]
    [StringLength(150)]
    string Descricao,

    [Range(0.01, 999999.99, ErrorMessage = "O preço deve ser maior que zero.")]
    decimal Preco,

    [Range(0, int.MaxValue, ErrorMessage = "A quantidade em estoque não pode ser negativa.")]
    int QuantidadeEstoque
);
