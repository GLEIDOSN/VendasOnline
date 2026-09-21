using VendasOnline.API.Data.Repositories.Interface;
using VendasOnline.API.Dtos.Request;
using VendasOnline.API.Dtos.Response;
using VendasOnline.API.Models;
using VendasOnline.API.Services.Interfaces;

namespace VendasOnline.API.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<IEnumerable<ClienteResponseDto>> ObterTodosAsync()
    {
        var clientes = await _clienteRepository.ObterTodosAsync();
        return clientes.Select(MapToResponse);
    }

    public async Task<ClienteResponseDto> ObterPorIdAsync(int id)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException($"Cliente com ID {id} não foi encontrado.");

        return MapToResponse(cliente);
    }

    public async Task<IEnumerable<ClienteResponseDto>> ObterPorNomeAsync(string nome)
    {
        var clientes = await _clienteRepository.ObterPorNomeAsync(nome);
        return clientes.Select(MapToResponse);
    }

    public async Task<int> ContarAsync()
    {
        return await _clienteRepository.ContarAsync();
    }

    public async Task<ClienteResponseDto> CriarAsync(ClienteRequestDto dto)
    {
        var cliente = new Cliente
        {
            Nome = dto.Nome,
            Email = dto.Email,
            CpfCnpj = dto.CpfCnpj,
            Telefone = dto.Telefone,
            DataCadastro = DateTime.UtcNow,
            Ativo = true
        };

        await _clienteRepository.AdicionarAsync(cliente);
        await _clienteRepository.SalvarAlteracoesAsync();

        return MapToResponse(cliente);
    }

    public async Task<ClienteResponseDto> AtualizarAsync(int id, ClienteRequestDto dto)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException($"Cliente com ID {id} não foi encontrado.");

        cliente.Nome = dto.Nome;
        cliente.Email = dto.Email;
        cliente.CpfCnpj = dto.CpfCnpj;
        cliente.Telefone = dto.Telefone;

        await _clienteRepository.AtualizarAsync(cliente);
        await _clienteRepository.SalvarAlteracoesAsync();

        return MapToResponse(cliente);
    }

    public async Task RemoverAsync(int id)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException($"Cliente com ID {id} não foi encontrado.");

        await _clienteRepository.RemoverAsync(cliente);
        await _clienteRepository.SalvarAlteracoesAsync();
    }

    private static ClienteResponseDto MapToResponse(Cliente c) =>
        new(c.Id, c.Nome, c.Email, c.CpfCnpj, c.Telefone, c.DataCadastro, c.Ativo);
}
