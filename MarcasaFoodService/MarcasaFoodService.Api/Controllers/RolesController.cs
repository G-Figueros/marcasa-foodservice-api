using MarcasaFoodService.Api.Models.Roles;
using MarcasaFoodService.Api.Services.Operations;
using MarcasaFoodService.Api.Services.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarcasaFoodService.Api.Controllers;

[Route("api/roles")]
public sealed class RolesController(IRolesService service) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Gerente", Policy = Permissions.VerRoles)]
    public async Task<IActionResult> Listar() => Exito(await service.ListarAsync(UsuarioId));

    [HttpGet("permisos")]
    [Authorize(Roles = "Gerente", Policy = Permissions.VerRoles)]
    public async Task<IActionResult> Permisos([FromQuery] int? rolId) => Exito(await service.PermisosAsync(UsuarioId, rolId));

    [HttpPost]
    [Authorize(Roles = "Gerente", Policy = Permissions.CrearRoles)]
    public async Task<IActionResult> Crear([FromBody] GuardarRolDto request)
    {
        var id = await service.GuardarAsync(UsuarioId, null, request);
        return Exito(new { RolId = id }, "Rol creado.");
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Gerente", Policy = Permissions.EditarRoles)]
    public async Task<IActionResult> Editar(int id, [FromBody] GuardarRolDto request)
    {
        await service.GuardarAsync(UsuarioId, id, request);
        return Exito(new { RolId = id }, "Rol actualizado.");
    }

    [HttpPatch("{id:int}/estado")]
    [Authorize(Roles = "Gerente", Policy = Permissions.EditarRoles)]
    public async Task<IActionResult> Estado(int id, [FromBody] EstadoRolDto request)
    {
        await service.EstadoAsync(UsuarioId, id, request.Activo);
        return Exito(new { RolId = id, request.Activo }, request.Activo ? "Rol activado." : "Rol desactivado.");
    }
}
