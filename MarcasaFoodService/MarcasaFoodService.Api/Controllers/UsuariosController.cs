using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MarcasaFoodService.Api.Models.Users;
using MarcasaFoodService.Api.Services.Users;
using MarcasaFoodService.Api.Services.Operations;

namespace MarcasaFoodService.Api.Controllers;

[Route("api/usuarios")]
public sealed class UsuariosController(IUsuariosService service) : ApiControllerBase
{
    [HttpGet("me")]
    public IActionResult Me() => Exito(new
    {
        UsuarioId,
        NombreUsuario = User.Identity?.Name,
        RolId = User.FindFirst("role_id")?.Value,
        Permisos = User.FindAll("permission").Select(c => c.Value).ToArray()
    });

    [HttpGet]
    [Authorize(Roles = "Gerente", Policy = Permissions.VerUsuarios)]
    public async Task<IActionResult> Listar() => Exito(await service.ListarAsync(UsuarioId));

    [HttpGet("roles")]
    [Authorize(Roles = "Gerente", Policy = Permissions.VerUsuarios)]
    public async Task<IActionResult> Roles() => Exito(await service.RolesAsync(UsuarioId));

    [HttpPost]
    [Authorize(Roles = "Gerente", Policy = Permissions.CrearUsuarios)]
    public async Task<IActionResult> Crear([FromBody] GuardarUsuarioDto request)
    {
        var id = await service.CrearAsync(UsuarioId, request);
        return Exito(new { UsuarioId = id, PasswordTemporal = UsuariosService.PasswordTemporal },
            "Usuario creado. Deberá cambiar la contraseña en su primer ingreso.");
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Gerente", Policy = Permissions.EditarUsuarios)]
    public async Task<IActionResult> Editar(int id, [FromBody] GuardarUsuarioDto request)
    {
        await service.EditarAsync(UsuarioId, id, request);
        return Exito(new { UsuarioId = id }, "Usuario actualizado.");
    }

    [HttpPatch("{id:int}/estado")]
    [Authorize(Roles = "Gerente", Policy = Permissions.EditarUsuarios)]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] EstadoUsuarioDto request)
    {
        await service.EstadoAsync(UsuarioId, id, request.Activo);
        return Exito(new { UsuarioId = id, request.Activo }, request.Activo ? "Usuario activado." : "Usuario desactivado.");
    }

    [HttpPost("{id:int}/reiniciar-password")]
    [Authorize(Roles = "Gerente", Policy = Permissions.EditarUsuarios)]
    public async Task<IActionResult> ReiniciarPassword(int id)
    {
        await service.ReiniciarPasswordAsync(UsuarioId, id);
        return Exito(new { UsuarioId = id, PasswordTemporal = UsuariosService.PasswordTemporal },
            "Contraseña reiniciada. El usuario deberá cambiarla al ingresar.");
    }
}
