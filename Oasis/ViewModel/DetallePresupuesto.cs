using System;
using System.Collections.Generic;
using Oasis.Models;
using System.Linq;
using System.Web;

namespace Oasis.ViewModel
{
    public class DetallePresupuesto
    {
        public int id_presupuesto { get; set; }
        public string empresa { get; set; }
        public string sucursal { get; set; }
        public string descripcion { get; set; }
        public System.DateTime fecha_desde { get; set; }
        public System.DateTime fecha_hasta { get; set; }

        public List<Presupuesto_Vendedor_Detalle> ListaPresupuestoDetalle { get; set; }

    }
}