using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacturaFacturaElectronica.Models
{
    public class FacturaModel
    {
        public string? NumberFacturaElectronica { get; set; }
        public long IdEmpresa { get; set; }
        public long IdCliente { get; set; }
        public DateTime FechaFactura { get; set; }
    }
}
