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
    public class FacturaDB
    {
        private readonly ConnectionDB _connectionDB;
        public FacturaDB()
        {
            _connectionDB = new ConnectionDB();
        }
        public long CrearFactura(FacturaModel model)
        {
            try
            {
                using (SqlConnection connection = _connectionDB.GetConnection())
                {
                    using (SqlCommand command = new SqlCommand("p_create_Factura", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar los parámetros necesarios para el procedimiento almacenado
                        command.Parameters.AddWithValue("@NumberFacturaElectronica", model.NumberFacturaElectronica);
                        command.Parameters.AddWithValue("@IdEmpresa", model.IdEmpresa);
                        command.Parameters.AddWithValue("@IdCliente", model.IdCliente);
                        command.Parameters.AddWithValue("@FechaFactura", model.FechaFactura);

                        connection.Open();

                        // Ejecutar el comando y leer el resultado del SELECT
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Obtener el valor de IdEmpresa de la columna devuelta
                                long IdFactura = reader.GetInt64(reader.GetOrdinal("IdFactura"));
                                return IdFactura;
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
        public long CrearDetalleFactura(DetalleFacturaModel model)
        {
            try
            {
                using (SqlConnection connection = _connectionDB.GetConnection())
                {
                    using (SqlCommand command = new SqlCommand("p_create_Detalle_Factura", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar los parámetros necesarios para el procedimiento almacenado
                        command.Parameters.AddWithValue("@IdProducto", model.IdProducto);
                        command.Parameters.AddWithValue("@IdFactura", model.IdFactura);
                        command.Parameters.AddWithValue("@Cantidad", model.Cantidad);
                        command.Parameters.AddWithValue("@ValorUnitario", model.ValorUnitario);
                        command.Parameters.AddWithValue("@ValorTotal", model.ValorTotal);
                        command.Parameters.AddWithValue("@ValorImpuesto", model.ValorImpuesto);

                        connection.Open();

                        // Ejecutar el comando y leer el resultado del SELECT
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Obtener el valor de IdEmpresa de la columna devuelta
                                long IdDetalle = reader.GetInt64(reader.GetOrdinal("IdDetalle"));
                                return IdDetalle;
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
