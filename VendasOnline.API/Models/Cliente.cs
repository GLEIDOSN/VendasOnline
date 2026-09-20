using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendasOnline.API.Models;

[Table("Clientes")]
public class Cliente
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail em formato inválido.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [StringLength(14)]
    public string? CpfCnpj { get; set; }

    [StringLength(20)]
    public string? Telefone { get; set; }

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public bool Ativo { get; set; } = true;

    // Relacionamento 1:N -> Um cliente pode ter vários pedidos
    public ICollection<Pedido> Pedidos { get; set; } = [];
}
