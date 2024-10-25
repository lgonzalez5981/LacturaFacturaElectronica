using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LacturaFacturaElectronica.DAO
{
    public class ConnectionDB
    {
        private readonly string _connectionString;

        public ConnectionDB()
        {
            // Configuración para leer el appsettings.json
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _connectionString = configurationBuilder.GetSection("ConnectionStrings:DB").Value;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
