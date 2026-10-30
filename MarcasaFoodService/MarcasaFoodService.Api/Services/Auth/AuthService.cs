using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using MarcasaFoodService.Api.Models.Auth;
using MarcasaFoodService.Api.Models.Common;
using MarcasaFoodService.Api.Repositories.Auth;
using Microsoft.IdentityModel.Tokens;

namespace MarcasaFoodService.Api.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtSigningKey _signingKey;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository authRepository, JwtSigningKey signingKey, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _signingKey = signingKey;
            _configuration = configuration;
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            string usuario = LimpiarUsuario(request.Usuario);
            string password = request.Password;

            if (!EsUsuarioSeguro(usuario))
            {
                return new ApiResponse<LoginResponseDto>
                {
                    Success = false,
                    Message = "El usuario contiene caracteres no permitidos."
                };
            }

            UsuarioLoginDbDto? usuarioDb = await _authRepository.ObtenerUsuarioLoginAsync(usuario);

            if (usuarioDb is null)
            {
                return CredencialesInvalidas();
            }

            bool passwordValido = PasswordHasher.Verify(
                password,
                usuarioDb.PasswordSalt,
                usuarioDb.PasswordHash
            );

            if (!passwordValido)
            {
                return CredencialesInvalidas();
            }

            List<PermisoUsuarioDto> permisos = await _authRepository.ObtenerPermisosUsuarioAsync(usuarioDb.UsuarioId);

            await _authRepository.ActualizarUltimoAccesoAsync(usuarioDb.UsuarioId);

            return new ApiResponse<LoginResponseDto>
            {
                Success = true,
                Message = "Login procesado correctamente.",
                Data = new LoginResponseDto
                {
                    Usuario = new UsuarioSesionDto
                    {
                        UsuarioId = usuarioDb.UsuarioId,
                        RolId = usuarioDb.RolId,
                        NombreRol = usuarioDb.NombreRol,
                        NombreUsuario = usuarioDb.NombreUsuario,
                        NombreCompleto = ConstruirNombreCompleto(usuarioDb.Nombres, usuarioDb.Apellidos),
                        Correo = usuarioDb.Correo,
                        DebeCambiarPassword = usuarioDb.DebeCambiarPassword
                    },
                    Permisos = permisos,
                    Token = CrearToken(usuarioDb, permisos)
                }
            };
        }

        private static ApiResponse<LoginResponseDto> CredencialesInvalidas()
        {
            return new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Message = "Usuario o contraseña incorrectos."
            };
        }

        private static string LimpiarUsuario(string usuario)
        {
            return usuario
                .Trim()
                .Replace(" ", string.Empty);
        }

        private static bool EsUsuarioSeguro(string usuario)
        {
            return Regex.IsMatch(usuario, @"^[a-zA-Z0-9._-]{3,60}$");
        }

        private static string ConstruirNombreCompleto(string nombres, string? apellidos)
        {
            return string.Join(
                " ",
                new[]
                {
                    nombres,
                    apellidos
                }.Where(valor => !string.IsNullOrWhiteSpace(valor))
            );
        }

        public async Task<ApiResponse<object>> CambiarPasswordAsync(int usuarioId, string nombreUsuario, CambiarPasswordRequestDto request)
        {
            ApiResponse<object> Error(string message) => new() { Success = false, Message = message };
            var user = await _authRepository.ObtenerUsuarioLoginAsync(nombreUsuario);
            if (user is null || user.UsuarioId != usuarioId || !PasswordHasher.Verify(request.PasswordActual, user.PasswordSalt, user.PasswordHash))
                return Error("La contraseña actual es incorrecta.");
            if (request.PasswordNuevo != request.Confirmacion)
                return Error("La confirmación no coincide.");
            if (request.PasswordNuevo.Length < 12 || request.PasswordNuevo.Length > 100 ||
                !request.PasswordNuevo.Any(char.IsLetter) || !request.PasswordNuevo.Any(char.IsDigit) ||
                !request.PasswordNuevo.Any(c => !char.IsLetterOrDigit(c)))
                return Error("La nueva contraseña debe tener entre 12 y 100 caracteres, letras, números y símbolos.");
            if (request.PasswordNuevo == "Temporal2026*" || PasswordHasher.Verify(request.PasswordNuevo, user.PasswordSalt, user.PasswordHash))
                return Error("Elegí una contraseña distinta de la actual y de la temporal.");
            var (hash, salt) = PasswordHasher.Hash(request.PasswordNuevo);
            await _authRepository.CambiarPasswordPropiaAsync(usuarioId, user.VersionCredenciales, hash, salt);
            return new ApiResponse<object> { Success = true, Message = "Contraseña actualizada. Iniciá sesión nuevamente." };
        }

        private string CrearToken(UsuarioLoginDbDto usuario, IEnumerable<PermisoUsuarioDto> permisos)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usuario.UsuarioId.ToString()),
                new(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new(ClaimTypes.Name, usuario.NombreUsuario),
                new(ClaimTypes.Role, usuario.NombreRol),
                new("role_id", usuario.RolId.ToString()),
                new("credential_version", usuario.VersionCredenciales.ToString()),
                new("password_change_required", usuario.DebeCambiarPassword ? "true" : "false")
            };
            claims.AddRange(permisos.Where(p => p.Habilitado)
                .Select(p => new Claim("permission", p.Codigo)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingKey.Value));
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "MarcasaFoodService",
                audience: _configuration["Jwt:Audience"] ?? "MarcasaFoodService.Web",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue("Jwt:ExpirationMinutes", 60)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
