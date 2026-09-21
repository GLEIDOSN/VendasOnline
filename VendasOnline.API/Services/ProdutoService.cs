using VendasOnline.API.Data.Repositories.Interface;
using VendasOnline.API.Dtos.Request;
using VendasOnline.API.Dtos.Response;
using VendasOnline.API.Models;
using VendasOnline.API.Services.Interfaces;

namespace VendasOnline.API.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;

    public ProdutoService(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<IEnumerable<ProdutoResponseDto>> ObterTodosAsync()
    {
        var produtos = await _produtoRepository.ObterTodosAsync();
        return produtos.Select(MapToResponse);
    }

    public async Task<ProdutoResponseDto> ObterPorIdAsync(int id)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException($"Produto com ID {id} não foi encontrado.");

        return MapToResponse(produto);
    }

    public async Task<IEnumerable<ProdutoResponseDto>> ObterPorNomeAsync(string nome)
    {
        var produtos = await _produtoRepository.ObterPorNomeAsync(nome);
        return produtos.Select(MapToResponse);
    }

    public async Task<int> ContarAsync()
    {
        return await _produtoRepository.ContarAsync();
    }

    public async Task<ProdutoResponseDto> CriarAsync(ProdutoRequestDto dto)
    {
        var produto = new Produto
        {
            Descricao = dto.Descricao,
            Preco = dto.Preco,
            QuantidadeEstoque = dto.QuantidadeEstoque,
            DataCriacao = DateTime.UtcNow,
            Ativo = true
        };

        await _produtoRepository.AdicionarAsync(produto);
        await _produtoRepository.SalvarAlteracoesAsync();

        return MapToResponse(produto);
    }

    public async Task<ProdutoResponseDto> AtualizarAsync(int id, ProdutoRequestDto dto)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException($"Produto com ID {id} não foi encontrado.");

        produto.Descricao = dto.Descricao;
        produto.Preco = dto.Preco;
        produto.QuantidadeEstoque = dto.QuantidadeEstoque;

        await _produtoRepository.AtualizarAsync(produto);
        await _produtoRepository.SalvarAlteracoesAsync();

        return MapToResponse(produto);
    }

    public async Task RemoverAsync(int id)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException($"Produto com ID {id} não foi encontrado.");

        await _produtoRepository.RemoverAsync(produto);
        await _produtoRepository.SalvarAlteracoesAsync();
    }

    private static ProdutoResponseDto MapToResponse(Produto p) =>
        new(p.Id, p.Descricao, p.Preco, p.QuantidadeEstoque, p.DataCriacao, p.Ativo);
}
