using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarcasaFoodService.Api.Services.Operations;
using Microsoft.AspNetCore.Authorization;
using MarcasaFoodService.Api.Models.Operations;

namespace MarcasaFoodService.Api.Controllers;

[Route("api/inventario")]
public sealed class InventarioController(IOperationsService operations) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = Permissions.VerInventario)]
    public async Task<IActionResult> Listar([FromQuery] string? textoBusqueda = null,
        [FromQuery] bool soloActivos = true, CancellationToken ct = default) =>
        Exito(await operations.InventarioAsync(textoBusqueda, soloActivos, ct));

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Gerente,Supervisora", Policy = Permissions.AgregarStock)]
    public async Task<IActionResult> Ajustar(int id, [FromBody] AjustarInventarioRequest request, CancellationToken ct)
    {
        await operations.AjustarInventarioAsync(id, request, UsuarioId, ct);
        return Exito(new { InventarioId = id }, "Inventario actualizado.");
    }
}
