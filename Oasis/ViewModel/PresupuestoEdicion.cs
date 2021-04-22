using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oasis.ViewModel
{
    public class PresupuestoEdicion
    {
        public int id_presupuesto_cabecera { get; set; }
        public string empresa { get; set; }
        public string sucursal { get; set; }
        public string descripcion { get; set; }
        public System.DateTime fecha_desde { get; set; }
        public System.DateTime fecha_hasta { get; set; }
        public int id_vendedor { get; set; }
        public float valor_venta { get; set; }
        public float valor_cobro { get; set; }
        //public bool anulada { get; set; }
        //public decimal descuento { get; set; }

        //public List<prov_oc_detalle> ListaDeDetalleOrdenCompra { get; set; }
    }
}