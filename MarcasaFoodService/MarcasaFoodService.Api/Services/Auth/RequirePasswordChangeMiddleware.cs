using MarcasaFoodService.Api.Models.Common;

namespace MarcasaFoodService.Api.Services.Auth;

// La sesión temporal solo puede llamar al endpoint que cambia su contraseña.
public sealed class RequirePasswordChangeMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true &&
            context.User.HasClaim("password_change_required", "true") &&
            !context.Request.Path.Equals("/api/Auth/change-password", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new ApiResponse<object>
            {
                Success = false,
                Message = "Debés cambiar tu contraseña antes de continuar."
            });
            return;
        }
        await next(context);
    }
}
