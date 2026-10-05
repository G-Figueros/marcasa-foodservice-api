namespace MarcasaFoodService.Api.Models.Auth
{
    public class UsuarioLoginDbDto
    {
        public int UsuarioId { get; set; }

        public int RolId { get; set; }

        public string NombreRol { get; set; } = string.Empty;

        public string NombreUsuario { get; set; } = string.Empty;

        public string Nombres { get; set; } = string.Empty;

        public string? Apellidos { get; set; }

        public string? Correo { get; set; }

        public string? Telefono { get; set; }

        public string PasswordHash { get; set; } = string.Empty;

        public string PasswordSalt { get; set; } = string.Empty;

        public bool DebeCambiarPassword { get; set; }

        public bool Activo { get; set; }

        public DateTime? UltimoAcceso { get; set; }
    }
}