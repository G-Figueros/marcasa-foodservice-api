using MarcasaFoodService.Api.Models.Users;

namespace MarcasaFoodService.Api.Repositories.Users;

public interface IUsuariosRepository
{
    Task<List<UsuarioAdminDto>> ListarAsync(int actorId);
    Task<List<RolUsuarioDto>> RolesAsync(int actorId);
    Task<int> CrearAsync(int actorId, GuardarUsuarioDto data, string hash, string salt);
    Task EditarAsync(int actorId, int usuarioId, GuardarUsuarioDto data);
    Task EstadoAsync(int actorId, int usuarioId, bool activo);
    Task ReiniciarPasswordAsync(int actorId, int usuarioId, string hash, string salt);
}
