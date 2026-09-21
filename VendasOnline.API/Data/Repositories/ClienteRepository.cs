using Microsoft.EntityFrameworkCore;
using VendasOnline.API.Data.Repositories.Interface;
using VendasOnline.API.Models;

namespace VendasOnline.API.Data.Repositories;

public class ClienteRepository : Repository<Cliente>, IClienteRepository
{
    public ClienteRepository(VendasOnlineDbContext context) : base(context) { }

    public async Task<IEnumerable<Cliente>> ObterPorNomeAsync(string nome)
    {
        return await _dbSet.AsNoTracking()
                           .Where(c => EF.Functions.ILike(c.Nome, $"%{nome}%"))
                           .ToListAsync();
    }

    public async Task<Cliente?> ObterComPedidosAsync(int id)
    {
        return await _dbSet.AsNoTracking()
                           .Include(c => c.Pedidos)
                           .FirstOrDefaultAsync(c => c.Id == id);
    }
}
