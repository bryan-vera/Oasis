using Oasis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Oasis.Controllers.Proveedores
{
    public class OrdenesCompraController : Controller
    {

        // GET: OrdenesCompra
        public ActionResult Index()
        {
            as2oasis oc = new as2oasis();            
            return View(oc.ordenes_compra);
        }

        // GET: OrdenesCompra/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: OrdenesCompra/Create
        public ActionResult Create()
        {

            prov_oc_principal oc_principal = new prov_oc_principal();
            List<SelectListItem> lst = new List<SelectListItem>();
            AS2Context as2 = new AS2Context();

            lst.Add(new SelectListItem() { Text = "LABOVIDA", Value = "LABOV" });
            lst.Add(new SelectListItem() { Text = "LEBENFARMA", Value = "LEBEN" });
            lst.Add(new SelectListItem() { Text = "FARMALIGHT", Value = "FARMA" });
            lst.Add(new SelectListItem() { Text = "DANIVET", Value = "DANIV" });
            lst.Add(new SelectListItem() { Text = "ANYUPA", Value = "ANYUP" });
            lst.Add(new SelectListItem() { Text = "MEDITOTAL", Value = "MEDIT" });

            var proveedores =
               as2.empresa
               .AsEnumerable()
               .Where(x => x.indicador_proveedor == true && x.activo == true  )
               .GroupBy(x => new { x.identificacion, x.email1, x.nombre_comercial })
               .Select(x => new empresa
               {
                   identificacion = x.Key.identificacion,
                   nombre_comercial = x.Key.nombre_comercial,
               })
               ;

            ViewBag.Opciones = lst;
            
            ViewBag.Proveedores = new SelectList(proveedores, "identificacion", "nombre_comercial");

            return View(oc_principal);
        }

        // POST: OrdenesCompra/Create
        [HttpPost]
        public ActionResult Create(prov_oc_principal oc_principalModel)
        {
            using (OASISContext oasis = new OASISContext())
            {
                if (oasis.prov_oc_principal.Any(x => x.id_oc_principal == oc_principalModel.id_oc_principal))
                {
                    ViewBag.ProductoDuplicado = true;
                    return View("Create", oc_principalModel);
                }
                oasis.prov_oc_principal.Add(oc_principalModel);
                oasis.SaveChanges();
            }
            ModelState.Clear();
            ViewBag.SuccessMessage = "Se ha registrado el producto";
            return View("Create", new prov_oc_principal());
            //invt_productos_gastos productos = new invt_productos_gastos();
            //return View(productos);
        }

        // GET: OrdenesCompra/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OrdenesCompra/Edit/5
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

        // GET: OrdenesCompra/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OrdenesCompra/Delete/5
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
