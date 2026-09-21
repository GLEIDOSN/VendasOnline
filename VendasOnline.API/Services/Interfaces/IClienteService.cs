using VendasOnline.API.Dtos.Request;
using VendasOnline.API.Dtos.Response;

namespace VendasOnline.API.Services.Interfaces;

public interface IClienteService
{
    Task<IEnumerable<ClienteResponseDto>> ObterTodosAsync();

    Task<ClienteResponseDto> ObterPorIdAsync(int id);

    Task<IEnumerable<ClienteResponseDto>> ObterPorNomeAsync(string nome);

    Task<int> ContarAsync();

    Task<ClienteResponseDto> CriarAsync(ClienteRequestDto dto);

    Task<ClienteResponseDto> AtualizarAsync(int id, ClienteRequestDto dto);

    Task RemoverAsync(int id);
}
