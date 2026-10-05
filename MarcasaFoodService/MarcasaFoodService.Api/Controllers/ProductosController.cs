using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarcasaFoodService.Api.Services.Operations;
using Microsoft.AspNetCore.Authorization;

namespace MarcasaFoodService.Api.Controllers;

[Route("api/productos")]
public sealed class ProductosController(IOperationsService operations) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = Permissions.VerProductos)]
    public async Task<IActionResult> Listar([FromQuery] string? textoBusqueda = null,
        [FromQuery] bool soloActivos = true, CancellationToken ct = default) =>
        Exito(await operations.ProductosAsync(textoBusqueda, soloActivos, ct));
}
