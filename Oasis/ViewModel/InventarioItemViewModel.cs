using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Oasis.Models;

namespace Oasis.ViewModel
{
    public class InventarioItemViewModel
    {
        [DisplayName("ID")]
        public int id_producto { get; set; }
        [DisplayName("DESCRIPCION")]
        public string descripcion { get; set; }
        [DisplayName("V.U.")]
        public decimal valor_unitario { get; set; }
        [DisplayName("CATEGORIA")]
        public IEnumerable<invt_categoria> Categoria { get; set; }
        [DisplayName("UM")]
        public string um { get; set; }

    }
}