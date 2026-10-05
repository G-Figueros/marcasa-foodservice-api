using Microsoft.Data.SqlClient;

namespace MarcasaFoodService.Api.Data
{
    public interface ISqlConnectionFactory
    {
        SqlConnection CreateConnection();
    }

    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection CreateConnection()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontró la cadena de conexión DefaultConnection.");

            return new SqlConnection(connectionString);
        }
    }
}