using System.ComponentModel.DataAnnotations;

namespace VendasOnline.API.Dtos.Request;

public record ClienteRequestDto(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    string Nome,

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail em formato inválido.")]
    string Email,

    string? CpfCnpj,
    string? Telefone
);
