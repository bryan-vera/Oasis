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
            as2oasis oasis = new as2oasis();
            return View(oasis.productos.ToList());
        }

        // GET: Gastos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Gastos/Create
        public ActionResult Create()
        {
            //as2oasis oasis = new as2oasis();
            productos productos = new productos();
            return View(productos);
        }

        public JsonResult ObtenerProductos(string textoBusqueda)
        {
            as2oasis oasis = new as2oasis();
            oasis.Configuration.LazyLoadingEnabled = false;
            var productos=oasis.productos.Where(x=>x.descripcion.Contains(textoBusqueda)).ToList();
            return Json(productos, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(productos productoModel)
        {
            using(as2oasis oasis = new as2oasis())
            {
                if(oasis.productos.Any(x => x.descripcion == productoModel.descripcion))
                {
                    ViewBag.ProductoDuplicado = true;
                    return View("Create", productoModel);
                }
                oasis.productos.Add(productoModel);
                oasis.SaveChanges();
            }
            ModelState.Clear();
            ViewBag.SuccessMessage = "Se ha registrado el producto";
            return View("Create",new productos());
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
                using(var db = new as2oasis())
                {
                    //invt_productos_gastos productos = db.invt_productos_gastos.Where(z => z.id_producto == id).FirstOrDefault();
                    productos productos_2 = db.productos.Find(id);
                    ViewBag.categoria = productos_2.categoria;
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
        public ActionResult Edit(int id, productos producto)
        {
            try
            {
                using (var db = new as2oasis())
                {
                    //invt_productos_gastos productos = db.invt_productos_gastos.Where(z => z.id_producto == id).FirstOrDefault();
                    productos productos_2 = db.productos.Find(id);
                    productos_2.descripcion = producto.descripcion.Trim();
                    productos_2.categoria = producto.categoria.Trim();
                    productos_2.um = producto.um.Trim();
                    productos_2.valor_unitario = producto.valor_unitario;
                    productos_2.iva = producto.iva;
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
