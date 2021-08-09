using Microsoft.Ajax.Utilities;
using Oasis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oasis.Controllers.Credito;
using Newtonsoft.Json;

namespace Oasis.Controllers
{
    public class VisitadorController : Controller
    {
        [HttpGet]
        public JsonResult PedidosVisitador(string id_visitador,string busqueda)
        {
            var codigo_visitador = ObtenerID(id_visitador);
            var oasis = new as2oasis();
            //var ped = oasis.Pedidos
            //    .Where(x => x.id_vendedor == codigo_visitador
            //    && ( x.secuencial_factura.Contains(busqueda) 
            //    || x.nombre_comercial.Contains(busqueda))
            //    //(
            //    //x.nombre_comercial.ToLower().Contains(busqueda)
            //    //|| x.identificacion.Contains(busqueda)
            //    //|| x.secuencial_factura.Contains(busqueda)
            //    //|| 
            //    //x.numero.Contains(busqueda))
            //    )
            //    //.OrderByDescending(x => x.fecha_creacion_pedido)
            //    //.Take(10)
            //    .ToList();

            var pedidos = oasis.Pedidos
                .Where(x => x.id_vendedor == codigo_visitador
                && (x.nombre_comercial.ToLower().Contains(busqueda) 
                //|| x.identificacion.Contains(busqueda) 
                || x.secuencial_factura.Contains(busqueda)
                //|| x.numero.Contains(busqueda) 
                ))
                .OrderByDescending(x => x.fecha_creacion_pedido)
                .Take(10)
                .ToList()
                .Select(x => new {
                    x.numero,
                    x.nombre_comercial,
                    x.estado,
                    x.estado_numerico,
                    estado_picking = x.estado_picking == null ? "" : x.estado.ToString(),
                    fecha_creacion_pedido = x.fecha_creacion_pedido.Value.ToShortDateString(),
                    fecha_autorizacion_pedido = x.fecha_autorizacion_pedido == null ? "" : x.fecha_autorizacion_pedido.Value.ToShortDateString(),
                    fecha_factura = x.fecha_factura == null ? "" : x.fecha_factura.Value.ToShortDateString(),
                    fecha_guia = x.fecha_guia == null ? "" : x.fecha_guia.Value.ToShortDateString(),
                    fecha_guia_troquelada = x.fecha_guia_troquelada == null ? "" : x.fecha_guia_troquelada.Value.ToShortDateString(),
                    secuencial_factura = x.secuencial_factura == null ? "" : x.secuencial_factura,
                    x.valor_pedido
                });
            return Json(pedidos, JsonRequestBehavior.AllowGet);
        }

        public int ObtenerID(string id_usuario)
        {
            var oasis = new as2oasis();
            var codigo_usuario = Convert.ToInt16(id_usuario);
            var user_vendedor = oasis.usuario
                .Where(x => x.id_usuario == codigo_usuario)
                .Select(x => x.username.ToLower()).FirstOrDefault();
            var datos_vendedor = oasis.Vendedores
                .Where(x => x.username.ToLower() == user_vendedor)
                .FirstOrDefault();

            return datos_vendedor.id_vendedor;
        } 

        [HttpGet]
        public JsonResult UltimosPedidos(string id_visitador, int pagina=0)
        {
            var oasis = new as2oasis();
            var codigo_visitador = ObtenerID(id_visitador);
            var pedidos = oasis.Pedidos
                .Where(x => x.id_vendedor == codigo_visitador)
                .OrderByDescending(x => x.fecha_creacion_pedido)
                .Skip(10*pagina)
                .Take(10)
                .ToList()
                .Select(x => new {
                    x.numero,
                    x.nombre_comercial,
                    x.estado,
                    x.estado_numerico,
                    estado_picking = x.estado_picking==null?"":x.estado.ToString(),
                    fecha_creacion_pedido = x.fecha_creacion_pedido.Value.ToShortDateString(),
                    fecha_autorizacion_pedido = x.fecha_autorizacion_pedido==null?"":x.fecha_autorizacion_pedido.Value.ToShortDateString(),
                    fecha_factura = x.fecha_factura==null?"":x.fecha_factura.Value.ToShortDateString(),
                    fecha_guia = x.fecha_guia==null?"":x.fecha_guia.Value.ToShortDateString(),
                    fecha_guia_troquelada = x.fecha_guia_troquelada==null?"":x.fecha_guia_troquelada.Value.ToShortDateString(),
                    secuencial_factura = x.secuencial_factura==null?"":x.secuencial_factura,
                    x.valor_pedido                  
                });

            return Json(pedidos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult UltimasVentas(string id_visitador)
        {
            var codigo_visitador = ObtenerID(id_visitador);
            var oasis = new as2oasis();
            var ultVentas = oasis.Ventas_Consolidado
                .Where(x => x.id_vendedor == codigo_visitador)
                .OrderByDescending(x=>x.fecha_factura)
                .Take(10)
                .ToList()
                .Select(x=>new { 
                cliente=x.nombre_comercial,
                x.secuencial_factura,
                fecha_factura=x.fecha_factura.ToShortDateString(),
                pedido=x.numero});
            return Json(ultVentas,JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult UltimosCobros(string id_visitador)
        {
            var codigo_visitador = ObtenerID(id_visitador);
            var oasis = new as2oasis();
            var ultCobros = oasis.Cobros_Consolidado
                .Where(x => x.id_vendedor == codigo_visitador)
                .OrderByDescending(x => x.fecha_creacion)
                .Take(10)
                .ToList()
                .Select(x => new {
                    cliente = x.nombre_comercial,
                    codigo_cobro=x.codigo_cobro, 
                    fecha_aplicacion = x.fecha_aplicacion.Value.ToShortDateString(),
                    fecha_creacion = x.fecha_creacion.Value.ToShortDateString(),
                    secuencial_factura = x.numero
                });
            return Json(ultCobros, JsonRequestBehavior.AllowGet);
        }

        public class Fecha_Sugerida
        {
            public string fecha_desde;
            public string fecha_hasta;
        }

        [HttpGet]
        public JsonResult MetaPresupuesto(int id_visitador)
        {
            var oasis = new as2oasis();
            var credito = new CreditoController();
            var user_vendedor = oasis.usuario
                .Where(x => x.id_usuario == id_visitador)
                .Select(x => x.username.ToLower()).FirstOrDefault();
            var datos_vendedor = oasis.Vendedores.Where(x => x.username.ToLower() == user_vendedor).FirstOrDefault();
            var empresa = datos_vendedor.empresa;
            var sucursal = datos_vendedor.sucursal;
            var JSONFechasSugeridas = credito.SugerirFechas(empresa,sucursal).Data.ToString();
            var fecha_sugerida = JsonConvert.DeserializeObject<Fecha_Sugerida>(JSONFechasSugeridas);
            var fecha_desde = DateTime.Parse(fecha_sugerida.fecha_desde);
            var fecha_hasta = DateTime.Parse(fecha_sugerida.fecha_hasta);

            var presupuesto = oasis.Presupuesto(empresa, sucursal, fecha_desde, fecha_hasta, "FARMACIA");
            var presupuesto_individual = presupuesto.Where(x => x.id_vendedor == datos_vendedor.id_vendedor).FirstOrDefault();
            var datos_meta = new { fecha_desde = fecha_sugerida.fecha_desde, fecha_hasta = fecha_sugerida.fecha_hasta, presupuesto_individual };


            return Json(datos_meta, JsonRequestBehavior.AllowGet);
        }
    }
}
