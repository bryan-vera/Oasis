using Newtonsoft.Json;
using Oasis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Oasis.Controllers.Produccion
{
    public class ProduccionController : Controller
    {
        // GET: Produccion
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObtenerOP(
            string fecha_desde,
            string fecha_hasta)
        {
            DateTime fecha_desde_ = DateTime.Parse(fecha_desde);
            DateTime fecha_hasta_ = DateTime.Parse(fecha_hasta);
 
            using (var context = new as2oasis())
            {
                var op =
                    context.Orden_Produccion
                    .Where(x => x.Fecha_creacion_OP >= fecha_desde_ &&
                                x.Fecha_creacion_OP <= fecha_hasta_
                            )
                    .GroupBy(x => new
                    {
                        x.Fecha_creacion_OP,
                        x.descripcion_op,
                        x.OP,
                        x.planta,
                        x.responsable,
                        x.codigo_producto,
                        x.descripcion_producto,
                        x.cantidad,
                        x.cantidad_fabricada,
                        x.codigo_lote
                    })
                    .ToList()
                    .Select(x => new
                    {
                        x.Key.OP,
                        x.Key.codigo_producto,
                        x.Key.descripcion_producto,
                        x.Key.cantidad,
                        x.Key.cantidad_fabricada,
                        rendimiento = Math.Round((double)((x.Key.cantidad_fabricada / x.Key.cantidad) * 100), 2) + "%",
                        x.Key.codigo_lote,
                        //Fecha_creacion_OP =x.Key.Fecha_creacion_OP.Value.ToShortDateString(),
                        //x.Key.descripcion_op,
                        x.Key.planta,
 
                    });

                return Json(op, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Produccion/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Produccion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Produccion/Create
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

        // GET: Produccion/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Produccion/Edit/5
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

        // GET: Produccion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Produccion/Delete/5
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
