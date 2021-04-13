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
using System.Data.Odbc;
using clsConectaMBA;
using System.Data;

namespace Oasis.Controllers.Bodega
{
    public class GuiasRemisionController : Controller
    {
        // GET: GuiasRemision
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ConexionMba cs = new ConexionMba();
            
            string cadena = " SELECT FP.EMPRESA `EMPRESA `, CASE FP.ORIGEN  WHEN 'PRI' THEN 'COSTA'" +
                " WHEN 'LA2' THEN 'SIERRA' WHEN 'LE2' THEN 'SIERRA'  " +
                " WHEN 'DA2' THEN 'SIERRA'  WHEN 'FA2' THEN 'SIERRA'  WHEN 'AN2' THEN 'SIERRA' " +
                " WHEN 'ME2' THEN 'SIERRA' WHEN 'LA3' THEN 'AUSTRO' WHEN 'LE3' THEN 'AUSTRO'  " +
                " WHEN 'DA3' THEN 'AUSTRO' WHEN 'ME3' THEN 'AUSTRO' WHEN 'FA3' THEN 'AUSTRO' " +
                " WHEN 'AN3' THEN 'AUSTRO' END AS REGION, " +
                " FIP.ESTADO `PROVINCIA`, FIP.CIUDAD_PRINCIPAL `CIUDAD`, FIP.NOMBRE_SECTOR `SECTOR`, " +
                " DATE_TO_CHAR(FP.fecha_factura, 'dd[/]mm[/]yyyy') AS `FECHA FACT`, " +
                " CAST(FP.VALOR_FACTURA AS float) `VALOR TOTAL`, FP.codigo_factura `FACTURA`, " +
                " DATE_TO_CHAR(FP.FECHA_EMBARQUE, 'dd[/]mm[/]yyyy') AS `DESPACHO`, " +
                " FIP.IDENTIFICACION_FISCAL `IDENTIFICACION`, FIP.NOMBRE_CLIENTE `CLIENTE`, FIP.DIRECCION_PRINCIPAL_1 `DIRECCION`," +
                " FIP.TELEFONO,FIP.TELEFONO_2,VEN.DESCRIPTION_SPN `VENDEDOR`, FP.VALOR_FACTURA `VALOR BRUTO`, FP.TOTAL_DEVOLUCION `NOTA DE CREDITO`, " +
                " (FP.VALOR_FACTURA - FP.TOTAL_DEVOLUCION) `VALOR NETO`" +
                " FROM " +
                " CLNT_FACTURA_PRINCIPAL FP " +
                " INNER JOIN " +
                " (CLNT_FICHA_PRINCIPAL FIP INNER JOIN SIST_LISTA_1 VEN ON FIP.SALESMAN = VEN.CODE) " +
                " ON FP.CODIGO_CLIENTE_EMPRESA = FIP.CODIGO_CLIENTE_EMPRESA " +
                " WHERE ANULADA = CAST('FALSE' AS BOOLEAN) AND CONFIRMADO = CAST('TRUE' AS BOOLEAN) " +
                " AND FECHA_FACTURA >= '2021/04/01'  AND  FIP.CLIENT_TYPE IN ('DISTR', 'FARMA')" +
                " AND VEN.GROUP_CATEGORY = 'SELLm' AND VEN.CORP=FIP.EMPRESA " +
                " AND  FP.EMPRESA = 'LABOV'";

            OdbcCommand DbCommand = new OdbcCommand(cadena, cs.getConexion());
            OdbcDataAdapter adp1 = new OdbcDataAdapter();
            DataTable dt = new DataTable();
            adp1.SelectCommand = DbCommand;
            adp1.Fill(dt);

            IEnumerable<GuiaUrbano> guia_labovida =
                dt.AsEnumerable()
                .Select(x => new GuiaUrbano
                {
                    id_organizacion=69,
                    empresa = "LABORATORIOS LABOVIDA S.A.",
                    provincia = x.Field<string>(2),
                    ciudad = x.Field<string>(3),
                    parroquia = x.Field<string>(4),
                    fecha_factura = DateTime.Parse(x.Field<string>(5)),
                    valor_factura = (decimal)x.Field<Double>(6),
                    fecha_guia = DateTime.Parse(x.Field<string>(5)),
                    descripcion = "NA",
                    email = "NA",
                    secuencial_guia ="NA",
                    nombre_comercial = x.Field<string>(10),
                    vendedor ="NA",
                    telefono1 = x.Field<string>(12),
                    telefono2 = x.Field<string>(13),
                    direccion = x.Field<string>(11),
                    secuencial_factura = x.Field<string>(7),
                    id_guia_remision = (Int16.Parse(
                        x.Field<string>(7).Split('-')[1].Substring(3, 10).Substring(7, 3) + 
                        x.Field<string>(7).Split('-')[1].Substring(3, 10).Substring(0, 1)) 
                        )
                *10,
                    identificacion = x.Field<string>(9),
                })
                ;


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


            var guias = (from s in db.GuiaUrbano
                           select s).AsEnumerable();

            guias = guias.Union(guia_labovida);

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
            ConexionMba cs = new ConexionMba();

            string cadena = " SELECT FP.EMPRESA `EMPRESA `, CASE FP.ORIGEN  WHEN 'PRI' THEN 'COSTA'" +
                " WHEN 'LA2' THEN 'SIERRA' WHEN 'LE2' THEN 'SIERRA'  " +
                " WHEN 'DA2' THEN 'SIERRA'  WHEN 'FA2' THEN 'SIERRA'  WHEN 'AN2' THEN 'SIERRA' " +
                " WHEN 'ME2' THEN 'SIERRA' WHEN 'LA3' THEN 'AUSTRO' WHEN 'LE3' THEN 'AUSTRO'  " +
                " WHEN 'DA3' THEN 'AUSTRO' WHEN 'ME3' THEN 'AUSTRO' WHEN 'FA3' THEN 'AUSTRO' " +
                " WHEN 'AN3' THEN 'AUSTRO' END AS REGION, " +
                " FIP.ESTADO `PROVINCIA`, FIP.CIUDAD_PRINCIPAL `CIUDAD`, FIP.NOMBRE_SECTOR `SECTOR`, " +
                " DATE_TO_CHAR(FP.fecha_factura, 'dd[/]mm[/]yyyy') AS `FECHA FACT`, " +
                " CAST(FP.VALOR_FACTURA AS float) `VALOR TOTAL`, FP.codigo_factura `FACTURA`, " +
                " DATE_TO_CHAR(FP.FECHA_EMBARQUE, 'dd[/]mm[/]yyyy') AS `DESPACHO`, " +
                " FIP.IDENTIFICACION_FISCAL `IDENTIFICACION`, FIP.NOMBRE_CLIENTE `CLIENTE`, FIP.DIRECCION_PRINCIPAL_1 `DIRECCION`," +
                " FIP.TELEFONO,FIP.TELEFONO_2,VEN.DESCRIPTION_SPN `VENDEDOR`, FP.VALOR_FACTURA `VALOR BRUTO`, FP.TOTAL_DEVOLUCION `NOTA DE CREDITO`, " +
                " (FP.VALOR_FACTURA - FP.TOTAL_DEVOLUCION) `VALOR NETO`" +
                " FROM " +
                " CLNT_FACTURA_PRINCIPAL FP " +
                " INNER JOIN " +
                " (CLNT_FICHA_PRINCIPAL FIP INNER JOIN SIST_LISTA_1 VEN ON FIP.SALESMAN = VEN.CODE) " +
                " ON FP.CODIGO_CLIENTE_EMPRESA = FIP.CODIGO_CLIENTE_EMPRESA " +
                " WHERE ANULADA = CAST('FALSE' AS BOOLEAN) AND CONFIRMADO = CAST('TRUE' AS BOOLEAN) " +
                " AND FECHA_FACTURA >= '2021/04/01'  AND  FIP.CLIENT_TYPE IN ('DISTR', 'FARMA')" +
                " AND VEN.GROUP_CATEGORY = 'SELLm' AND VEN.CORP=FIP.EMPRESA " +
                " AND  FP.EMPRESA = 'LABOV'";

            OdbcCommand DbCommand = new OdbcCommand(cadena, cs.getConexion());
            OdbcDataAdapter adp1 = new OdbcDataAdapter();
            DataTable dt = new DataTable();
            adp1.SelectCommand = DbCommand;
            adp1.Fill(dt);

            IEnumerable<GuiaUrbano> guia_labovida =
                dt.AsEnumerable()
                .Select(x => new GuiaUrbano
                {
                    id_organizacion = 69,
                    empresa = "LABORATORIOS LABOVIDA S.A.",
                    provincia = x.Field<string>(2),
                    ciudad = x.Field<string>(3),
                    parroquia = x.Field<string>(4),
                    fecha_factura = DateTime.Parse(x.Field<string>(5)),
                    valor_factura = (decimal)x.Field<Double>(6),
                    fecha_guia = DateTime.Parse(x.Field<string>(5)),
                    descripcion = "NA",
                    email = "NA",
                    secuencial_guia = "NA",
                    nombre_comercial = x.Field<string>(10),
                    vendedor = "NA",
                    telefono1 = x.Field<string>(12),
                    telefono2 = x.Field<string>(13),
                    direccion = x.Field<string>(11),
                    secuencial_factura = x.Field<string>(7),
                    id_guia_remision = (Int16.Parse(
                        x.Field<string>(7).Split('-')[1].Substring(3, 10).Substring(7, 3) + 
                        x.Field<string>(7).Split('-')[1].Substring(3, 10).Substring(0, 1)) 
                        )*10,
                    identificacion = x.Field<string>(9),
                });
            as2oasis db = new as2oasis();
            var guias = db.GuiaUrbano.AsEnumerable()
                    .Union(guia_labovida)
                    .Where(x => x.id_guia_remision == id).FirstOrDefault();

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
