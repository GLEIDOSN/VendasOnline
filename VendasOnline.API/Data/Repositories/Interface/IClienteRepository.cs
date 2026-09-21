using VendasOnline.API.Models;

namespace VendasOnline.API.Data.Repositories.Interface;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<IEnumerable<Cliente>> ObterPorNomeAsync(string nome);

    Task<Cliente?> ObterComPedidosAsync(int id);
}
