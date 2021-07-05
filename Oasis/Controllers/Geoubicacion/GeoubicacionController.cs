using Oasis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Oasis.Controllers.Geoubicacion
{
    public class GeoubicacionController : Controller
    {
        public class DatosUbicacion
        {
            public bool indicador_mock { get; set; }
            public decimal latitud { get; set; }
            public decimal longitud { get; set; }
            public DateTime fecha_hora { get; set; }
            public int id_usuario { get; set; }
        }

        // GET: Geoubicacion
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ObtenerDatosGeoPorVisitador(string id_usuario,
            string fecha_desde, 
            string fecha_hasta)
        {
            DateTime fecha_desde_ = DateTime.Parse(fecha_desde);
            DateTime fecha_hasta_ = DateTime.Parse(fecha_hasta);
            var id_usuario_ = Convert.ToInt16(id_usuario);
            var oasis = new as2oasis();
            var id_visitador = oasis.usuario.Where(x => x.id_vendedor == id_usuario_).Select(x => x.id_usuario).FirstOrDefault();
            var datos_geo = oasis.geoubicacion.Where(
                x => x.id_usuario == id_usuario_
                &&
                x.fecha_hora >= fecha_desde_ &&
                x.fecha_hora <= fecha_hasta_
                )
                .ToList()
                .Select(x => new
                {
                    x.latitud,
                    x.longitud,
                    fecha_hora = x.fecha_hora.ToLongDateString()
                })
                .ToList();
            return Json(datos_geo,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public HttpStatusCode GrabarUbicacion(DatosUbicacion datos)
        {
            using (as2oasis oasis = new as2oasis())
            {
                //var fecha_actual = new DateTime();
                var ubicacion = new geoubicacion();
                ubicacion.id_usuario =datos.id_usuario;
                ubicacion.latitud = datos.latitud;
                ubicacion.longitud = datos.longitud;
                ubicacion.indicador_mock = datos.indicador_mock;
                ubicacion.fecha_hora = DateTime.Now;

                oasis.geoubicacion.Add(ubicacion);
                oasis.SaveChanges();

                return HttpStatusCode.OK;
            }

        }
    }
}