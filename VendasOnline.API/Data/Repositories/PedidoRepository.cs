using Microsoft.EntityFrameworkCore;
using VendasOnline.API.Data.Repositories.Interface;
using VendasOnline.API.Models;

namespace VendasOnline.API.Data.Repositories;

public class PedidoRepository : Repository<Pedido>, IPedidoRepository
{
    public PedidoRepository(VendasOnlineDbContext context) : base(context) { }

    public async Task<Pedido?> ObterDetalhesPedidoAsync(int id)
    {
        return await _dbSet.AsNoTracking()
                           .Include(p => p.Cliente)
                           .Include(p => p.Itens)
                               .ThenInclude(i => i.Produto)
                           .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Pedido>> ObterPedidosPorClienteAsync(int clienteId)
    {
        return await _dbSet.AsNoTracking()
                           .Where(p => p.ClienteId == clienteId)
                           .ToListAsync();
    }
}
