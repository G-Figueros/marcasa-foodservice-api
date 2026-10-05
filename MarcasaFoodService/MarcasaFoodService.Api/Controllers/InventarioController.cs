using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarcasaFoodService.Api.Services.Operations;
using Microsoft.AspNetCore.Authorization;

namespace MarcasaFoodService.Api.Controllers;

[Route("api/inventario")]
public sealed class InventarioController(IOperationsService operations) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = Permissions.VerInventario)]
    public async Task<IActionResult> Listar([FromQuery] string? textoBusqueda = null,
        [FromQuery] bool soloActivos = true, CancellationToken ct = default) =>
        Exito(await operations.InventarioAsync(textoBusqueda, soloActivos, ct));
}
