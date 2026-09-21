using System.Linq.Expressions;

namespace VendasOnline.API.Data.Repositories.Interface;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> ObterTodosAsync();

    Task<T?> ObterPorIdAsync(int id);

    Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicate);

    Task<int> ContarAsync();

    Task AdicionarAsync(T entidade);

    Task AtualizarAsync(T entidade);

    Task RemoverAsync(T entidade);

    Task<bool> SalvarAlteracoesAsync();
}
