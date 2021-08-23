using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Oasis.Models;
using Oasis.Models.ATS;

namespace Oasis.Controllers.Contabilidad
{
    public class ContabilidadController : Controller
    {
        // GET: Contabilidad
        public ActionResult Autorizaciones()
        {
            return View();
        } 

        public ActionResult VerATS()
        {
            return View();
        }

        public ActionResult CuentasPorPagar()
        {
            ViewBag.Opciones = ListaEmpresas();
            ViewBag.Title = "Cuentas por pagar";
            return View();
        }

        public List<SelectListItem> ListaEmpresas()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem() { Text = "LABOVIDA", Value = "LABOVIDA S.A." });
            lst.Add(new SelectListItem() { Text = "LEBENFARMA", Value = "LEBENFARMA S.A." });
            lst.Add(new SelectListItem() { Text = "FARMALIGHT", Value = "FARMALIGHT S.A." });
            lst.Add(new SelectListItem() { Text = "DANIVET", Value = "LABORATORIOS DANIVET S.A." });
            lst.Add(new SelectListItem() { Text = "DANIVET 2", Value = "LABORATORIOS DANIVET S.A. 2" });
            lst.Add(new SelectListItem() { Text = "ANYUPA", Value = "LABORATORIOS ANYUPA S.A." });
            lst.Add(new SelectListItem() { Text = "MEDITOTAL", Value = "MEDITOTAL S.A." });
            return lst;
        }

        public JsonResult ObtenerCarteraPorPagar(
           string empresa,
           string fecha_desde,
           string fecha_hasta
           )
        {

            DateTime fecha_desde_ = DateTime.Parse(fecha_desde);
            DateTime fecha_hasta_ = DateTime.Parse(fecha_hasta);

            using (var context = new as2oasis())
            {
                var cartera =
                    context.Cartera_Proveedor
                    .Where(x => x.empresa == empresa &&  
                                x.fecha_factura >= fecha_desde_ &&
                                x.fecha_factura <= fecha_hasta_
                            );

                var lista_cartera = cartera
                    .ToList()
                    .Select(x => new {
                        x.empresa,
                        x.identificacion,
                        x.nombre_comercial,
                        x.categoria,
                        secuencial_factura = (x.secuencial_factura.Replace("-", string.Empty)),
                        fecha_factura = x.fecha_factura.ToShortDateString(),
                        fecha_vencimiento = x.fecha_vencimiento.Value.ToShortDateString(),
                        x.provincia,
                        x.ciudad,
                        x.parroquia,
                        x.direccion,
                        valor_factura = x.valor_factura, 
                        saldo_pendiente = x.saldo,
                        //x.dias_emitida,
                        //x.dias_diferencia
                    });

                //var cartera_json = JsonConvert.SerializeObject(lista_cartera, Formatting.Indented);
                return Json(lista_cartera, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public FileResult DevolverATS(string empresa, string año, string mes )
        {
             
            string dia = "01";
            var fecha_inicial = DateTime.Parse(año + "/" + mes + "/" + dia).Date;
            var fecha_final = DateTime.Parse((año + "/" + mes + "/" + DateTime.DaysInMonth(fecha_inicial.Year, fecha_inicial.Month)));

            var r = new Retenciones();
            r.Empresa = empresa;
            r.FechaInicio = fecha_inicial;
            r.FechaFin = fecha_final;
            var stream = new MemoryStream();
            var respuesta_xml = r.ObtenerRetenciones();
            var serializer = new XmlSerializer(typeof(Iva));
            serializer.Serialize(stream, respuesta_xml);
            stream.Position = 0;
            return File(stream, "application/xml", "ATS-" + año + "-" + mes + "-" + empresa + ".xml");

        }

        [HttpPost]
        public JsonResult RevisarTXT(HttpPostedFileBase file)
        {
            string contents = string.Empty; 
            string sep = "\t";

            using (var ms = new System.IO.MemoryStream())
            {
                file.InputStream.CopyTo(ms);
                ms.Position = 0;
                contents = new StreamReader(ms).ReadToEnd();
            }

            contents=contents.Replace("\n","\t");
            contents = contents.Replace("Factura\n", "Factura ");

            var array = contents.Split(sep.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            
            DataTable tablaAutorizaciones = new DataTable();
            // create columns
            for (int i = 0; i < 10; i++)
            {
                tablaAutorizaciones.Columns.Add();
            }

            for (int j = 1; j < array.Length-20; j++)
            {
                DataRow row = tablaAutorizaciones.NewRow();
                for (int i = 0; i < 10; i++)
                {
                    row[i] = array[11+j+i];
                }
                j = j + 10;
                // add the current row to the DataTable
                tablaAutorizaciones.Rows.Add(row);
            }

            var claveAccesos = tablaAutorizaciones.AsEnumerable()
                .Select(x=>new
                {
                   factura = x.Field<string>(0),
                   proveedor = x.Field<string>(2),
                   claveAcceso =  x.Field<string>(8)
                })
                .ToList()
                ;

            using (var context = new AS2Context())
            {

                var AS2ClavesAcceso =
                   context.factura_proveedorSRI
                   .Select(x => x.autorizacion)
                   .ToList();

                var listaClavesPendientes =
                    claveAccesos
                        .AsEnumerable()
                        .Where(x => !AS2ClavesAcceso.Contains(x.claveAcceso));

                var clavesSRI_json = JsonConvert.SerializeObject(listaClavesPendientes, Formatting.Indented);
                return Json(clavesSRI_json, JsonRequestBehavior.AllowGet);                
                    
            }
          
    }
    }
}
