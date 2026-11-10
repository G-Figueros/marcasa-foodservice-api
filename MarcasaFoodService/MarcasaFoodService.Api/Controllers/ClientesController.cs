using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarcasaFoodService.Api.Services.Operations;
using Microsoft.AspNetCore.Authorization;
using MarcasaFoodService.Api.Models.Operations;

namespace MarcasaFoodService.Api.Controllers;

[Route("api/clientes")]
public sealed class ClientesController(IOperationsService operations) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = Permissions.VerClientes)]
    public async Task<IActionResult> Listar([FromQuery] string? textoBusqueda = null,
        [FromQuery] bool soloActivos = true, CancellationToken ct = default) =>
        Exito(await operations.ClientesAsync(textoBusqueda, soloActivos, ct));

    [HttpPost]
    [Authorize(Roles = "Gerente,Supervisora", Policy = Permissions.CrearClientes)]
    public async Task<IActionResult> Crear([FromBody] GuardarClienteRequest request, CancellationToken ct) =>
        Exito(new { ClienteId = await operations.GuardarClienteAsync(null, request, UsuarioId, ct) }, "Cliente creado.");

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Gerente,Supervisora", Policy = Permissions.EditarClientes)]
    public async Task<IActionResult> Editar(int id, [FromBody] GuardarClienteRequest request, CancellationToken ct) =>
        Exito(new { ClienteId = await operations.GuardarClienteAsync(id, request, UsuarioId, ct) }, "Cliente actualizado.");

    [HttpPatch("{id:int}/estado")]
    [Authorize(Roles = "Gerente,Supervisora", Policy = Permissions.EditarClientes)]
    public async Task<IActionResult> Estado(int id, [FromBody] EstadoClienteRequest request, CancellationToken ct)
    {
        await operations.CambiarEstadoClienteAsync(id, request.Activo, UsuarioId, ct);
        return Exito(new { ClienteId = id, request.Activo }, request.Activo ? "Cliente activado." : "Cliente desactivado.");
    }
}
