using LacturaFacturaElectronica.DAO;
using LacturaFacturaElectronica.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacturaFacturaElectronica.BussinesLogic
{
    public class FacturaBL
    {
        readonly FacturaDB factura;
        public FacturaBL() { 
            factura = new FacturaDB();
        }
        public async Task<long> InsertFactura(FacturaModel model)
        {
            try
            {
                // Llamamos al método CrearEmpresa y retornamos el resultado
                long result = factura.CrearFactura(model);
                return result;
            }
            catch (Exception ex)
            {
                // Opcional: puedes registrar el error o manejarlo de alguna manera
                Console.WriteLine($"Error: {ex.Message}");
                return -1; // En caso de excepción, retornamos false
            }
        }
        public async Task<long> InsertDetalleFactura(DetalleFacturaModel model)
        {
            try
            {
                // Llamamos al método CrearEmpresa y retornamos el resultado
                long result = factura.CrearDetalleFactura(model);
                return result;
            }
            catch (Exception ex)
            {
                // Opcional: puedes registrar el error o manejarlo de alguna manera
                Console.WriteLine($"Error: {ex.Message}");
                return -1; // En caso de excepción, retornamos false
            }
        }
    }
}
