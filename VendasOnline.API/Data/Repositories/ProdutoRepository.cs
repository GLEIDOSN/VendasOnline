using Microsoft.EntityFrameworkCore;
using VendasOnline.API.Data.Repositories.Interface;
using VendasOnline.API.Models;

namespace VendasOnline.API.Data.Repositories;

public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    public ProdutoRepository(VendasOnlineDbContext context) : base(context) { }

    public async Task<IEnumerable<Produto>> ObterPorNomeAsync(string nome)
    {
        return await _dbSet.AsNoTracking()
                           .Where(p => EF.Functions.ILike(p.Descricao, $"%{nome}%"))
                           .ToListAsync();
    }
}
