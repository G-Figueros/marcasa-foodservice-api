using System.Data;
using Dapper;
using MarcasaFoodService.Api.Data;
using MarcasaFoodService.Api.Models.Auth;

namespace MarcasaFoodService.Api.Repositories.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public AuthRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<UsuarioLoginDbDto?> ObtenerUsuarioLoginAsync(string nombreUsuario)
        {
            await using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@NombreUsuario", nombreUsuario, DbType.String, size: 60);

            return await connection.QueryFirstOrDefaultAsync<UsuarioLoginDbDto>(
                "dbo.usp_LoginUsuario",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<List<PermisoUsuarioDto>> ObtenerPermisosUsuarioAsync(int usuarioId)
        {
            await using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UsuarioId", usuarioId, DbType.Int32);

            var permisos = await connection.QueryAsync<PermisoUsuarioDto>(
                "dbo.usp_ObtenerPermisosUsuario",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return permisos.ToList();
        }

        public async Task ActualizarUltimoAccesoAsync(int usuarioId)
        {
            await using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UsuarioId", usuarioId, DbType.Int32);

            await connection.ExecuteAsync(
                "dbo.usp_ActualizarUltimoAccesoUsuario",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<EstadoSesionDbDto?> ObtenerEstadoSesionAsync(int usuarioId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<EstadoSesionDbDto>(
                "dbo.usp_ObtenerEstadoSesion", new { UsuarioId = usuarioId }, commandType: CommandType.StoredProcedure);
        }

        public async Task CambiarPasswordPropiaAsync(int usuarioId, int version, string hash, string salt)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("dbo.usp_CambiarPasswordPropia",
                new { UsuarioId = usuarioId, VersionCredenciales = version, PasswordHash = hash, PasswordSalt = salt },
                commandType: CommandType.StoredProcedure);
        }
    }
}
