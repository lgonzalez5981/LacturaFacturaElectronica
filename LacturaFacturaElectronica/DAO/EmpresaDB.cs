using LacturaFacturaElectronica.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LacturaFacturaElectronica.DAO
{
    public class EmpresaDB
    {
        private readonly ConnectionDB _connectionDB;
        public EmpresaDB()
        {
            _connectionDB = new ConnectionDB();
        }
        public long CrearEmpresa(EmpresaModel model)
        {
            try
            {
                using (SqlConnection connection = _connectionDB.GetConnection())
                {
                    using (SqlCommand command = new SqlCommand("p_create_empresa", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar los parámetros necesarios para el procedimiento almacenado
                        command.Parameters.AddWithValue("@NitEmpresa", model.NitEmpresa);
                        command.Parameters.AddWithValue("@DescripcionEmpresa", model.DescripcionEmpresa);

                        connection.Open();

                        // Ejecutar el comando y leer el resultado del SELECT
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Obtener el valor de IdEmpresa de la columna devuelta
                                long idEmpresa = reader.GetInt64(reader.GetOrdinal("IdEmpresa"));
                                return idEmpresa;
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
