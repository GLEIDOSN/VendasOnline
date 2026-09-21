using VendasOnline.API.Models;

namespace VendasOnline.API.Data.Repositories.Interface;

public interface IProdutoRepository : IRepository<Produto>
{
    Task<IEnumerable<Produto>> ObterPorNomeAsync(string nome);
}
