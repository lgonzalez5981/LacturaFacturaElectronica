using LacturaFacturaElectronica.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacturaFacturaElectronica.DAO
{
    public class ProductoDB
    {
        private readonly ConnectionDB _connectionDB;
        public ProductoDB()
        {
            _connectionDB = new ConnectionDB();
        }
        public long CrearProducto(ProductoModel model)
        {
            try
            {
                using (SqlConnection connection = _connectionDB.GetConnection())
                {
                    using (SqlCommand command = new SqlCommand("p_create_Producto", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar los parámetros necesarios para el procedimiento almacenado
                        command.Parameters.AddWithValue("@CodigoProducto", model.CodigoProducto);
                        command.Parameters.AddWithValue("@DescripcionProducto", model.DescripcionProducto);

                        connection.Open();

                        // Ejecutar el comando y leer el resultado del SELECT
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Obtener el valor de IdEmpresa de la columna devuelta
                                long IdProducto = reader.GetInt64(reader.GetOrdinal("IdProducto"));
                                return IdProducto;
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
