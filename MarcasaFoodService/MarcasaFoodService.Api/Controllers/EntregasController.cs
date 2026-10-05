using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MarcasaFoodService.Api.Models.Common;
using MarcasaFoodService.Api.Models.Operations;
using MarcasaFoodService.Api.Services.Operations;
using Microsoft.AspNetCore.Authorization;

namespace MarcasaFoodService.Api.Controllers;

[Route("api/entregas")]
public sealed class EntregasController(IOperationsService operations) : ApiControllerBase
{
    [HttpGet("pendientes")]
    [Authorize(Policy = Permissions.VerPendientesEntrega)]
    public async Task<IActionResult> Pendientes([FromQuery] PedidoFiltro filtro,
        CancellationToken ct) => Exito(await operations.PendientesEntregaAsync(filtro, ct));

    // Datos estructurados para que el frontend genere la hoja de ruta o el PDF.
    [HttpGet("datos-ruta")]
    [Authorize(Policy = Permissions.ExportarPdfRuta)]
    public async Task<IActionResult> DatosRuta([FromQuery] PedidoFiltro filtro,
        CancellationToken ct) => Exito(await operations.DatosRutaAsync(filtro, ct));

    [HttpPost("{pedidoId:long}/confirmar")]
    [Authorize(Policy = Permissions.ConfirmarEntrega)]
    public async Task<IActionResult> Confirmar(long pedidoId,
        [FromBody] ConfirmarEntregaRequest request, CancellationToken ct)
    {
        var result = await operations.ConfirmarEntregaAsync(pedidoId, request, UsuarioId, ct);
        return Exito(result, result.Mensaje);
    }
}
