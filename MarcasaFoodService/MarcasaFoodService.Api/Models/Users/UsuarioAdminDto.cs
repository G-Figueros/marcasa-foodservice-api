using System.ComponentModel.DataAnnotations;

namespace MarcasaFoodService.Api.Models.Users;

public sealed class UsuarioAdminDto
{
    public int UsuarioId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string? Apellidos { get; set; }
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
    public int RolId { get; set; }
    public string NombreRol { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public bool DebeCambiarPassword { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public DateTime? UltimoAcceso { get; set; }
}

public sealed class RolUsuarioDto
{
    public int RolId { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public sealed class GuardarUsuarioDto
{
    [Required, RegularExpression(@"^[a-zA-Z0-9._-]{3,60}$")]
    public string NombreUsuario { get; set; } = string.Empty;
    [Required, StringLength(120, MinimumLength = 2)]
    public string Nombres { get; set; } = string.Empty;
    [StringLength(120)] public string? Apellidos { get; set; }
    [EmailAddress, StringLength(150)] public string? Correo { get; set; }
    [StringLength(25)] public string? Telefono { get; set; }
    [Range(1, int.MaxValue)] public int RolId { get; set; }
}

public sealed class EstadoUsuarioDto
{
    public bool Activo { get; set; }
}
