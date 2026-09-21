namespace VendasOnline.API.Dtos.Response;

public record ClienteResponseDto(
    int Id,
    string Nome,
    string Email,
    string? CpfCnpj,
    string? Telefone,
    DateTime DataCadastro,
    bool Ativo
);
