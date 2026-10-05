using MarcasaFoodService.Api.Models.Common;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;

namespace MarcasaFoodService.Api.Data;

// Traduce errores de negocio lanzados por los procedimientos sin revelar detalles internos.
public sealed class SqlExceptionMiddleware(RequestDelegate next, ILogger<SqlExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (SqlException error) when (error.Number >= 50000 && error.Number < 56000)
        {
            context.Response.StatusCode = error.Number switch
            {
                50003 or 51001 or 52001 or 53001 or 54001 or 55001 => StatusCodes.Status403Forbidden,
                50004 or 51002 or 55004 => StatusCodes.Status404NotFound,
                50005 or 50006 or 50007 or 50008 or 51005 or 51006 or 51007 => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status409Conflict
            };
            await context.Response.WriteAsJsonAsync(new ApiResponse<object>
            {
                Success = false,
                Message = error.Message
            });
        }
        catch (ValidationException error)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ApiResponse<object>
            {
                Success = false,
                Message = error.Message
            });
        }
        catch (SqlException error)
        {
            logger.LogError(error, "Error de SQL Server al procesar {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsJsonAsync(new ApiResponse<object>
            {
                Success = false,
                Message = "No fue posible consultar la base de datos."
            });
        }
    }
}
