using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oasis.Models;

namespace Oasis.Controllers
{
    public class GastosController : Controller
    {
        // GET: Gastos
        public ActionResult Index()
        {
            OASISContext oasis = new OASISContext();
            return View(oasis.invt_productos_gastos.ToList());
        }

        // GET: Gastos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Gastos/Create
        public ActionResult Create()
        {
            invt_productos_gastos productos = new invt_productos_gastos();
            return View(productos);
        }

        public JsonResult ObtenerProductos(string textoBusqueda)
        {
            OASISContext oasis = new OASISContext();
            oasis.Configuration.LazyLoadingEnabled = false;
            var productos= oasis.invt_productos_gastos.Where(x=>x.descripcion.Contains(textoBusqueda)).ToList();
            return Json(productos, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(invt_productos_gastos productoModel)
        {
            using(OASISContext oasis = new OASISContext())
            {
                if(oasis.invt_productos_gastos.Any(x => x.descripcion == productoModel.descripcion))
                {
                    ViewBag.ProductoDuplicado = true;
                    return View("Create", productoModel);
                }
                oasis.invt_productos_gastos.Add(productoModel);
                oasis.SaveChanges();
            }
            ModelState.Clear();
            ViewBag.SuccessMessage = "Se ha registrado el producto";
            return View("Create",new invt_productos_gastos());
        }

        // POST: Gastos/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: Gastos/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                using(var db = new OASISContext())
                {
                    //invt_productos_gastos productos = db.invt_productos_gastos.Where(z => z.id_producto == id).FirstOrDefault();
                    invt_productos_gastos productos_2 = db.invt_productos_gastos.Find(id);
                    ViewBag.categoria = productos_2.grupo_categoria;
                    return View(productos_2);
                }
            }
            catch (Exception)
            {

                throw;
            }
            //return View();
        }

        // POST: Gastos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, invt_productos_gastos producto)
        {
            try
            {
                using (var db = new OASISContext())
                {
                    //invt_productos_gastos productos = db.invt_productos_gastos.Where(z => z.id_producto == id).FirstOrDefault();
                    invt_productos_gastos productos_2 = db.invt_productos_gastos.Find(id);
                    productos_2.descripcion = producto.descripcion;
                    productos_2.grupo_categoria = producto.grupo_categoria;
                    productos_2.um = producto.um;
                    productos_2.valor_unitario = producto.valor_unitario;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {

                throw;
            }

            //try
            //{
            //    // TODO: Add update logic here

            //    return RedirectToAction("Index");
            //}
            //catch
            //{
            //    return View();
            //}
        }

        // GET: Gastos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Gastos/Delete/5
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
