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
    }
}