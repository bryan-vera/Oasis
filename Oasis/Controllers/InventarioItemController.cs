using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oasis.Models;
using Oasis.ViewModel;

namespace Oasis.Controllers
{
    public class InventarioItemController : Controller
    {
        private readonly invt_categoria categoriaRepository;
        private readonly invt_productos_gastos productoRepository;

        //public invt_productos_gastos(invt_categoria categoriaRepository, invt_productos_gastos productoRepository)
        //{
        //    // Check that manufacturerRepository and inventoryItem are not null
        //    this.categoriaRepository = categoriaRepository;
        //    this.productoRepository = productoRepository;
        //}

        // GET: InventarioItem
        public ActionResult Index()
        {
            return View();
        }

        // GET: InventarioItem/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: InventarioItem/Create
        public ActionResult Create()
        {
            //InventarioItemViewModel viewModel = new InventarioItemViewModel
            //{
            //    Categoria = categoriaRepository.GetAll()
            //};

            return View();

            //return View();
        }

        // POST: InventarioItem/Create
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

        // GET: InventarioItem/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: InventarioItem/Edit/5
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

        // GET: InventarioItem/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: InventarioItem/Delete/5
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
