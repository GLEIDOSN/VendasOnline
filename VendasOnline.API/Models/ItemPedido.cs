using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendasOnline.API.Models;

[Table("ItensPedido")]
public class ItemPedido
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int PedidoId { get; set; }

    [ForeignKey(nameof(PedidoId))]
    public Pedido Pedido { get; set; } = null!;

    [Required]
    public int ProdutoId { get; set; }

    [ForeignKey(nameof(ProdutoId))]
    public Produto Produto { get; set; } = null!;

    [Required]
    [Range(1, 1000)]
    public int Quantidade { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecoUnitario { get; set; }
}
