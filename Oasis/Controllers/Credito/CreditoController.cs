using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Oasis.Models;

namespace Oasis.Controllers.Credito
{
    public class CreditoController : Controller
    {
        // GET: Credito
        public ActionResult Index()
        {
            return View();
        }

        public class VentasXVendedor{
            public string fecha_creacion;
            public string secuencial_factura;
            public string cliente;
            public string ciudad;
            //public string categoria;
            public string valor_factura;
        }

        public class CobrosXVendedor
        {
            public string fecha_creacion;
            public string secuencial_factura;
            public string codigo_cobro;
            public string cliente;
            public string categoria;
            public string valor_cobro;
        }

        public class NCXVendedor
        {
            public string fecha_creacion;
            public string secuencial_factura;
            public string secuencial_nc;
            public string cliente;
            public string motivo;
            public string valor_nc;
        }

        public JsonResult ObtenerVentasPorVendedor(
            string empresa,
            string sucursal,
            string fecha_desde,
            string fecha_hasta,
            string tipocliente,
            string vendedor
            )
        {
            DateTime fecha_desde_ = DateTime.Parse(fecha_desde);
            DateTime fecha_hasta_ = DateTime.Parse(fecha_hasta);

            var tipoCliente_ = tipocliente.Replace(@"[", string.Empty).Replace(@"]", string.Empty).Replace("\"", string.Empty);

            using (var context = new as2oasis())
            {
                var ventas = context.VentasPorVendedor(empresa, sucursal, fecha_desde_, fecha_hasta_, tipoCliente_,vendedor);
                List<VentasXVendedor> Ventas = new List<VentasXVendedor>();
                foreach (var item in ventas)
                {
                    var vta = new VentasXVendedor();
                    vta.fecha_creacion = ((DateTime)item.fecha_factura).ToShortDateString();
                    vta.secuencial_factura = item.secuencial_factura;
                    vta.cliente = item.nombre_comercial;
                    vta.ciudad = item.ciudad;
                    vta.valor_factura = ((float)item.valor_factura).ToString("N2");
                    Ventas.Add(vta);
                }
                var ventas_json = JsonConvert.SerializeObject(Ventas, Formatting.Indented);

                return Json(ventas_json, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ObtenerCobrosPorVendedor(
            string empresa,
            string sucursal,
            string fecha_desde,
            string fecha_hasta,
            string tipocliente,
            string vendedor
            )
        {
            DateTime fecha_desde_ = DateTime.Parse(fecha_desde);
            DateTime fecha_hasta_ = DateTime.Parse(fecha_hasta);

            var tipoCliente_ = tipocliente.Replace(@"[", string.Empty).Replace(@"]", string.Empty).Replace("\"", string.Empty);

            using (var context = new as2oasis())
            {
                var cobros = context.CobrosPorVendedor(empresa, sucursal, fecha_desde_, fecha_hasta_, tipoCliente_, vendedor);
                List<CobrosXVendedor> Cobros = new List<CobrosXVendedor>();
                foreach (var item in cobros)
                {
                    var cbr = new CobrosXVendedor();
                    
                    cbr.fecha_creacion = ((DateTime)item.fecha_aplicacion).ToShortDateString();
                    cbr.codigo_cobro = item.codigo;
                    cbr.secuencial_factura = item.secuencial_factura;
                    cbr.cliente = item.nombre_comercial;
                    cbr.categoria = item.categoria;
                    cbr.valor_cobro = ((float)item.valor).ToString("N2");
                    Cobros.Add(cbr);
                }


                var cobros_json = JsonConvert.SerializeObject(Cobros, Formatting.Indented);

                return Json(cobros_json, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ObtenerNCPorVendedor(
            string empresa,
            string sucursal,
            string fecha_desde,
            string fecha_hasta,
            string tipocliente,
            string vendedor
            )
        {
            DateTime fecha_desde_ = DateTime.Parse(fecha_desde);
            DateTime fecha_hasta_ = DateTime.Parse(fecha_hasta);

            var tipoCliente_ = tipocliente.Replace(@"[", string.Empty).Replace(@"]", string.Empty).Replace("\"", string.Empty);

            using (var context = new as2oasis())
            {
                var nc = context.NCPorVendedor(empresa, sucursal, fecha_desde_, fecha_hasta_, tipoCliente_, vendedor);
                List<NCXVendedor> NC = new List<NCXVendedor>();
                foreach (var item in nc)
                {
                    var nc_ = new NCXVendedor();
                    nc_.fecha_creacion = ((DateTime)item.fecha_documento).ToShortDateString();
                    nc_.secuencial_factura = item.numero_factura;
                    nc_.secuencial_nc = item.secuencial_nc;
                    nc_.cliente = item.nombre_fiscal;
                    nc_.motivo = item.motivo_nc;
                    nc_.valor_nc = ((float)item.valor_nc).ToString("N2");
                    NC.Add(nc_);
                }

                var nc_json = JsonConvert.SerializeObject(NC, Formatting.Indented);

                return Json(nc_json, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ObtenerPresupuesto(
            string empresa, 
            string sucursal,
            string fecha_desde,
            string fecha_hasta, 
            string tipoCliente) {

            DateTime fecha_desde_ = DateTime.Parse(fecha_desde);
            DateTime fecha_hasta_ = DateTime.Parse(fecha_hasta);
            var tipoC = JsonConvert.DeserializeObject(tipoCliente);
            //string csv = jsonToCSV(tipoCliente, ",");

            //var lista = tipoC.Split(',');
            var tipoCliente_ = tipoCliente.Replace(@"[", string.Empty).Replace(@"]", string.Empty).Replace("\"",string.Empty);
                
            using (var context = new as2oasis())
            {
                var presupuesto = context.Presupuesto(empresa, sucursal, fecha_desde_, fecha_hasta_,tipoCliente_);
                var presupuesto_json = JsonConvert.SerializeObject(presupuesto, Formatting.Indented);

                return Json(presupuesto_json,JsonRequestBehavior.AllowGet);
                //foreach (Course cs in courses)
                //    Console.WriteLine(cs.CourseName);
            }
            //return View();
        }

        // GET: Consolidado
        public ActionResult Consolidado()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem() { Text = "LABOVIDA", Value = "LABOV" });
            lst.Add(new SelectListItem() { Text = "LEBENFARMA", Value = "LEBENFARMA S.A." });
            lst.Add(new SelectListItem() { Text = "FARMALIGHT", Value = "FARMALIGHT S.A." });
            lst.Add(new SelectListItem() { Text = "DANIVET", Value = "LABORATORIOS DANIVET S.A." });
            lst.Add(new SelectListItem() { Text = "DANIVET 2", Value = "LABORATORIOS DANIVET S.A. 2" });
            lst.Add(new SelectListItem() { Text = "ANYUPA", Value = "LABORATORIOS ANYUPA S.A." });
            lst.Add(new SelectListItem() { Text = "MEDITOTAL", Value = "MEDITOTAL S.A." });

            ViewBag.Opciones = lst;

            return View();
        }


        // GET: Credito/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Credito/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Credito/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Credito/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }


        // POST: Credito/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Credito/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Credito/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
