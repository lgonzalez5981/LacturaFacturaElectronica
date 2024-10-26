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
    public class DetalleFacturaModel
    {
        public long IdProducto { get; set; }
        public long IdFactura { get; set; }
        public int Cantidad { get; set; }
        public double ValorUnitario { get; set; }
        public double ValorTotal { get; set; }
        public double ValorImpuesto { get; set; }
    }
}
