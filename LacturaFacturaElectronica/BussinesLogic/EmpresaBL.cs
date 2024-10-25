using LacturaFacturaElectronica.DAO;
using LacturaFacturaElectronica.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacturaFacturaElectronica.BussinesLogic
{
    public class EmpresaBL
    {
        readonly EmpresaDB Empresa;
        public EmpresaBL()
        {
            Empresa = new EmpresaDB();
        }
        public async Task<long> InsertUpdateEmpresa(EmpresaModel model)
        {
            try
            {
                // Llamamos al método CrearEmpresa y retornamos el resultado
                long result = Empresa.CrearEmpresa(model);
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
