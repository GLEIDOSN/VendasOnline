using VendasOnline.API.Models;

namespace VendasOnline.API.Data.Repositories.Interface;

public interface IPedidoRepository : IRepository<Pedido>
{
    Task<Pedido?> ObterDetalhesPedidoAsync(int id);

    Task<IEnumerable<Pedido>> ObterPedidosPorClienteAsync(int clienteId);
}
