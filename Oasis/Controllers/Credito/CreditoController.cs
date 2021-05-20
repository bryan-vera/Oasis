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
            public string fecha_aplicacion;
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
                    vta.valor_factura = ((float)item.valor_factura).ToString("N2", System.Globalization.CultureInfo.InvariantCulture);
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
                    cbr.fecha_aplicacion = ((DateTime)item.fecha_aplicacion).ToShortDateString();
                    cbr.fecha_creacion = ((DateTime)item.fecha_creacion).ToShortDateString();
                    cbr.codigo_cobro = item.codigo;
                    cbr.secuencial_factura = item.secuencial_factura;
                    cbr.cliente = item.nombre_comercial;
                    cbr.categoria = item.categoria;
                    cbr.valor_cobro = ((float)item.valor).ToString("N2", System.Globalization.CultureInfo.InvariantCulture);
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
                    nc_.valor_nc = ((float)item.valor_nc).ToString("N2", System.Globalization.CultureInfo.InvariantCulture);
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

        public JsonResult ObtenerCartera(
            string empresa,
            string sucursal,
            string tipoCliente)
        {
            var tipoC = JsonConvert.DeserializeObject(tipoCliente);
            var tipoCliente_ = tipoCliente.Replace(@"[", string.Empty).Replace(@"]", string.Empty).Replace("\"", string.Empty);
        
            using (var context = new as2oasis())
            {
                var cartera = 
                    context.CarteraEmpresa(empresa, sucursal, tipoCliente_)
                    .Select(x => new
                    {
                        x.empresa,
                        x.sucursal,
                        x.identificacion,
                        x.nombre_comercial,
                        x.categoria,
                        x.vendedor_cliente,
                        x.vendedor_factura,
                        secuencial_factura=(x.secuencial_factura.Replace("-", string.Empty)).Substring(2, x.secuencial_factura.Length-4),
                        fecha_factura = x.fecha_factura.ToShortDateString(),
                        fecha_vencimiento = x.fecha_vencimiento.Value.ToShortDateString(),
                        x.provincia,
                        x.ciudad,
                        x.parroquia,
                        x.direccion,
                        valor_factura = x.valor_factura,
                        totalChequePost = x.totalChequePost,
                        saldo_pendiente = x.saldo_pendiente,
                        x.dias_emitida,
                        x.dias_diferencia
                    });
                var cartera_json = JsonConvert.SerializeObject(cartera, Formatting.Indented);

                return Json(cartera_json, JsonRequestBehavior.AllowGet);
                //foreach (Course cs in courses)
                //    Console.WriteLine(cs.CourseName);
            }
            //return View();
        }

        public JsonResult ObtenerCarteraVisitador(
            string empresa,
            string sucursal,
            string tipoCliente,
            string visitador)
        {
            var tipoC = JsonConvert.DeserializeObject(tipoCliente);
            var tipoCliente_ = tipoCliente.Replace(@"[", string.Empty).Replace(@"]", string.Empty).Replace("\"", string.Empty);

            using (var context = new as2oasis())
            {
                var anticipo =
                    context.Anticipos
                    .AsEnumerable()
                    .Where(x => x.empresa == empresa &&
                        x.sucursal == sucursal &&
                        x.id_vistador == Int16.Parse(visitador))
                    .Select(x => new
                    {
                        empresa = x.empresa,
                        sucursal = x.sucursal,
                        identificacion =x.ruc_cliente,
                        nombre_comercial =x.nombre_cliente,
                        categoria = x.categoria,
                        vendedor_cliente = x.visitador,
                        vendedor_factura = x.visitador,
                        tipo_documento = "ANT.",
                        secuencial_factura =x.sencuencia_documento,
                        fecha_factura=x.fecha_documento.Value.ToShortDateString(),
                        fecha_vencimiento="",
                        provincia="",
                        ciudad="",
                        parroquia="",
                        direccion="",
                        valor_factura  = (Decimal?)0.00,
                        totalChequePost = (Decimal)0.00,
                        saldo_pendiente = (Decimal?)(x.valor_anticipo*-1),
                        dias_emitida = (int?)0,
                        dias_diferencia = (int?)0
                    });

                var cartera = 
                    (context.CarteraEmpresa(empresa, sucursal, tipoCliente_))
                    .AsEnumerable()
                    .Where(x => x.id_vendedor_cliente == Int16.Parse(visitador) || x.id_vendedor_factura==Int16.Parse(visitador))
                    .Select(x => new
                    {
                        x.empresa,
                        x.sucursal,
                        x.identificacion,
                        x.nombre_comercial,
                        x.categoria,
                        x.vendedor_cliente,
                        x.vendedor_factura,
                        tipo_documento = "FACT.",
                        secuencial_factura =(x.secuencial_factura.Replace("-", string.Empty)).Substring(2, x.secuencial_factura.Length-4),
                        fecha_factura = x.fecha_factura.ToShortDateString(),
                        fecha_vencimiento = x.fecha_vencimiento.Value.ToShortDateString(),
                        x.provincia,
                        x.ciudad,
                        x.parroquia,
                        x.direccion,
                        valor_factura = x.valor_factura,
                        totalChequePost = x.totalChequePost,
                        saldo_pendiente = x.saldo_pendiente,
                        x.dias_emitida,
                        x.dias_diferencia
                    });
                var cartera_json = JsonConvert.SerializeObject(cartera.Concat(anticipo), Formatting.Indented);

                return Json(cartera_json, JsonRequestBehavior.AllowGet);
                //foreach (Course cs in courses)
                //    Console.WriteLine(cs.CourseName);
            }
            //return View();
        }

        // GET: Consolidado
        public ActionResult Consolidado()
        {
            ViewBag.Opciones = ListaEmpresas();
            return View();
        }



        // GET: Cartera
        public ActionResult Cartera()
        {
            ViewBag.Opciones = ViewBag.Opciones = ListaEmpresas();
            return View();
        }

        public JsonResult ObtenerNCAfectacion(
            string empresa,
            string sucursal,
            string fecha_desde,
            string fecha_hasta,
            string tipocliente,
            string visitador = null
            )
        {
            DateTime fecha_desde_ = DateTime.Parse(fecha_desde);
            DateTime fecha_hasta_ = DateTime.Parse(fecha_hasta);


            var tipoCliente_ = tipocliente.Replace(@"[", string.Empty).Replace(@"]", string.Empty).Replace("\"", string.Empty);
            string[] categoriaCliente = tipoCliente_.Split(',');

            using (var context = new as2oasis())
            {
                var nc =
                    context.NC_Consolidado
                    .Where(x => x.empresa == empresa &&
                                x.sucursal == sucursal &&
                                categoriaCliente.Contains(x.categoria)
                            );

                if (!String.IsNullOrEmpty(visitador))
                {
                    var codigo_visitador = Int16.Parse(visitador);
                    nc = nc.Where(x => x.id_vendedor == codigo_visitador);
                }

                var listaNC = nc
                    .ToList()
                    .Select(x => new {
                        x.empresa,
                        x.sucursal,
                        x.identificacion,
                        x.nombre_fiscal,
                        fecha_documento = x.fecha_documento.Value.ToShortDateString(),
                        x.secuencial_nc,
                        x.motivo_nc,
                        valor = x.valor_nc.Value.ToString("N2"),
                        fecha_factura = x.fecha_factura.Value.ToShortDateString(),
                        factura = x.numero_factura,
                        x.vendedor,
                    });

                var nc_json = JsonConvert.SerializeObject(listaNC, Formatting.Indented);
                return Json(nc_json, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ObtenerChequesPost(
            string empresa,
            string sucursal,
            string tipoCliente,
            string visitador = null)
        {
            var tipoC = JsonConvert.DeserializeObject(tipoCliente);
            var tipoCliente_ = tipoCliente.Replace(@"[", string.Empty).Replace(@"]", string.Empty).Replace("\"", string.Empty);
            string[] categoriaCliente = tipoCliente_.Split(',');

            using (var context = new as2oasis())
            {
                var chequesPost =
                    context.Cheques_Postfechados
                    .Where(x => x.empresa == empresa &&
                                x.sucursal == sucursal &&
                                categoriaCliente.Contains(x.categoria)
                            );

                if (!String.IsNullOrEmpty(visitador))
                {
                    var codigo_visitador = Int16.Parse(visitador);
                    chequesPost = chequesPost.Where(x => x.id_usuario == codigo_visitador);
                }

                var listaCHQPost = chequesPost
                    .ToList()
                    .Select(x => new {
                        x.empresa,
                        x.sucursal,
                        x.codigo,
                        x.nombre_cliente,
                        x.codigo_cobro,
                        valor = x.valor.ToString("N2"),
                        fecha_aplicacion = x.fecha_cobro.Value.ToShortDateString(),
                        fecha_creacion = x.fecha_creacion.Value.ToShortDateString(),
                        x.secuencial_factura,
                        x.dias_credito_otorgado,
                        x.vendedor,
                        descripcion = x.descripcion +" " +x.descripcion2
                    });


                var chequesPost_json = JsonConvert.SerializeObject(listaCHQPost, Formatting.Indented);

                return Json(chequesPost_json, JsonRequestBehavior.AllowGet);
             }
        }

        public ActionResult ChequesPost()
        {
            ViewBag.Opciones = ListaEmpresas();
            ViewBag.Title = "Cheques postfechados";
            return View();
        }

        public ActionResult NotasCredito()
        {
            ViewBag.Opciones = ListaEmpresas();
            ViewBag.Title = "Notas de crédito";
            return View();
        }

        public List<SelectListItem>  ListaEmpresas()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            //lst.Add(new SelectListItem() { Text = "LABOVIDA", Value = "LABOV" });
            lst.Add(new SelectListItem() { Text = "LEBENFARMA", Value = "LEBENFARMA S.A." });
            lst.Add(new SelectListItem() { Text = "FARMALIGHT", Value = "FARMALIGHT S.A." });
            lst.Add(new SelectListItem() { Text = "DANIVET", Value = "LABORATORIOS DANIVET S.A." });
            lst.Add(new SelectListItem() { Text = "DANIVET 2", Value = "LABORATORIOS DANIVET S.A. 2" });
            lst.Add(new SelectListItem() { Text = "ANYUPA", Value = "LABORATORIOS ANYUPA S.A." });
            lst.Add(new SelectListItem() { Text = "MEDITOTAL", Value = "MEDITOTAL S.A." });
            return lst;
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

        [HttpGet]
        public JsonResult SugerirFechas(string empresa, string sucursal)
        {
            as2oasis oasis = new as2oasis();
            DateTime hoy = DateTime.Today;

            var resultadoFecha =
                oasis.presupuesto_cabecera.Where(x => x.empresa == empresa
                && x.sucursal == sucursal
                && (x.fecha_desde <= hoy
                && x.fecha_hasta >= hoy)).ToList()
                .Select(x=>new {
                        fecha_desde=x.fecha_desde.ToShortDateString(),
                        fecha_hasta=x.fecha_hasta.ToShortDateString()
                }).FirstOrDefault();
            var msjError = new {MensajeError="No se puede obtener una fecha sugerida"};
            var resultadoFecha_json = JsonConvert.SerializeObject(resultadoFecha, Formatting.Indented,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

            if (resultadoFecha is null) { 
                return Json(msjError, JsonRequestBehavior.AllowGet);
            }

            return Json(resultadoFecha_json, JsonRequestBehavior.AllowGet);
           
        }
    }
}
