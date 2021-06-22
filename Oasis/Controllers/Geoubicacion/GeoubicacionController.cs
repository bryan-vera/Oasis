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