using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oasis.Models;
using PagedList;

namespace Oasis.Controllers.Proveedores
{
    public class ProveedorController : Controller
    {
        // GET: Proveedor
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "nombre_comercial" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            empresa emp = new empresa();
            AS2Context as2 = new AS2Context();
            var proveedores = 
                as2.empresa
                .AsEnumerable()
                .Where(x => x.indicador_proveedor == true)
                .GroupBy(x => new { x.identificacion, x.activo, x.email1, x.nombre_comercial, x.indicador_proveedor })
                .Select(x => new empresa { 
                    identificacion = x.Key.identificacion, 
                    activo=x.Key.activo, 
                    email1=x.Key.email1, 
                    nombre_comercial=x.Key.nombre_comercial, 
                    indicador_proveedor=x.Key.indicador_proveedor })
                ;

            if (!String.IsNullOrEmpty(searchString))
            {
                proveedores = proveedores.Where(s => s.nombre_comercial.Contains(searchString)
                                       || s.email1.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "nombre_comercial":
                    proveedores = proveedores.OrderByDescending(s => s.nombre_comercial);
                    break;                
                default:
                    proveedores = proveedores.OrderBy(s => s.nombre_comercial);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(proveedores.ToPagedList(pageNumber,pageSize ));
        }

        // GET: Proveedor/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //// GET: Proveedor/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Proveedor/Create
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

        //// GET: Proveedor/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        // POST: Proveedor/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: Proveedor/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Proveedor/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
