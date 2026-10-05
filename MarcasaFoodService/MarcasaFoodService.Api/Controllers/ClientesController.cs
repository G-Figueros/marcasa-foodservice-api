using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarcasaFoodService.Api.Services.Operations;
using Microsoft.AspNetCore.Authorization;

namespace MarcasaFoodService.Api.Controllers;

[Route("api/clientes")]
public sealed class ClientesController(IOperationsService operations) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = Permissions.VerClientes)]
    public async Task<IActionResult> Listar([FromQuery] string? textoBusqueda = null,
        [FromQuery] bool soloActivos = true, CancellationToken ct = default) =>
        Exito(await operations.ClientesAsync(textoBusqueda, soloActivos, ct));
}
