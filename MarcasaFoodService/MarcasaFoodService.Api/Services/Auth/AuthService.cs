using System.Security.Cryptography;
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

            bool passwordValido = ValidarPasswordSqlServer(
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

        private static bool ValidarPasswordSqlServer(string passwordIngresado, string salt, string hashGuardado)
        {
            string hashCalculado = GenerarHashSha256CompatibleSqlServer(passwordIngresado, salt);

            try
            {
                return CryptographicOperations.FixedTimeEquals(
                    Convert.FromHexString(hashCalculado), Convert.FromHexString(hashGuardado));
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static string GenerarHashSha256CompatibleSqlServer(string password, string salt)
        {
            /*
             * SQL Server HASHBYTES sobre NVARCHAR trabaja con bytes Unicode.
             * Por eso usamos Encoding.Unicode para que el hash coincida
             * con el valor generado en el script SQL inicial.
             */
            byte[] bytes = Encoding.Unicode.GetBytes(password + salt);
            byte[] hash = SHA256.HashData(bytes);

            return Convert.ToHexString(hash);
        }

        private string CrearToken(UsuarioLoginDbDto usuario, IEnumerable<PermisoUsuarioDto> permisos)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usuario.UsuarioId.ToString()),
                new(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new(ClaimTypes.Name, usuario.NombreUsuario),
                new("role_id", usuario.RolId.ToString())
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
