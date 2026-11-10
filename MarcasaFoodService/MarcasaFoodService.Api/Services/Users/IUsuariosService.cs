using MarcasaFoodService.Api.Models.Users;

namespace MarcasaFoodService.Api.Services.Users;

public interface IUsuariosService
{
    Task<CuentaDto?> CuentaAsync(int usuarioId);
    Task<CuentaDto?> ActualizarCuentaAsync(int usuarioId, ActualizarCuentaDto data);
    Task<List<UsuarioAdminDto>> ListarAsync(int actorId);
    Task<List<RolUsuarioDto>> RolesAsync(int actorId);
    Task<int> CrearAsync(int actorId, GuardarUsuarioDto data);
    Task EditarAsync(int actorId, int usuarioId, GuardarUsuarioDto data);
    Task EstadoAsync(int actorId, int usuarioId, bool activo);
    Task ReiniciarPasswordAsync(int actorId, int usuarioId);
}
