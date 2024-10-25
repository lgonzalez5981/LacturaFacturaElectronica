using LacturaFacturaElectronica.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LacturaFacturaElectronica.DAO
{
    public class ClienteDB
    {
        private readonly ConnectionDB _connectionDB;
        public ClienteDB() {
            _connectionDB = new ConnectionDB();
        }
        public long CrearCliente(ClienteModel model)
        {
            try
            {
                using (SqlConnection connection = _connectionDB.GetConnection())
                {
                    using (SqlCommand command = new SqlCommand("p_create_cliente_Empresa", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar los parámetros necesarios para el procedimiento almacenado
                        command.Parameters.AddWithValue("@Identificacion", model.Identificacion);
                        command.Parameters.AddWithValue("@NombreCliente", model.NombreCliente);
                        command.Parameters.AddWithValue("@IdEmpresa", model.IdEmpresa);

                        connection.Open();

                        // Ejecutar el comando y leer el resultado del SELECT
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Obtener el valor de IdEmpresa de la columna devuelta
                                long IdCliente = reader.GetInt64(reader.GetOrdinal("IdCliente"));
                                return IdCliente;
                            }
                            else
                            {
                                return -1;
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Manejar la excepción según sea necesario, ya sea registrando el error o lanzándolo nuevamente
                Console.WriteLine($"Error ejecutando el procedimiento: {ex.Message}");
                return 0;
            }
        }
    }
}
