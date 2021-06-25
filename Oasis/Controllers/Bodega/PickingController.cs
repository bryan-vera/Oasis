using Newtonsoft.Json;
using Oasis.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Oasis.Controllers.Bodega
{
    public class PickingController : Controller
    {
        public class DatoPicking
        {
            public int id_factura { get; set; }
            public int codigo_estado { get; set; }
            public string secuencial { get; set; }
            public string empresa { get; set; }
            public int id_usuario { get; set; }
        }

        //private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
        //new TodoItemDTO
        //{
        //    Id = todoItem.Id,
        //    Name = todoItem.Name,
        //    IsComplete = todoItem.IsComplete
        //};

        [HttpGet]
        public string MisFacturas(int id_usuario)
        {
            //id_usuario = Convert.ToInt16(id_usuario);
            using (var context = new as2oasis())
            {

                var mis_facturas =
                     context.picking
                    .Where(x =>
                    x.id_usuario==id_usuario)
                    .GroupBy(x=> new
                    {
                    x.id_factura,x.secuencial,x.empresa
                    })
                    .Select(x => new
                    {
                        id_factura= x.Key.id_factura,
                        secuencial=x.Key.secuencial,
                        empresa=x.Key.empresa,
                        estado = x.Max(y=>y.estado)
                    })
                    .ToList();                             

                var resultado_json = JsonConvert.SerializeObject(mis_facturas, Formatting.Indented);

                return resultado_json;
            }
        }

        [HttpPost]
        public JsonResult ReportePick()
        {
            using (var context = new as2oasis())
            {

                var mis_facturas =
                    context.picking
                    .Where(x => x.estado != 0)
                    .ToList()
                    .Select(x=> new {
                        x.empresa,
                        numero = x.id_factura ,
                        fecha_hora = x.fecha_hora.Value.ToString(),
                        //fecha_inicio_pick = x.fecha_inicio_pick.Value.ToString(),
                        //fecha_inicio_desp = x.fecha_inicio_desp?.ToString(),
                        //fecha_fin_pick = x.fecha_fin_pick?.ToString(),
                        //fecha_fin_desp = x.fecha_fin_desp?.ToString(),
                        fecha_guia_troquelada = "",
                        Diferencia = "",
                    });

                var resultado_json = new JsonResult { Data = mis_facturas };

                return resultado_json;
            }


            //var oa = new as2oasis();
            //var resultado = oa.PickingGuias
                //.OrderByDescending(x => x.fecha_inicio_pick)
                //.ToList()
                //.Select(x => new
                //{
                //    x.empresa,
                //    x.numero,
                //    fecha_inicio_pick = x.fecha_inicio_pick.Value.ToString(),
                //    fecha_inicio_desp = x.fecha_inicio_desp?.ToString(),
                //    fecha_fin_pick = x.fecha_fin_pick?.ToString(),
                //    fecha_fin_desp = x.fecha_fin_desp?.ToString(),
                //    fecha_guia_troquelada = x.fecha_guia_troquelada?.ToString(),
                //    x.Diferencia,
                //})
                //;
            //return new JsonResult { Data = resultado };
        }

        [HttpPost]
        public HttpStatusCode GrabarDatos(DatoPicking DPicking)
        {
            using (as2oasis oasis = new as2oasis())
            {
                var detalle_pick = new picking(); 
                detalle_pick.estado = DPicking.codigo_estado + 1;
                detalle_pick.fecha_hora = DateTime.Now;
                detalle_pick.id_factura = DPicking.id_factura;
                detalle_pick.id_usuario = DPicking.id_usuario;
                detalle_pick.empresa = DPicking.empresa;
                detalle_pick.secuencial = DPicking.secuencial;

                //var registro = oasis.picking.Where(x => x.id_factura == DPicking.id_factura).FirstOrDefault();

                oasis.picking.Add(detalle_pick);
                oasis.SaveChanges();

                return HttpStatusCode.OK;
            }

        }

        [HttpGet]
        public ActionResult DescargarAPP()
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"C:\\Mobile\\APP.apk");
            string fileName = "PickingHorus.apk";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }


        [HttpGet]
        public string LecturaPicking(string ClaveAcceso)
        {
            ClaveAcceso = ClaveAcceso.Substring(1, ClaveAcceso.Length - 1);
            using (var context = new as2oasis())
            {
                var id_factura =
                    context.DVP
                    .Where(x => x.clave_acceso.Contains(ClaveAcceso))
                    .GroupBy(x => new {
                        x.id_factura_cliente,
                        x.Secuencial_documento,
                        x.Cliente,
                        x.Fecha_factura,
                        x.Empresa,
                        x.clave_acceso
                    })
                    .Select(x => new {
                        x.Key.Cliente,
                        x.Key.id_factura_cliente,
                        x.Key.Secuencial_documento,
                        x.Key.Fecha_factura,
                        x.Key.Empresa,
                        x.Key.clave_acceso
                    })
                    .FirstOrDefault();

                var detalle_factura =
                    context.DVP
                    .Where(x => x.clave_acceso == ClaveAcceso)
                    .Select(x => new { x.Producto, x.Código_producto, x.Cantidad })
                    .GroupBy(x => new { x.Producto, x.Código_producto })
                    .ToList()
                    .Select(x => new { x.Key.Producto, CodigoProducto = Convert.ToInt16(x.Key.Código_producto), Cantidad = x.Sum(y => y.Cantidad) })
                    ;

                var datos_picking =
                    context.picking
                    .Where(x => x.id_factura == id_factura.id_factura_cliente)
                    .Select(x => new {
                        x.estado,
                        x.fecha_hora
                    })
                    .ToList();


                var resultado = new
                {
                    id_factura.Empresa,
                    IdFactura = id_factura.id_factura_cliente,
                    Secuencial = id_factura.Secuencial_documento,
                    CodigoEstado = datos_picking.Count == 0 ? 0 : 
                        datos_picking.OrderByDescending(z=>z.estado).First().estado,
                    id_factura.Cliente,
                    Fecha_factura = id_factura.Fecha_factura.Value.ToShortDateString(),
                    FechaUltimoEstado = datos_picking.Count == 0 ? "" :
                        datos_picking.OrderByDescending(z => z.estado).First().fecha_hora.Value.ToString(),
                    id_factura.clave_acceso,
                    DetalleFactura = detalle_factura
                };

                var resultado_json = JsonConvert.SerializeObject(resultado, Formatting.Indented);

                return resultado_json;
            }
        }

        //GET: Picking
        public ActionResult Index()
        {

            return View();
        }
    }
}