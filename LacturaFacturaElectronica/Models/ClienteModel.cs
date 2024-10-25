using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacturaFacturaElectronica.Models
{
    public class ClienteModel
    {
        public string? Identificacion { get; set; }
        public string? NombreCliente { get; set; }
        public long IdEmpresa { get; set; }
    }
}
