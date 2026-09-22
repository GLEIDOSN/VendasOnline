using Microsoft.AspNetCore.Mvc;
using VendasOnline.API.Dtos.Request;
using VendasOnline.API.Dtos.Response;
using VendasOnline.API.Services.Interfaces;

namespace VendasOnline.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController(IClienteService clienteService) : Controller
{
    private readonly IClienteService _clienteService = clienteService;

    // GET: api/clientes (Find All)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> ObterTodos()
    {
        var clientes = await _clienteService.ObterTodosAsync();
        return Ok(clientes);
    }

    // GET: api/clientes/5 (Find By ID)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteResponseDto>> ObterPorId(int id)
    {
        var cliente = await _clienteService.ObterPorIdAsync(id);
        return Ok(cliente);
    }

    // GET: api/clientes/busca-nome?nome=Maria (Find By Name)
    [HttpGet("busca-nome")]
    public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> ObterPorNome([FromQuery] string nome)
    {
        var clientes = await _clienteService.ObterPorNomeAsync(nome);
        return Ok(clientes);
    }

    // GET: api/clientes/contar (Count)
    [HttpGet("contar")]
    public async Task<ActionResult<object>> Contar()
    {
        var total = await _clienteService.ContarAsync();
        return Ok(new { TotalClientes = total });
    }

    // POST: api/clientes (Create)
    [HttpPost]
    public async Task<ActionResult<ClienteResponseDto>> Criar([FromBody] ClienteRequestDto dto)
    {
        var novoCliente = await _clienteService.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = novoCliente.Id }, novoCliente);
    }

    // PUT: api/clientes/5 (Update)
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ClienteResponseDto>> Atualizar(int id, [FromBody] ClienteRequestDto dto)
    {
        var clienteAtualizado = await _clienteService.AtualizarAsync(id, dto);
        return Ok(clienteAtualizado);
    }

    // DELETE: api/clientes/5 (Delete)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        await _clienteService.RemoverAsync(id);
        return NoContent();
    }
}
