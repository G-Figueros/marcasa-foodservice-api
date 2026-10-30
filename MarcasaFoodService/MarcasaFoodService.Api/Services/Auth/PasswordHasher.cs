using System.Security.Cryptography;
using System.Text;

namespace MarcasaFoodService.Api.Services.Auth;

// Mantiene compatibilidad con el hash inicial de SQL y usa PBKDF2 para claves nuevas.
public static class PasswordHasher
{
    private const int Iterations = 210_000;

    public static (string Hash, string Salt) Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(32);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
        return ($"PBKDF2${Iterations}${Convert.ToBase64String(hash)}", Convert.ToBase64String(salt));
    }

    public static bool Verify(string password, string salt, string storedHash)
    {
        try
        {
            byte[] actual;
            byte[] expected;
            if (storedHash.StartsWith("PBKDF2$", StringComparison.Ordinal))
            {
                var parts = storedHash.Split('$');
                if (parts.Length != 3 || !int.TryParse(parts[1], out var iterations) || iterations < 100_000)
                    return false;
                expected = Convert.FromBase64String(parts[2]);
                actual = Rfc2898DeriveBytes.Pbkdf2(password, Convert.FromBase64String(salt),
                    iterations, HashAlgorithmName.SHA256, expected.Length);
            }
            else
            {
                expected = Convert.FromHexString(storedHash);
                actual = SHA256.HashData(Encoding.Unicode.GetBytes(password + salt));
            }
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException) { return false; }
    }
}
