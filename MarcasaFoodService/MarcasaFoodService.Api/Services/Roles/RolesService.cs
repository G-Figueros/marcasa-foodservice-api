using System.ComponentModel.DataAnnotations;
using MarcasaFoodService.Api.Models.Roles;
using MarcasaFoodService.Api.Repositories.Roles;

namespace MarcasaFoodService.Api.Services.Roles;

public sealed class RolesService(IRolesRepository repository) : IRolesService
{
    public Task<List<RolAdminDto>> ListarAsync(int actorId) => repository.ListarAsync(actorId);
    public Task<List<PermisoRolDto>> PermisosAsync(int actorId, int? rolId) => repository.PermisosAsync(actorId, rolId);
    public Task<int> GuardarAsync(int actorId, int? rolId, GuardarRolDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre) || request.Nombre.Trim().Length < 2 ||
            request.PermisoIds.Any(id => id <= 0) || request.PermisoIds.Count > 200)
            throw new ValidationException("Revisá el nombre y los permisos del rol.");
        return repository.GuardarAsync(actorId, rolId, request);
    }
    public Task EstadoAsync(int actorId, int rolId, bool activo) => repository.EstadoAsync(actorId, rolId, activo);
}
