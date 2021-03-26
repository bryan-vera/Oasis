using iTextSharp.text;
using iTextSharp.text.pdf;
using Oasis.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;


namespace Oasis.Controllers.Bodega
{
    public class GuiasRemisionController : Controller
    {
        // GET: GuiasRemision
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            as2oasis db = new as2oasis();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.FacturaSortParm = String.IsNullOrEmpty(sortOrder) ? "factura_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;


            var guias = from s in db.GuiaUrbano
                           select s;


            if (!String.IsNullOrEmpty(searchString))
            {
                guias = guias.Where(s => s.secuencial_factura.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "factura_desc":
                    guias = guias.OrderByDescending(s => s.secuencial_factura);
                    break;
                case "Date":
                    guias = guias.OrderByDescending(s => s.fecha_factura);
                    break;
                case "date_desc":
                    guias = guias.OrderByDescending(s => s.fecha_factura);
                    break;
                default:
                    guias = guias.OrderByDescending(s => s.fecha_factura);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1); 
            return View(guias.ToPagedList(pageNumber,pageSize));
        }

        // GET: GuiasRemision/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: GuiasRemision/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GuiasRemision/Create
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


        // GET: Imprimir
        public void Imprimir(int id, float peso, float bultos)
        {
            as2oasis db = new as2oasis();
            var guias = db.GuiaUrbano.Where(x => x.id_guia_remision == id).FirstOrDefault();

            using (MemoryStream myMemoryStream = new MemoryStream())
            {
                Reporte R = new Reporte();
                R.Empresa = "LABOV";
                R.MemoryStream = myMemoryStream;
                var doc = R.CrearDocGuia();
                var pdf = R.CrearPDF();

                PdfPTable tabla_general = new PdfPTable(2)
                {
                    LockedWidth = true,
                    TotalWidth = 550f,
                    SpacingBefore = 4f
                };

                tabla_general.SetWidths(new float[] { 275f, 275f });

                //Inicia la apertura del documento y escritura
                doc.AddTitle("PDF");
                doc.Open();

                R.InsertarTexto(pdf, guias.nombre_comercial,380,355);
                R.InsertarTexto(pdf, guias.identificacion,380,340);
                R.InsertarTexto(pdf, guias.direccion, 380, 330);
                R.InsertarTexto(pdf, guias.ciudad.Length>26?guias.ciudad.Substring(0,26):guias.ciudad, 380, 305);
                R.InsertarTexto(pdf, guias.telefono1, 380, 290);
                R.InsertarTexto(pdf, bultos.ToString(), 400, 180);
                R.InsertarTexto(pdf, peso.ToString(), 490, 180);
                R.InsertarTexto(pdf, guias.secuencial_factura, 445, 160);
                R.InsertarTexto(pdf, ((DateTime)guias.fecha_factura).ToShortDateString(), 445, 145);


                doc.Close();
                var pdf_generado = R.GenerarPDF();

                Response.Clear();
                Response.ClearHeaders();
                //Response.AddHeader("Content-Type", "application/pdf");
                //Response.AddHeader("Content-Length", pdf_generado.Length.ToString());
                //Response.AddHeader("Content-Disposition", "inline; filename=file.pdf");
                //Response.BinaryWrite(Convert.ToBase64String(pdf_generado));
                //Response.Flush();
                //Response.End();

                Response.Write(Convert.ToBase64String(pdf_generado));
                Response.Flush();
                Response.End();


            }
        }


        // GET: GuiasRemision/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GuiasRemision/Edit/5
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

        // GET: GuiasRemision/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GuiasRemision/Delete/5
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
