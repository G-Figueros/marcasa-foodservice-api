using MarcasaFoodService.Api.Models.Roles;

namespace MarcasaFoodService.Api.Services.Roles;

public interface IRolesService
{
    Task<List<RolAdminDto>> ListarAsync(int actorId);
    Task<List<PermisoRolDto>> PermisosAsync(int actorId, int? rolId);
    Task<int> GuardarAsync(int actorId, int? rolId, GuardarRolDto request);
    Task EstadoAsync(int actorId, int rolId, bool activo);
}
