using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oasis.Models;

namespace Oasis.ViewModel
{
    public class DetalleOrdenCompra
    {
        public int id_oc_principal { get; set; }
        //public string empresa { get; set; }
        //public int id_dpto { get; set; }
        public int id_proveedor { get; set; }
        public int id_departamento { get; set; }
        public int id_organizacion { get; set; }
        public System.DateTime fecha_documento { get; set; }
        public decimal valor_total { get; set; }
        public bool anulada { get; set; }
        //public int id_producto { get; set; } 
        //public decimal cantidad_producto { get; set; }
        //public decimal valor_linea { get; set; }
        public decimal descuento { get; set; }

        public List<prov_oc_detalle> ListaDeDetalleOrdenCompra { get; set; }

    }
}