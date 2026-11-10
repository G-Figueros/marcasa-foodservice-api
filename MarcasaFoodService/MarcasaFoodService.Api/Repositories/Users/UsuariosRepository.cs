using System.Data;
using Dapper;
using MarcasaFoodService.Api.Data;
using MarcasaFoodService.Api.Models.Users;

namespace MarcasaFoodService.Api.Repositories.Users;

// Toda escritura pasa por los procedimientos de la base y recibe el actor del JWT.
public sealed class UsuariosRepository(ISqlConnectionFactory factory) : IUsuariosRepository
{
    public async Task<CuentaDto?> CuentaAsync(int usuarioId)
    {
        await using var db = factory.CreateConnection();
        return await db.QuerySingleOrDefaultAsync<CuentaDto>("dbo.usp_ObtenerCuenta",
            new { UsuarioId = usuarioId }, commandType: CommandType.StoredProcedure);
    }

    public async Task<CuentaDto?> ActualizarCuentaAsync(int usuarioId, ActualizarCuentaDto data)
    {
        await using var db = factory.CreateConnection();
        return await db.QuerySingleOrDefaultAsync<CuentaDto>("dbo.usp_ActualizarCuenta",
            new { UsuarioId = usuarioId, Nombres = data.Nombres.Trim(),
                Apellidos = string.IsNullOrWhiteSpace(data.Apellidos) ? null : data.Apellidos.Trim(),
                Correo = string.IsNullOrWhiteSpace(data.Correo) ? null : data.Correo.Trim(),
                Telefono = string.IsNullOrWhiteSpace(data.Telefono) ? null : data.Telefono.Trim() },
            commandType: CommandType.StoredProcedure);
    }
    public async Task<List<UsuarioAdminDto>> ListarAsync(int actorId)
    {
        await using var db = factory.CreateConnection();
        return (await db.QueryAsync<UsuarioAdminDto>("dbo.usp_ListarUsuariosAdmin",
            new { ActorId = actorId }, commandType: CommandType.StoredProcedure)).ToList();
    }

    public async Task<List<RolUsuarioDto>> RolesAsync(int actorId)
    {
        await using var db = factory.CreateConnection();
        return (await db.QueryAsync<RolUsuarioDto>("dbo.usp_ListarRolesUsuarios",
            new { ActorId = actorId }, commandType: CommandType.StoredProcedure)).ToList();
    }

    public async Task<int> CrearAsync(int actorId, GuardarUsuarioDto data, string hash, string salt)
    {
        await using var db = factory.CreateConnection();
        return await db.QuerySingleAsync<int>("dbo.usp_CrearUsuarioAdmin", Params(actorId, data, hash, salt),
            commandType: CommandType.StoredProcedure);
    }

    public async Task EditarAsync(int actorId, int usuarioId, GuardarUsuarioDto data)
    {
        await using var db = factory.CreateConnection();
        await db.ExecuteAsync("dbo.usp_EditarUsuarioAdmin", Params(actorId, data, usuarioId: usuarioId),
            commandType: CommandType.StoredProcedure);
    }

    public async Task EstadoAsync(int actorId, int usuarioId, bool activo)
    {
        await using var db = factory.CreateConnection();
        await db.ExecuteAsync("dbo.usp_CambiarEstadoUsuarioAdmin",
            new { ActorId = actorId, UsuarioId = usuarioId, Activo = activo }, commandType: CommandType.StoredProcedure);
    }

    public async Task ReiniciarPasswordAsync(int actorId, int usuarioId, string hash, string salt)
    {
        await using var db = factory.CreateConnection();
        await db.ExecuteAsync("dbo.usp_ReiniciarPasswordUsuarioAdmin",
            new { ActorId = actorId, UsuarioId = usuarioId, PasswordHash = hash, PasswordSalt = salt },
            commandType: CommandType.StoredProcedure);
    }

    private static DynamicParameters Params(int actorId, GuardarUsuarioDto data, string? hash = null,
        string? salt = null, int? usuarioId = null)
    {
        var p = new DynamicParameters();
        p.Add("ActorId", actorId);
        if (usuarioId.HasValue) p.Add("UsuarioId", usuarioId.Value);
        p.Add("NombreUsuario", data.NombreUsuario.Trim());
        p.Add("Nombres", data.Nombres.Trim());
        p.Add("Apellidos", string.IsNullOrWhiteSpace(data.Apellidos) ? null : data.Apellidos.Trim());
        p.Add("Correo", string.IsNullOrWhiteSpace(data.Correo) ? null : data.Correo.Trim());
        p.Add("Telefono", string.IsNullOrWhiteSpace(data.Telefono) ? null : data.Telefono.Trim());
        p.Add("RolId", data.RolId);
        if (hash is not null) p.Add("PasswordHash", hash);
        if (salt is not null) p.Add("PasswordSalt", salt);
        return p;
    }
}
