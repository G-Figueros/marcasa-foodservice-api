using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MarcasaFoodService.Api.Models.Common;
using MarcasaFoodService.Api.Models.Operations;
using MarcasaFoodService.Api.Services.Operations;
using Microsoft.AspNetCore.Authorization;

namespace MarcasaFoodService.Api.Controllers;

[Route("api/pedidos")]
public sealed class PedidosController(IOperationsService operations) : ApiControllerBase
{
    [HttpGet("pendientes-autorizacion")]
    [Authorize(Policy = Permissions.VerPendientes)]
    public async Task<IActionResult> PendientesAutorizacion([FromQuery] PedidoFiltro filtro,
        CancellationToken ct) => Exito(await operations.PendientesAutorizacionAsync(filtro, ct));

    [HttpGet("autorizados-ruta")]
    [Authorize(Policy = Permissions.VerAutorizados)]
    public async Task<IActionResult> AutorizadosRuta([FromQuery] PedidoFiltro filtro,
        CancellationToken ct) => Exito(await operations.AutorizadosRutaAsync(filtro, ct));

    [HttpGet("{id:long}")]
    [Authorize(Policy = Permissions.VerPedido)]
    public async Task<IActionResult> Obtener(long id, CancellationToken ct)
    {
        var pedido = await operations.PedidoAsync(id, ct);
        return pedido is null ? NotFound() : Exito(pedido);
    }

    [HttpGet("{id:long}/detalle")]
    [Authorize(Policy = Permissions.VerPedido)]
    public async Task<IActionResult> Detalle(long id, CancellationToken ct) =>
        Exito(await operations.DetalleAsync(id, ct));

    [HttpPost]
    [Authorize(Policy = Permissions.CrearPedidos)]
    public async Task<IActionResult> Crear([FromBody] CrearPedidoRequest request,
        CancellationToken ct)
    {
        var result = await operations.CrearAsync(request, UsuarioId, ct);
        return CreatedAtAction(nameof(Obtener), new { id = result.PedidoId },
            new ApiResponse<CrearPedidoResultadoDto>
            { Success = true, Message = result.Mensaje, Data = result });
    }

    [HttpPost("{id:long}/autorizar")]
    [Authorize(Policy = Permissions.AutorizarPedidos)]
    public async Task<IActionResult> Autorizar(long id,
        [FromBody] AutorizarPedidoRequest request, CancellationToken ct)
    {
        var result = await operations.AutorizarAsync(id, request, UsuarioId, ct);
        return Exito(result, result.Mensaje);
    }

    [HttpPost("{id:long}/devolver-correccion")]
    [Authorize(Policy = Permissions.DevolverPedidos)]
    public async Task<IActionResult> Devolver(long id,
        [FromBody] ComentarioRequest request, CancellationToken ct)
    {
        var result = await operations.DevolverAsync(id, request.Comentario, UsuarioId, ct);
        return Exito(result, result.Mensaje);
    }

    [HttpPost("{id:long}/solicitar-cancelacion")]
    [Authorize(Policy = Permissions.SolicitarCancelacion)]
    public async Task<IActionResult> SolicitarCancelacion(long id,
        [FromBody] ComentarioRequest request, CancellationToken ct)
    {
        var result = await operations.SolicitarCancelacionAsync(id, request.Comentario, UsuarioId, ct);
        return Exito(result, result.Mensaje);
    }

    [HttpPost("{id:long}/aprobar-cancelacion")]
    [Authorize(Policy = Permissions.AprobarCancelacion)]
    public async Task<IActionResult> AprobarCancelacion(long id,
        [FromBody] ComentarioRequest request, CancellationToken ct)
    {
        var result = await operations.AprobarCancelacionAsync(id, request.Comentario, UsuarioId, ct);
        return Exito(result, result.Mensaje);
    }
}
