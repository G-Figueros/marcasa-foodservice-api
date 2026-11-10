using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarcasaFoodService.Api.Services.Operations;
using Microsoft.AspNetCore.Authorization;
using MarcasaFoodService.Api.Models.Operations;

namespace MarcasaFoodService.Api.Controllers;

[Route("api/productos")]
public sealed class ProductosController(IOperationsService operations) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = Permissions.VerProductos)]
    public async Task<IActionResult> Listar([FromQuery] string? textoBusqueda = null,
        [FromQuery] bool soloActivos = true, CancellationToken ct = default) =>
        Exito(await operations.ProductosAsync(textoBusqueda, soloActivos, ct));

    [HttpPost]
    [Authorize(Roles = "Gerente,Supervisora", Policy = Permissions.EditarProductos)]
    public async Task<IActionResult> Crear([FromBody] GuardarProductoRequest request, CancellationToken ct) =>
        Exito(new { ProductoId = await operations.GuardarProductoAsync(null, request, UsuarioId, ct) }, "Producto creado con inventario en cero.");

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Gerente,Supervisora", Policy = Permissions.EditarProductos)]
    public async Task<IActionResult> Editar(int id, [FromBody] GuardarProductoRequest request, CancellationToken ct) =>
        Exito(new { ProductoId = await operations.GuardarProductoAsync(id, request, UsuarioId, ct) }, "Producto actualizado.");

    [HttpPatch("{id:int}/estado")]
    [Authorize(Roles = "Gerente,Supervisora", Policy = Permissions.EditarProductos)]
    public async Task<IActionResult> Estado(int id, [FromBody] EstadoProductoRequest request, CancellationToken ct)
    {
        await operations.CambiarEstadoProductoAsync(id, request.Activo, UsuarioId, ct);
        return Exito(new { ProductoId = id, request.Activo }, request.Activo ? "Producto activado." : "Producto fuera de venta.");
    }
}
