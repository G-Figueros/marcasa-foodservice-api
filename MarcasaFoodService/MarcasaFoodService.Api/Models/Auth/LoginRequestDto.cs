using System.ComponentModel.DataAnnotations;

namespace MarcasaFoodService.Api.Models.Auth
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "El usuario debe tener entre 3 y 60 caracteres.")]
        public string Usuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }
}