using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Oasis.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

        public void ImprimirOrdenProduccion(string lote)
        {
            var context = new as2oasis();
            var cabecera_op =
                context.Orden_Produccion_Cabecera
                    .Where(x => x.lote == lote)
                    .ToList();
             

            using (MemoryStream myMemoryStream = new MemoryStream())
            {
                Image LABO_LOGO = Image.GetInstance(Properties.Resources.LABOV, BaseColor.WHITE);
                LABO_LOGO.ScaleAbsolute(50f, 50f);

                Reporte R = new Reporte();
                R.Empresa = "LABOV";
                R.MemoryStream = myMemoryStream;
                var doc = R.CrearDocA4();
                var pdf = R.CrearPDF();
                doc.Open();

                Font _standardFont = FontFactory.GetFont("SEGOE UI", 15, Font.BOLD, BaseColor.BLACK);
                Font subtitulo = FontFactory.GetFont("SEGOE UI", 7, Font.BOLD, BaseColor.BLACK);
                Font encabezado_tabla = FontFactory.GetFont("SEGOE UI", 8,Font.BOLD, BaseColor.BLACK);
                Font detalle = FontFactory.GetFont("SEGOE UI", 7, Font.NORMAL, BaseColor.BLACK);

                Font font = R.Fuente(_standardFont);
                //Fuente para encabezados
                Font _EncstandardFont = FontFactory.GetFont("SEGOE UI", 6);
                Font fontEnc = R.Fuente(_EncstandardFont);

                PdfPTable encabezado = new PdfPTable(6)
                {
                    LockedWidth = true,
                    TotalWidth = 500f,
                    SpacingBefore = 5f
                };

                encabezado.SetWidths(new float[] { 66f, 40f, 66f, 150f, 66f, 80f });

                PdfPTable table1 = new PdfPTable(3)
                {
                    LockedWidth = true,
                    TotalWidth = 500f,
                    SpacingBefore = 5f
                };


                #region cabecera
                Chunk _Titulo = new Chunk(" ORDEN DE PRODUCCION \n LOTE: " + cabecera_op.First().lote + " \n "
                   , _standardFont);
                Paragraph __Titulo = new Paragraph(_Titulo);
                __Titulo.Alignment = Element.ALIGN_CENTER;

                table1.SetWidths(new float[] { 100f, 200f, 100f });

                PdfPCell cell1 = new PdfPCell();
                cell1 = new PdfPCell(LABO_LOGO);

                cell1.Padding = 0;
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                table1.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase(_Titulo));
                cell1.Padding = 0;
                cell1.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                cell1.Border = PdfPCell.NO_BORDER;

                table1.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase("USUARIO: \n" + System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString() +
                    " \n "));
                cell1.Padding = 0;
                cell1.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                cell1.Border = PdfPCell.NO_BORDER;

                table1.AddCell(cell1);

                doc.Add(table1);

                doc.Add(Chunk.NEWLINE);

                cell1 = new PdfPCell(new Phrase("Cod. PT.:", subtitulo));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase(Convert.ToInt32(cabecera_op.First().codigo).ToString(), detalle));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase("PT.:", subtitulo));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase(cabecera_op.First().nombre, detalle));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase("O.P.:", subtitulo));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase(cabecera_op.First().OP, detalle));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase("Und. Elaboradas.:", subtitulo));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase(String.Format("{0:n0}", cabecera_op.First().cantidad), detalle));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase("Costo unit.:", subtitulo));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase("0", detalle));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase("U.M.:", subtitulo));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase("", detalle));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase("Cajas elab.:", subtitulo));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase(String.Format("{0:n0}", cabecera_op.First().cantidad_fabricada), detalle));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase("Costo unit. (cajas):", subtitulo));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase("", detalle));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase("Costo total:", subtitulo));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase("", detalle));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase("Und. a elaborar:", subtitulo));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase(String.Format("{0:n0}", cabecera_op.First().cantidad ), detalle));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase("Rendimiento:", subtitulo));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase((cabecera_op.First().rendimiento*100).ToString() + "%", detalle));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase("Fecha inicio O.P.:", subtitulo));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase(cabecera_op.First().fecha.ToShortDateString(), detalle));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase(""));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase(""));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase(""));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase(""));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase("Fecha cierre O.P.:", subtitulo));
                cell1.Border = PdfPCell.NO_BORDER;
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                encabezado.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase(cabecera_op.First().fecha_cierre.HasValue? 
                    cabecera_op.First().fecha_cierre.Value.ToShortDateString():
                    "", detalle));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell1.Border = PdfPCell.NO_BORDER;
                encabezado.AddCell(cell1);

                doc.Add(encabezado);
                #endregion

                doc.Close();
                var pdf_generado = R.GenerarPDF();

                Response.Clear();
                Response.ClearHeaders();
                Response.Write(Convert.ToBase64String(pdf_generado));
                Response.Flush();
                Response.End();

                //return File(Response, "application/pdf", "DownloadName.pdf");
            }

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
                    context.Orden_Produccion_Cabecera
                    .Where(x => x.fecha >= fecha_desde_ &&
                                x.fecha <= fecha_hasta_ 
                                //&&
                                //x.cod__producto_material== "PRODUCTOS TERMINADOS"
                            )
                    .ToList()
                    .Select(x => new
                    {
                        x.lote,
                        fecha = x.fecha.ToShortDateString(),
                        x.codigo,
                        x.nombre,
                        x.cantidad,
                        x.cantidad_fabricada, 
                        rendimiento = Math.Round((double)((x.rendimiento) * 100), 2) + "%",
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
