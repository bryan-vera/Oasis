using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oasis.Models;
using Oasis.ViewModel;

namespace Oasis.Controllers.Proveedores
{
    public class DetalleOrdenCompraController : Controller
    {
        // GET: DetalleOrdenCompra
        public ActionResult Index()
        {
            OASISContext oc = new OASISContext();
            List<DetalleOrdenCompra> DetalleOrdenCompralist = new List<DetalleOrdenCompra>(); // to hold list of Customer and order details
            var DetalleOCLista = (from OCPrincipal in oc.prov_oc_principal
                                join OCDetalle in oc.prov_oc_detalle
                                on OCPrincipal.id_oc_principal equals OCDetalle.id_oc_principal
                                select new { OCPrincipal.id_oc_principal, OCPrincipal.id_proveedor,
                                    OCPrincipal.fecha_documento,
                                    OCPrincipal.valor_total,
                                    OCDetalle.id_producto,
                                    OCDetalle.cantidad_producto,
                                    OCDetalle.descuento,
                                    OCDetalle.valor_linea,
                                OCPrincipal.anulada}).ToList();

            foreach (var item in DetalleOCLista)

            {
                DetalleOrdenCompra obj = new DetalleOrdenCompra(); // ViewModel
                obj.anulada = item.anulada;
                obj.cantidad_producto = item.cantidad_producto;
                obj.descuento = item.descuento;
                obj.fecha_documento = item.fecha_documento;
                obj.id_oc_principal = item.id_oc_principal;
                obj.id_producto = item.id_producto;
                obj.id_proveedor = item.id_proveedor;
                obj.valor_total = item.valor_total;
                obj.valor_linea = item.valor_linea;
                DetalleOrdenCompralist.Add(obj);
            }

            return View(DetalleOrdenCompralist);
        }

        [HttpPost]
        public ActionResult Create(List<DetalleOrdenCompra> ocModel)
        {
            using (OASISContext oasis = new OASISContext())
            {
                var oc_principal = new prov_oc_principal();
                oc_principal.fecha_documento = ocModel[0].fecha_documento;
                oc_principal.id_proveedor = ocModel[0].id_proveedor;
                oc_principal.valor_total = ocModel[0].valor_total;
                oasis.prov_oc_principal.Add(oc_principal);
                oasis.SaveChanges();

                int pk = oc_principal.id_oc_principal;  // You can get primary key of your inserted row
                foreach (var i in ocModel)
                {
                    var oc_detalle = new prov_oc_detalle();
                    oc_detalle.prov_oc_principal = oc_principal;
                    oc_detalle.cantidad_producto = i.cantidad_producto;
                    oc_detalle.descuento = i.descuento;
                    oc_detalle.id_producto = i.id_producto;
                    oc_detalle.valor_linea = i.valor_linea;
                    oasis.prov_oc_detalle.Add(oc_detalle);
                }
                oasis.SaveChanges();
                ViewBag.Message = "Se ha confirmado la orden de compra " + ocModel[0].id_oc_principal;

                //if (oasis.prov_oc_principal.Any(x => x.id_oc_principal == ocModel.))
                //{
                //    ViewBag.ProductoDuplicado = true;
                //    return View("Create", productoModel);
                //}

                //oasis.invt_productos_gastos.Add(productoModel);
                //oasis.SaveChanges();
            }
            ModelState.Clear();
            ViewBag.SuccessMessage = "Se ha registrado el producto";
            return View("Create", new invt_productos_gastos());
        }



        public ActionResult Create()
        {
            DetalleOrdenCompra DetalleOC = new DetalleOrdenCompra();
            departamentos departamento = new departamentos();
            List<SelectListItem> lst = new List<SelectListItem>();
            AS2Context as2 = new AS2Context();
            OASISContext oasis = new OASISContext();

            lst.Add(new SelectListItem() { Text = "LABOVIDA", Value = "LABOV" });
            lst.Add(new SelectListItem() { Text = "LEBENFARMA", Value = "LEBEN" });
            lst.Add(new SelectListItem() { Text = "FARMALIGHT", Value = "FARMA" });
            lst.Add(new SelectListItem() { Text = "DANIVET", Value = "DANIV" });
            lst.Add(new SelectListItem() { Text = "ANYUPA", Value = "ANYUP" });
            lst.Add(new SelectListItem() { Text = "MEDITOTAL", Value = "MEDIT" });

            var proveedores =
               as2.empresa
               .AsEnumerable()
               .Where(x => x.indicador_proveedor == true && x.activo == true)
               .GroupBy(x => new { x.identificacion, x.email1, x.nombre_comercial })
               .Select(x => new empresa
               {
                   identificacion = x.Key.identificacion,
                   nombre_comercial = x.Key.nombre_comercial,
               })
               ;

            var lista_departamentos =
                oasis
               .departamentos
               .AsEnumerable()
               .Select(x => new departamentos
               {
                   ID_DPTO = x.ID_DPTO,
                   NOMBRE_DEPARTAMENTO = x.NOMBRE_DEPARTAMENTO
               })
               ;


            ViewBag.Opciones = lst;

            ViewBag.Proveedores = new SelectList(proveedores, "identificacion", "nombre_comercial");
            ViewBag.Departamentos = new SelectList(lista_departamentos, "ID_DPTO", "NOMBRE_DEPARTAMENTO");

            return View(DetalleOC);
        }
    }
}