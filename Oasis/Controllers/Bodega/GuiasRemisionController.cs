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
using Newtonsoft.Json;

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
                    empresa = "(MBA) LABORATORIOS LABOVIDA S.A. ",
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
                        x.Field<string>(7).Split('-')[1].Substring(3, 10).Substring(1, 9))),
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

        // GET: GuiasRemision/Details/5
        public ActionResult ReporteGuias()
        {
            ViewBag.Opciones = ListaEmpresas();
            ViewBag.Title = "Guías de despacho";
            return View();
        }

        public JsonResult ObtenerReporteGuia(
            string fecha_desde_factura,
            string fecha_hasta_factura,
            string fecha_desde_guia,
            string fecha_hasta_guia,
            bool indicador_factura,
            string empresa = ""
            )
        {
            DateTime fecha_desde;
            DateTime fecha_hasta;

            if (indicador_factura)
            {
                fecha_desde = DateTime.Parse(fecha_desde_factura);
                fecha_hasta = DateTime.Parse(fecha_hasta_factura);
            } else
            {
                fecha_desde = DateTime.Parse(fecha_desde_guia);
                fecha_hasta = DateTime.Parse(fecha_hasta_guia);
            }

          
            using (var context = new as2oasis())
            {
                IQueryable<Reporte_GuiasRemision> guias;

                if(indicador_factura) {
                    guias = context
                    .Reporte_GuiasRemision
                        .Where(x =>
                            x.fecha_factura >= fecha_desde &&
                            x.fecha_factura <= fecha_hasta);
                } else
                {
                    guias = context
                    .Reporte_GuiasRemision
                        .Where(x =>
                            x.fecha_guia >= fecha_desde &&
                            x.fecha_guia <= fecha_hasta);
                }

                if (!String.IsNullOrEmpty(empresa))
                    guias = guias.Where(x => x.empresa == empresa);

                var guias_json = guias.ToList()
                     .Select(x => new
                     {
                         x.empresa,
                         x.nombre_comercial,
                         x.secuencial_factura,
                         fecha_factura = x.fecha_factura.Value.ToString(),
                         x.secuencial_guia,
                         fecha_guia = x.fecha_guia == null ? "":x.fecha_guia.Value.ToString(),
                         x.ciudad,
                         peso = x.peso == null ? 0 : x.peso,
                         bultos = x.bultos==null?0:x.bultos,
                         serial_urbano = x.serial_urbano == null ? "" : x.serial_urbano,
                         nota_guia = x.nota_guia == null ? "" : x.nota_guia
                     }) ;

                var resultado = JsonConvert.SerializeObject(guias_json, Formatting.Indented);

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
        }


        public List<SelectListItem> ListaEmpresas()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem() { Text = "LABOVIDA", Value = "LABOVIDA S.A." });
            lst.Add(new SelectListItem() { Text = "LEBENFARMA", Value = "LEBENFARMA S.A." });
            lst.Add(new SelectListItem() { Text = "FARMALIGHT", Value = "FARMALIGHT S.A." });
            lst.Add(new SelectListItem() { Text = "DANIVET", Value = "LABORATORIOS DANIVET S.A." });
            lst.Add(new SelectListItem() { Text = "DANIVET 2", Value = "LABORATORIOS DANIVET S.A. 2" });
            lst.Add(new SelectListItem() { Text = "ANYUPA", Value = "LABORATORIOS ANYUPA S.A." });
            lst.Add(new SelectListItem() { Text = "MEDITOTAL", Value = "MEDITOTAL S.A." });
            return lst;
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
        public void Imprimir(int id, decimal peso, int bultos,
            string guia_urbano, bool EsLabovida = false)
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
                " AND  FP.EMPRESA = 'LABOV'" +
                " AND FP.codigo_factura like '%"+id+"%'";

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
                        x.Field<string>(7).Split('-')[1].Substring(3, 10).Substring(1, 9))),
                    identificacion = x.Field<string>(9),
                });
            as2oasis db = new as2oasis();

            var guias = db.GuiaUrbano.AsEnumerable()
                    .Union(guia_labovida)
                    .Where(x => x.id_guia_remision == id
                        && (EsLabovida==true?x.id_organizacion==69: x.id_organizacion != 69)
                    ).FirstOrDefault();



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
                Response.Write(Convert.ToBase64String(pdf_generado));
                Response.Flush();
                Response.End();
            }

            var registro_guia = new guia_urbano_troq();
            registro_guia.ciudad = guias.ciudad;
            registro_guia.direccion = guias.direccion;
            registro_guia.empresa = guias.empresa;
            registro_guia.fecha_factura = guias.fecha_factura;
            registro_guia.fecha_guia_troquelada = DateTime.Now;
            registro_guia.identificacion = guias.identificacion;
            registro_guia.id_factura = guias.id_guia_remision;
            registro_guia.id_organizacion = guias.id_organizacion;
            registro_guia.parroquia = guias.parroquia;
            registro_guia.provincia = guias.provincia;
            registro_guia.secuencial_factura = guias.secuencial_factura;
            registro_guia.valor_factura = guias.valor_factura;
            registro_guia.peso = peso;
            registro_guia.serial_urbano = guia_urbano;
            registro_guia.bultos = bultos;
            db.guia_urbano_troq.Add(registro_guia);
            db.SaveChanges();

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
