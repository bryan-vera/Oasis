using Oasis.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oasis.ViewModel;
using Oasis.Models;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Oasis.Controllers.Credito
{
    public class PresupuestoController : Controller
    {
        [HttpGet]
        public ActionResult DuplicarPresupuesto (int id_presupuesto)
        {
            as2oasis oasis = new as2oasis();
            AS2Context as2 = new AS2Context();
            var presupuesto =
                oasis
                .presupuesto_cabecera
                .AsEnumerable()
                .Where(x => x.id_presupuesto == id_presupuesto)
                .Select(x => new presupuesto_cabecera
                {
                    id_presupuesto = x.id_presupuesto,
                    descripcion = x.descripcion,
                    empresa = x.empresa,
                    sucursal = x.sucursal,
                    fecha_desde = x.fecha_desde,
                    fecha_hasta = x.fecha_hasta
                })
                .First();

            List<Presupuesto_Vendedor_Detalle> detallePresupuesto = 
                oasis.Presupuesto_Vendedor_Detalle
                .AsEnumerable()
                .Where(x => x.id_presupuesto == id_presupuesto)
                .ToList();
            
            ViewBag.DetallePresupuesto = detallePresupuesto;

            var detalleP = new DetallePresupuesto();
            detalleP.id_presupuesto = presupuesto.id_presupuesto;
            detalleP.empresa = presupuesto.empresa;
            detalleP.sucursal = presupuesto.sucursal;
            detalleP.descripcion = presupuesto.descripcion;
            detalleP.fecha_desde = presupuesto.fecha_desde;
            detalleP.fecha_hasta = presupuesto.fecha_hasta;
            detalleP.ListaPresupuestoDetalle = detallePresupuesto;

            var serializer = new JavaScriptSerializer();
            ViewBag.PresupuestoSeleccionado = serializer.Serialize(detalleP);

            return View(detalleP);
        }


        public JsonResult ObtenerVendedores(string empresa,string sucursal,string textoBusqueda)
        {
            as2oasis oasis = new as2oasis();
            var vendedores = oasis.Vendedores
                .AsEnumerable()
                .Where(
                x =>
                x.empresa == empresa && x.sucursal == sucursal &&
                x.username.ToLower().Contains(textoBusqueda.ToLower()))
                .GroupBy(x => new { x.id_vendedor, x.username })
                .Select(x => new
                {
                    id_vendedor = x.Key.id_vendedor,
                    username = x.Key.username,
                })
                .Take(5)
                ;

            //as2.empresa.Where(x=>x.) .invt_productos_gastos.Where(x => x.descripcion.Contains(textoBusqueda)).ToList();
            return Json(vendedores, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult AnularPresupuesto(int id_presupuesto)
        {
            return null;
        }


        [HttpPost]
        public ActionResult Create(DetallePresupuesto presupuesto)
        {
            bool status = false;

            using (as2oasis oasis = new as2oasis())
            {
                bool duplicado = oasis.presupuesto_cabecera.Any(x => x.empresa == presupuesto.empresa
                    && x.sucursal == presupuesto.sucursal
                    && x.fecha_desde <= presupuesto.fecha_hasta
                    && x.fecha_hasta >= presupuesto.fecha_desde);

                if (!duplicado)
                {

                    var p_cabecera = new presupuesto_cabecera();
                    p_cabecera.empresa = presupuesto.empresa;
                    p_cabecera.sucursal = presupuesto.sucursal;
                    p_cabecera.descripcion = presupuesto.descripcion;
                    p_cabecera.fecha_desde = presupuesto.fecha_desde;
                    p_cabecera.fecha_hasta = presupuesto.fecha_hasta;
                    p_cabecera.activo = true;
                    oasis.presupuesto_cabecera.Add(p_cabecera);
                    oasis.SaveChanges();

                    int pk = p_cabecera.id_presupuesto;  // You can get primary key of your inserted row
                    foreach (var i in presupuesto.ListaPresupuestoDetalle)
                    {

                        var p_detalle = new presupuesto_detalle();
                        p_detalle.presupuesto_cabecera = p_cabecera;
                        p_detalle.id_vendedor = i.id_vendedor;
                        p_detalle.valor_cobro = i.valor_cobro;
                        p_detalle.valor_venta = i.valor_venta;
                        oasis.presupuesto_detalle.Add(p_detalle);
                    }
                    oasis.SaveChanges();
                    status = true;

                    ModelState.Clear();


                    ViewBag.SuccessMessage = "Se ha registrado el producto";
                    return new JsonResult { Data = new { status = status } };

                }
                else
                {
                    return null;
                }
            }
        }

        // GET: Presupuesto
        // GET: DetalleOrdenCompra
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            OASISContext oc = new OASISContext();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "empresa" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            presupuesto_cabecera pre = new presupuesto_cabecera();
            //AS2Context as2 = new AS2Context();
            as2oasis oasis = new as2oasis();
            var presupuesto =
                oasis
                .presupuesto_cabecera
                .AsEnumerable()
                .Where(x=>x.activo==true)
                .Select(x => new presupuesto_cabecera
                {
                    id_presupuesto=x.id_presupuesto,
                    descripcion = x.descripcion,
                    empresa = x.empresa,
                    sucursal = x.sucursal,
                    fecha_desde = x.fecha_desde,
                    fecha_hasta = x.fecha_hasta
                })
                ;

            if (!String.IsNullOrEmpty(searchString))
            {
                presupuesto = presupuesto.Where(s => s.descripcion.Contains(searchString)
                                       || s.empresa.Contains(searchString)
                                       || s.sucursal.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "empresa":
                    presupuesto = presupuesto.OrderByDescending(s => s.empresa);
                    break;
                default:
                    presupuesto = presupuesto.OrderBy(s => s.fecha_hasta);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(presupuesto.ToPagedList(pageNumber, pageSize));


            //List<DetalleOrdenCompra> DetalleOrdenCompralist = new List<DetalleOrdenCompra>(); // to hold list of Customer and order details
            //List<prov_oc_principal> OrdenCompraLista = new List<prov_oc_principal>(); // to hold list of Customer and order details
            //return View((from prov_oc_principal in oc.prov_oc_principal.Take(10)
            //             select prov_oc_principal).ToList());
        }
    }
}