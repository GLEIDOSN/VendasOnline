using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VendasOnline.API.Enums;

namespace VendasOnline.API.Models;

[Table("Pedidos")]
public class Pedido
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int ClienteId { get; set; }

    [ForeignKey(nameof(ClienteId))]
    public Cliente Cliente { get; set; } = null!;

    public DateTime DataPedido { get; set; } = DateTime.UtcNow;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorTotal { get; set; }

    [Required]
    [EnumDataType(typeof(StatusPedido), ErrorMessage = "O status informado é inválido. Valores permitidos: Pendente, Processando, Concluido, Cancelado.")]
    public StatusPedido Status { get; set; } = StatusPedido.Pendente;

    public ICollection<ItemPedido> Itens { get; set; } = [];
}
