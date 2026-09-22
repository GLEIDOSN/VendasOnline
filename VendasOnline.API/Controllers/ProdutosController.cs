using Microsoft.AspNetCore.Mvc;
using VendasOnline.API.Dtos.Request;
using VendasOnline.API.Dtos.Response;
using VendasOnline.API.Services.Interfaces;

namespace VendasOnline.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController(IProdutoService produtoService) : ControllerBase
{
    private readonly IProdutoService _produtoService = produtoService;

    // GET: api/produtos (Find All)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoResponseDto>>> ObterTodos()
    {
        var produtos = await _produtoService.ObterTodosAsync();
        return Ok(produtos);
    }

    // GET: api/produtos/5 (Find By ID)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProdutoResponseDto>> ObterPorId(int id)
    {
        var produto = await _produtoService.ObterPorIdAsync(id);
        return Ok(produto);
    }

    // GET: api/produtos/busca-nome?nome=Teclado (Find By Name)
    [HttpGet("busca-nome")]
    public async Task<ActionResult<IEnumerable<ProdutoResponseDto>>> ObterPorNome([FromQuery] string nome)
    {
        var produtos = await _produtoService.ObterPorNomeAsync(nome);
        return Ok(produtos);
    }

    // GET: api/produtos/contar (Count)
    [HttpGet("contar")]
    public async Task<ActionResult<object>> Contar()
    {
        var total = await _produtoService.ContarAsync();
        return Ok(new { TotalProdutos = total });
    }

    // POST: api/produtos (Create)
    [HttpPost]
    public async Task<ActionResult<ProdutoResponseDto>> Criar([FromBody] ProdutoRequestDto dto)
    {
        var novoProduto = await _produtoService.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = novoProduto.Id }, novoProduto);
    }

    // PUT: api/produtos/5 (Update)
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProdutoResponseDto>> Atualizar(int id, [FromBody] ProdutoRequestDto dto)
    {
        var produtoAtualizado = await _produtoService.AtualizarAsync(id, dto);
        return Ok(produtoAtualizado);
    }

    // DELETE: api/produtos/5 (Delete)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        await _produtoService.RemoverAsync(id);
        return NoContent();
    }
}