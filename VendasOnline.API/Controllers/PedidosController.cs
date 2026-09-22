using Microsoft.AspNetCore.Mvc;
using VendasOnline.API.Dtos.Request;
using VendasOnline.API.Dtos.Response;
using VendasOnline.API.Enums;
using VendasOnline.API.Services.Interfaces;

namespace VendasOnline.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    // GET: api/pedidos (Find All)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PedidoResponseDto>>> ObterTodos()
    {
        var pedidos = await _pedidoService.ObterTodosAsync();
        return Ok(pedidos);
    }

    // GET: api/pedidos/5 (Find By ID)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PedidoResponseDto>> ObterPorId(int id)
    {
        var pedido = await _pedidoService.ObterPorIdAsync(id);
        return Ok(pedido);
    }

    // GET: api/pedidos/contar (Count)
    [HttpGet("contar")]
    public async Task<ActionResult<object>> Contar()
    {
        var total = await _pedidoService.ContarAsync();
        return Ok(new { TotalPedidos = total });
    }

    // POST: api/pedidos (Create)
    [HttpPost]
    public async Task<ActionResult<PedidoResponseDto>> Criar([FromBody] PedidoRequestDto dto)
    {
        var novoPedido = await _pedidoService.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = novoPedido.Id }, novoPedido);
    }

    // PATCH: api/pedidos/5/status (Update Status)
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<PedidoResponseDto>> AtualizarStatus(int id, [FromBody] StatusPedido novoStatus)
    {
        var pedidoAtualizado = await _pedidoService.AtualizarStatusAsync(id, novoStatus);
        return Ok(pedidoAtualizado);
    }

    // DELETE: api/pedidos/5 (Delete)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        await _pedidoService.RemoverAsync(id);
        return NoContent();
    }
}
