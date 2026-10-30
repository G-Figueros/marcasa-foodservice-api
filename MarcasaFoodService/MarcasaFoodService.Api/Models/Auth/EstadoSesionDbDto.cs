namespace MarcasaFoodService.Api.Models.Auth;

public sealed class EstadoSesionDbDto
{
    public int UsuarioId { get; set; }
    public int RolId { get; set; }
    public string NombreRol { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public bool DebeCambiarPassword { get; set; }
    public int VersionCredenciales { get; set; }
}
