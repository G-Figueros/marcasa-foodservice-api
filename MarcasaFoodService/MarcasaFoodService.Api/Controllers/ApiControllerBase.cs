using System.Security.Claims;
using MarcasaFoodService.Api.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarcasaFoodService.Api.Controllers;

[ApiController]
[Authorize]
public abstract class ApiControllerBase : ControllerBase
{
    protected int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("El token no incluye usuario."));

    protected OkObjectResult Exito<T>(T data, string message = "Consulta realizada correctamente.") =>
        Ok(new ApiResponse<T> { Success = true, Message = message, Data = data });
}
