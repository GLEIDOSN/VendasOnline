using VendasOnline.API.Dtos.Request;
using VendasOnline.API.Dtos.Response;

namespace VendasOnline.API.Services.Interfaces;

public interface IProdutoService
{
    Task<IEnumerable<ProdutoResponseDto>> ObterTodosAsync();

    Task<ProdutoResponseDto> ObterPorIdAsync(int id);

    Task<IEnumerable<ProdutoResponseDto>> ObterPorNomeAsync(string nome);

    Task<int> ContarAsync();

    Task<ProdutoResponseDto> CriarAsync(ProdutoRequestDto dto);

    Task<ProdutoResponseDto> AtualizarAsync(int id, ProdutoRequestDto dto);

    Task RemoverAsync(int id);
}
