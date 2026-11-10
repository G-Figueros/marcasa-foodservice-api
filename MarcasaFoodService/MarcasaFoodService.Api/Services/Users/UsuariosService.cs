using MarcasaFoodService.Api.Models.Users;
using MarcasaFoodService.Api.Repositories.Users;
using MarcasaFoodService.Api.Services.Auth;

namespace MarcasaFoodService.Api.Services.Users;

public sealed class UsuariosService(IUsuariosRepository repository) : IUsuariosService
{
    public Task<CuentaDto?> CuentaAsync(int usuarioId) => repository.CuentaAsync(usuarioId);
    public Task<CuentaDto?> ActualizarCuentaAsync(int usuarioId, ActualizarCuentaDto data) =>
        repository.ActualizarCuentaAsync(usuarioId, data);
    public const string PasswordTemporal = "Temporal2026*";
    public Task<List<UsuarioAdminDto>> ListarAsync(int actorId) => repository.ListarAsync(actorId);
    public Task<List<RolUsuarioDto>> RolesAsync(int actorId) => repository.RolesAsync(actorId);
    public async Task<int> CrearAsync(int actorId, GuardarUsuarioDto data)
    {
        var (hash, salt) = PasswordHasher.Hash(PasswordTemporal);
        return await repository.CrearAsync(actorId, data, hash, salt);
    }
    public Task EditarAsync(int actorId, int usuarioId, GuardarUsuarioDto data) =>
        repository.EditarAsync(actorId, usuarioId, data);
    public Task EstadoAsync(int actorId, int usuarioId, bool activo) =>
        repository.EstadoAsync(actorId, usuarioId, activo);
    public async Task ReiniciarPasswordAsync(int actorId, int usuarioId)
    {
        var (hash, salt) = PasswordHasher.Hash(PasswordTemporal);
        await repository.ReiniciarPasswordAsync(actorId, usuarioId, hash, salt);
    }
}
