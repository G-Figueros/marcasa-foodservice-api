using System.ComponentModel.DataAnnotations;

namespace MarcasaFoodService.Api.Models.Roles;

public sealed class RolAdminDto
{
    public int RolId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public int UsuariosActivos { get; set; }
    public int PermisosHabilitados { get; set; }
}

public sealed class PermisoRolDto
{
    public int PermisoId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Habilitado { get; set; }
}

public sealed class GuardarRolDto
{
    [Required, StringLength(50, MinimumLength = 2)] public string Nombre { get; set; } = string.Empty;
    [StringLength(200)] public string? Descripcion { get; set; }
    [Required] public List<int> PermisoIds { get; set; } = [];
}

public sealed class EstadoRolDto
{
    public bool Activo { get; set; }
}
