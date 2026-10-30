using System.ComponentModel.DataAnnotations;

namespace MarcasaFoodService.Api.Models.Auth;

public sealed class CambiarPasswordRequestDto
{
    [Required] public string PasswordActual { get; set; } = string.Empty;
    [Required] public string PasswordNuevo { get; set; } = string.Empty;
    [Required] public string Confirmacion { get; set; } = string.Empty;
}
