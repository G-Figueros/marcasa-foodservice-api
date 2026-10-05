namespace MarcasaFoodService.Api.Models.Auth
{
    public class LoginResponseDto
    {
        public UsuarioSesionDto Usuario { get; set; } = new();

        public List<PermisoUsuarioDto> Permisos { get; set; } = new();

        public string Token { get; set; } = string.Empty;
    }

    public class UsuarioSesionDto
    {
        public int UsuarioId { get; set; }

        public int RolId { get; set; }

        public string NombreRol { get; set; } = string.Empty;

        public string NombreUsuario { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;

        public string? Correo { get; set; }

        public bool DebeCambiarPassword { get; set; }
    }

    public class PermisoUsuarioDto
    {
        public int PermisoId { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public bool Habilitado { get; set; }
    }
}