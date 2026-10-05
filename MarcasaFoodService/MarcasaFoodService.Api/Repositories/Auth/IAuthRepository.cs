using MarcasaFoodService.Api.Models.Auth;

namespace MarcasaFoodService.Api.Repositories.Auth
{
    public interface IAuthRepository
    {
        Task<UsuarioLoginDbDto?> ObtenerUsuarioLoginAsync(string nombreUsuario);

        Task<List<PermisoUsuarioDto>> ObtenerPermisosUsuarioAsync(int usuarioId);

        Task ActualizarUltimoAccesoAsync(int usuarioId);
    }
}