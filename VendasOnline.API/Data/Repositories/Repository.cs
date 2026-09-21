using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VendasOnline.API.Data.Repositories.Interface;

namespace VendasOnline.API.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly VendasOnlineDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(VendasOnlineDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> ObterTodosAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<T?> ObterPorIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
    }

    public async Task<int> ContarAsync()
    {
        return await _dbSet.CountAsync();
    }

    public async Task AdicionarAsync(T entidade)
    {
        await _dbSet.AddAsync(entidade);
    }

    public async Task AtualizarAsync(T entidade)
    {
        _dbSet.Update(entidade);

        await Task.CompletedTask;
    }

    public async Task RemoverAsync(T entidade)
    {
        _dbSet.Remove(entidade);

        await Task.CompletedTask;
    }

    public async Task<bool> SalvarAlteracoesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
