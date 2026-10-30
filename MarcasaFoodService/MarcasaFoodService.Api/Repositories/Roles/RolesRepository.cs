using System.Data;
using Dapper;
using MarcasaFoodService.Api.Data;
using MarcasaFoodService.Api.Models.Roles;

namespace MarcasaFoodService.Api.Repositories.Roles;

// El TVP reemplaza la asignación completa de permisos dentro de una transacción SQL.
public sealed class RolesRepository(ISqlConnectionFactory factory) : IRolesRepository
{
    public async Task<List<RolAdminDto>> ListarAsync(int actorId)
    {
        await using var db = factory.CreateConnection();
        return (await db.QueryAsync<RolAdminDto>("dbo.usp_ListarRolesAdmin",
            new { ActorId = actorId }, commandType: CommandType.StoredProcedure)).ToList();
    }

    public async Task<List<PermisoRolDto>> PermisosAsync(int actorId, int? rolId)
    {
        await using var db = factory.CreateConnection();
        return (await db.QueryAsync<PermisoRolDto>("dbo.usp_ListarPermisosRolAdmin",
            new { ActorId = actorId, RolId = rolId }, commandType: CommandType.StoredProcedure)).ToList();
    }

    public async Task<int> GuardarAsync(int actorId, int? rolId, GuardarRolDto request)
    {
        var ids = new DataTable();
        ids.Columns.Add("PermisoId", typeof(int));
        foreach (var id in request.PermisoIds.Distinct()) ids.Rows.Add(id);
        var parameters = new DynamicParameters();
        parameters.Add("ActorId", actorId);
        parameters.Add("RolId", rolId);
        parameters.Add("Nombre", request.Nombre.Trim());
        parameters.Add("Descripcion", string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion.Trim());
        parameters.Add("Permisos", ids.AsTableValuedParameter("dbo.TVP_PermisoIdAdmin"));
        await using var db = factory.CreateConnection();
        return await db.QuerySingleAsync<int>("dbo.usp_GuardarRolAdmin", parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task EstadoAsync(int actorId, int rolId, bool activo)
    {
        await using var db = factory.CreateConnection();
        await db.ExecuteAsync("dbo.usp_CambiarEstadoRolAdmin",
            new { ActorId = actorId, RolId = rolId, Activo = activo }, commandType: CommandType.StoredProcedure);
    }
}
