using MarcasaFoodService.Api.Models.Users;
using MarcasaFoodService.Api.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace MarcasaFoodService.Api.Controllers;

[Route("api/cuenta")]
public sealed class CuentaController(IUsuariosService service) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Obtener() =>
        await service.CuentaAsync(UsuarioId) is { } cuenta ? Exito(cuenta) : NotFound();

    [HttpPut]
    public async Task<IActionResult> Actualizar([FromBody] ActualizarCuentaDto request) =>
        await service.ActualizarCuentaAsync(UsuarioId, request) is { } cuenta
            ? Exito(cuenta, "Información personal actualizada.") : NotFound();
}
