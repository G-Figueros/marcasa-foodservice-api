using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarcasaFoodService.Api.Controllers;

[Route("api/usuarios")]
public sealed class UsuariosController : ApiControllerBase
{
    [HttpGet("me")]
    public IActionResult Me() => Exito(new
    {
        UsuarioId,
        NombreUsuario = User.Identity?.Name,
        RolId = User.FindFirst("role_id")?.Value,
        Permisos = User.FindAll("permission").Select(c => c.Value).ToArray()
    });
}
