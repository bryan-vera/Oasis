using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oasis.Models;
using Oasis.ViewModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;

namespace Oasis.Controllers.Proveedores
{
    public class DetalleOrdenCompraController : Controller
    {
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

        [HttpPost]
        public JsonResult ListaOC()
        {
            as2oasis oa = new as2oasis();
            var listaOrdenComprar = oa.ordenes_compra
                .OrderByDescending(x => x.fecha)
                .AsEnumerable().Select(x => new
                {
                    Id_oc = x.id_orden_compra,
                    Empresa = x.empresa,
                    Secuencial = x.secuencial,
                    Fecha = x.fecha.ToShortDateString(),
                    Proveedor = x.cliente,
                    Total = x.valor_total,
                    Enlazado = x.orden_enlazada
                })
                .ToList();
            return new JsonResult { Data = listaOrdenComprar };
        }

        [HttpPost]
        public JsonResult ReporteOC()
        {
            as2oasis oa = new as2oasis();
            var detalleOrdenCompra = oa.DetalleOCEnlazada
                .OrderByDescending(x => x.fecha)
                .AsEnumerable()
                .Select(x => new
                {
                    empresa = x.empresa,
                    proveedor = x.proveedor,
                    ruc_proveedor = x.ruc_proveedor,
                    secuencial = x.secuencial,
                    fecha = x.fecha.ToShortDateString(),
                    departamento = x.departamento,
                    codigo_producto = x.codigo_producto,
                    descripcion_producto = x.descripcion_producto,
                    categoria_producto = x.categoria_producto,
                    um_producto = x.um_producto,
                    cantidad_producto = x.cantidad_producto,
                    valor_linea = x.valor_linea.ToString("N2"),
                    valor_total = x.valor_total.ToString("N2")
                })
                .ToList();
            return new JsonResult { Data = detalleOrdenCompra };
        }

        // GET: DetalleOrdenCompra
        public ActionResult Index()
        {
            ViewBag.Opciones = ListaEmpresas();
            return View();
        }

        public ActionResult Reporte()
        {
            return View();
        }

        //public ActionResult Detallar(string id_oc)
        //{
        //    ViewBag.Opciones = ListaEmpresas();
        //    return View();
        //}


        [HttpPost]
        public ActionResult Create(DetalleOrdenCompra ocModel)
        {
            bool status = false;
            using (as2oasis oasis = new as2oasis())
            {
                var oc_principal = new prov_oc_principal();
                oc_principal.fecha_documento = ocModel.fecha_documento;
                oc_principal.id_proveedor = ocModel.id_proveedor;
                oc_principal.valor_total = ocModel.valor_total;
                oc_principal.id_organizacion = ocModel.id_organizacion;
                oc_principal.id_departamento = ocModel.id_departamento;
                oc_principal.id_oc_principal = ocModel.id_oc_principal;
                oasis.prov_oc_principal.Add(oc_principal);
                oasis.SaveChanges();

                int pk = oc_principal.id_oc_principal;  // You can get primary key of your inserted row
                foreach (var i in ocModel.ListaDeDetalleOrdenCompra)
                {

                    var oc_detalle = new prov_oc_detalle();
                    //oc_principal.prov_oc_detalle.Add(i);
                    oc_detalle.prov_oc_principal = oc_principal;
                    oc_detalle.cantidad_producto = i.cantidad_producto;
                    //oc_detalle.descuento = i.descuento;
                    oc_detalle.id_producto = i.id_producto;
                    oc_detalle.valor_linea = i.valor_linea;
                    oasis.prov_oc_detalle.Add(oc_detalle);
                }
                oasis.SaveChanges();
                status = true;
            }
            ModelState.Clear();


            ViewBag.SuccessMessage = "Se ha registrado el producto";
            return new JsonResult { Data = new { status = status } };
        }

        // GET: Gastos/Details/5
        public void Imprimir(int id)
        {
            OASISContext oasis = new OASISContext();
            AS2Context as2 = new AS2Context();
            var oc_detalle = new prov_oc_detalle();
            var detalle =
                oasis.prov_oc_detalle
                .Where(x => x.id_oc_principal == id)
                .Select(x => new
                {
                    codigo = x.id_producto,
                    //descripcion = x.invt_productos_gastos.descripcion,
                    cantidad = x.cantidad_producto,
                    //um = x.invt_productos_gastos.um,
                    //valor_unitario = x.invt_productos_gastos.valor_unitario,
                    valor_linea = x.valor_linea
                });

            var principal =
                oasis.prov_oc_principal
                .Where(x => x.id_oc_principal == id)
                .Select(x => new
                {
                    valor_total = x.valor_total,
                    anulada = x.anulada,
                    //empresa = x.empresa,
                    //departamento = x.id_dpto,
                    fecha_documento = x.fecha_documento,
                    ruc_proveedor = x.id_proveedor
                })
                ;

            var ruc_proveedor = principal.FirstOrDefault().ruc_proveedor;

            var proveedor =
                as2.empresa
                    //.Where(x => x.identificacion == ruc_proveedor)
                    .Select(x => new
                    {
                        x.direccion_empresa.FirstOrDefault().ubicacion.direccion1,
                        x.direccion_empresa.FirstOrDefault().telefono1,
                        x.email1,
                        x.nombre_comercial
                    })
                    .FirstOrDefault();

            //var principal =
            //    oasis.prov_oc_principal
            //    //.Join(
            //    //    as2.empresa,
            //    //    principalOC => principalOC.id_proveedor,
            //    //    proveedor => proveedor.identificacion,
            //    //    (principalOC, proveedor) => new { PrincipalOC = principalOC, Proveedor = proveedor }
            //    //    )
            //    .Where(x => x.PrincipalOC.id_oc_principal == id)
            //    //.GroupBy(x=>new {x.PrincipalOC.id_oc_principal,x.PrincipalOC.anulada,
            //    //    x.PrincipalOC.departamentos.NOMBRE_DEPARTAMENTO,
            //    //    x.PrincipalOC.empresa,
            //    //    x.PrincipalOC.fecha_documento,
            //    //    x.PrincipalOC.id_proveedor,
            //    //    x.Proveedor.direccion_empresa,
            //    //    x.PrincipalOC.valor_total,
            //    //    x.Proveedor.email1,
            //    //})
            //    .Select(x=>new 
            //    {
            //        valor_total= x.Key.valor_total,
            //        anulada = x.Key.anulada,
            //        departamento = x.Key.NOMBRE_DEPARTAMENTO,
            //        empresa = x.Key.empresa,
            //        fecha_documento = x.Key.fecha_documento,
            //        ruc_proveedor = x.Key.id_proveedor,
            //        direccion_proveedor = x.Key.direccion_empresa,
            //        email = x.Key.email1
            //    })
            //    .First()
            //    ;

            //var detalle =
            //   oasis.prov_oc_detalle
            //  .Join(oasis.invt_productos_gastos,
            //    detalleOC=>detalleOC.id_producto,
            //    producto=>producto.id_producto,
            //    (detalleOC,producto)=>new {DetalleOC=detalleOC,Producto=producto })
            //  .AsEnumerable()
            //  .Where(x => x.DetalleOC.id_oc_principal == id)
            //  .Select(x => new
            //  {
            //      codigo = x.Producto.id_producto,
            //      producto = x.Producto.descripcion,
            //      cantidad = x.DetalleOC.cantidad_producto,
            //      um=x.Producto.um,
            //      valor_unitario = x.Producto.valor_unitario,
            //      valor_linea =x.DetalleOC.invt_productos_gastos.
            //      nombre_comercial = x.Key.nombre_comercial,
            //  })
            //  ;
            using (MemoryStream myMemoryStream = new MemoryStream())
            {
                Reporte R = new Reporte();
                R.Empresa = "LABOV";
                R.MemoryStream = myMemoryStream;
                var doc = R.CrearDoc();
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

                PdfPTable tabla_interna = new PdfPTable(1);

                PdfPCell celda = new PdfPCell();
                celda = new PdfPCell(R.LABO_LOGO);
                celda.HorizontalAlignment = Element.ALIGN_CENTER;
                celda.VerticalAlignment = Element.ALIGN_MIDDLE;
                celda.Padding = 10;
                celda.Border = PdfPCell.NO_BORDER;
                tabla_interna.AddCell(celda);

                var c_ = new Chunk("ORDEN DE COMPRA No.: " + id + " \n", R.titulo);
                celda = new PdfPCell(new Phrase(c_));
                celda.Border = PdfPCell.NO_BORDER;
                tabla_interna.DefaultCell.Border = Rectangle.NO_BORDER;
                tabla_interna.AddCell(celda);
                celda = new PdfPCell(tabla_interna);
                celda.Border = PdfPCell.NO_BORDER;
                tabla_general.AddCell(celda);

                Chunk c1 = new Chunk("LABOVIDA S.A. \n", R.titulo);
                Chunk c2 = new Chunk("RUC: 0991410465001\n", R.titulo);
                Chunk c3 = new Chunk("Dirección: Av. Juan Tanca Marengo Cdla. Santa Adriana \n", R.encabezado_subtitulo);
                Chunk c4 = new Chunk("Mz. B Solar 4\n", R.encabezado_subtitulo);
                Chunk c5 = new Chunk("Teléfono: (593) 04-3082202 / (593) 04-3082249 \n", R.encabezado_subtitulo);
                Chunk c6 = new Chunk("Email: grupolabovida@yahoo.com \n", R.encabezado_subtitulo);
                Phrase p1 = new Phrase();
                p1.Add(c1);
                p1.Add(c2);
                p1.Add(c3);
                p1.Add(c4);
                p1.Add(c5);
                p1.Add(c6);
                Paragraph p = new Paragraph();
                p.Add(p1);

                celda = new PdfPCell(p);
                celda.Padding = 10;
                celda.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                tabla_interna = new PdfPTable(1);
                tabla_interna.AddCell(celda);
                celda = new PdfPCell();
                celda.Border = PdfPCell.NO_BORDER;
                tabla_interna.AddCell(celda);
                celda = new PdfPCell(tabla_interna);
                celda.Border = PdfPCell.NO_BORDER;

                tabla_general.AddCell(celda);

                //c1 = new Chunk("ORDEN DE COMPRA: " + id + " \n", R.titulo);
                //celda = new PdfPCell(new Phrase(c1));
                //tabla_general.AddCell(celda);
                //tabla_general.DefaultCell.Border = Rectangle.NO_BORDER;
                c1 = new Chunk("Fecha de emisión:", R.subtitulo_negrita);
                var c1_ = new Chunk(principal.FirstOrDefault().fecha_documento.ToShortDateString() + "\n", R.subtitulo);
                c2 = new Chunk("Proveedor:", R.subtitulo_negrita);
                var c2_ = new Chunk(proveedor.nombre_comercial + "\n", R.subtitulo);
                c3 = new Chunk("RUC / C.I.:", R.subtitulo_negrita);
                var c3_ = new Chunk(principal.FirstOrDefault().ruc_proveedor + "\n", R.subtitulo);
                c4 = new Chunk("Dirección: ", R.subtitulo_negrita);
                var c4_ = new Chunk(proveedor.direccion1 + "\n", R.subtitulo);
                c5 = new Chunk("Teléfono: ", R.subtitulo_negrita);
                var c5_ = new Chunk(proveedor.telefono1 + "\n", R.subtitulo);
                c6 = new Chunk("Email: ", R.subtitulo_negrita);
                var c6_ = new Chunk(proveedor.email1 + "\n", R.subtitulo);
                p1 = new Phrase();
                p1.Add(c1);
                p1.Add(c1_);
                p1.Add(c2);
                p1.Add(c2_);
                p1.Add(c3);
                p1.Add(c3_);
                p1.Add(c4);
                p1.Add(c4_);
                p1.Add(c5);
                p1.Add(c5_);
                p1.Add(c6);
                p1.Add(c6_);
                p = new Paragraph();
                p.Add(p1);
                celda = new PdfPCell(p);
                celda.Padding = 5;
                celda.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                tabla_general.AddCell(celda);

                c1 = new Chunk("Fecha de entrega:", R.subtitulo_negrita);
                c1_ = new Chunk("\n", R.subtitulo);
                c2 = new Chunk("Lugar de entrega: ", R.subtitulo_negrita);
                c2_ = new Chunk("CDLA. SANTA ADRIANA MZ. B SOLAR 4\n", R.subtitulo);
                p1 = new Phrase();
                p1.Add(c1);
                p1.Add(c1_);
                p1.Add(c2);
                p1.Add(c2_);
                p = new Paragraph();
                p.Add(p1);
                celda = new PdfPCell(p);
                celda.Padding = 5;
                celda.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                tabla_general.AddCell(celda);
                doc.Add(tabla_general);

                PdfPTable tabla_detalle = new PdfPTable(6)
                {
                    LockedWidth = true,
                    TotalWidth = 550f,
                    SpacingBefore = 4f
                };

                tabla_detalle.SetWidths(new float[] { 100f, 250f, 50f, 50f, 50f, 50f });

                celda = new PdfPCell(new Phrase("CÓDIGO", R.subtitulo_negrita));
                celda.FixedHeight = 30f;
                celda.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                tabla_detalle.AddCell(celda);
                celda = new PdfPCell(new Phrase("DESCRIPCIÓN", R.subtitulo_negrita));
                celda.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                celda.FixedHeight = 30f;
                tabla_detalle.AddCell(celda);
                celda = new PdfPCell(new Phrase("CANT.", R.subtitulo_negrita));
                celda.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                celda.FixedHeight = 30f;
                tabla_detalle.AddCell(celda);
                celda = new PdfPCell(new Phrase("UM", R.subtitulo_negrita));
                celda.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                celda.FixedHeight = 30f;
                tabla_detalle.AddCell(celda);
                celda = new PdfPCell(new Phrase("P.UNIT", R.subtitulo_negrita));
                celda.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                celda.FixedHeight = 30f;
                tabla_detalle.AddCell(celda);
                celda = new PdfPCell(new Phrase("P.TOTAL", R.subtitulo_negrita));
                celda.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                celda.FixedHeight = 30f;
                tabla_detalle.AddCell(celda);
                doc.Add(tabla_detalle);

                tabla_detalle = new PdfPTable(6)
                {
                    LockedWidth = true,
                    TotalWidth = 550f
                };
                tabla_detalle.SetWidths(new float[] { 100f, 250f, 50f, 50f, 50f, 50f });

                foreach (var det in detalle)
                {
                    celda = new PdfPCell(new Phrase(det.codigo.ToString(), R.subtitulo));
                    celda.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    //celda.Border = PdfPCell.NO_BORDER;
                    celda.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                    //celda.Padding = 20f;
                    celda.FixedHeight = 20f;
                    tabla_detalle.AddCell(celda);
                    //celda = new PdfPCell(new Phrase(new Chunk(det.descripcion.ToString() , R.subtitulo)));
                    celda.FixedHeight = 20f;
                    celda.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    //celda.Border = PdfPCell.NO_BORDER;
                    celda.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                    //celda.Padding = 20f;
                    tabla_detalle.AddCell(celda);
                    celda = new PdfPCell(new Phrase(new Chunk(det.cantidad.ToString(), R.subtitulo)));
                    celda.FixedHeight = 20f;
                    celda.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    //celda.Border = PdfPCell.NO_BORDER;
                    celda.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                    celda.Padding = 5f;
                    tabla_detalle.AddCell(celda);
                    //celda = new PdfPCell(new Phrase(new Chunk(det.um.ToString(), R.subtitulo)));
                    celda.FixedHeight = 20f;
                    celda.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    //celda.Border = PdfPCell.NO_BORDER;
                    celda.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                    //celda.Padding = 20f;
                    tabla_detalle.AddCell(celda);
                    //celda = new PdfPCell(new Phrase(new Chunk(det.valor_unitario.ToString(), R.subtitulo)));
                    celda.FixedHeight = 20f;
                    celda.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    //celda.Border = PdfPCell.NO_BORDER;
                    celda.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                    //celda.Padding = 20f;
                    //celda.Border = PdfPCell.NO_BORDER;
                    tabla_detalle.AddCell(celda);
                    celda = new PdfPCell(new Phrase(new Chunk(det.valor_linea.ToString(), R.subtitulo)));
                    celda.FixedHeight = 20f;
                    celda.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    //celda.Border = PdfPCell.NO_BORDER;
                    celda.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                    //celda.Padding = 10f;
                    tabla_detalle.AddCell(celda);
                }

                if (detalle.Count() < 20)
                {
                    for (var contador = detalle.Count(); contador < 20; contador++)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            celda = new PdfPCell(new Phrase(""));
                            //celda.Border = PdfPCell.NO_BORDER;
                            celda.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                            celda.FixedHeight = 20f;
                            tabla_detalle.AddCell(celda);
                        }
                    }
                }


                celda = new PdfPCell(tabla_detalle);
                celda.Padding = 0;
                //celda.Border = PdfPCell.BOTTOM_BORDER;
                var tablaAgrupaDetalle = new PdfPTable(1)
                {
                    LockedWidth = true,
                    TotalWidth = 550f
                };
                tablaAgrupaDetalle.AddCell(celda);
                doc.Add(tablaAgrupaDetalle);


                var tablaTotales = new PdfPTable(2)
                {
                    LockedWidth = true,
                    TotalWidth = 550f,
                    SpacingBefore = 15f
                };


                tablaTotales.SetWidths(new float[] { 450f, 50f });


                celda = new PdfPCell(new Phrase("SUBTOTAL", R.subtitulo));
                celda.FixedHeight = 20f;
                celda.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                celda.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                tablaTotales.AddCell(celda);

                celda = new PdfPCell(new Phrase(new Chunk(principal.FirstOrDefault().valor_total.ToString(), R.subtitulo)));
                celda.FixedHeight = 20f;
                celda.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                celda.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                tablaTotales.AddCell(celda);

                celda = new PdfPCell(new Phrase("IVA 12%", R.subtitulo));
                celda.FixedHeight = 20f;
                celda.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                celda.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                tablaTotales.AddCell(celda);


                celda = new PdfPCell(new Phrase(new Chunk(principal.FirstOrDefault().valor_total.ToString(), R.subtitulo)));
                celda.FixedHeight = 20f;
                celda.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                celda.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                tablaTotales.AddCell(celda);


                celda = new PdfPCell(new Phrase("TOTAL", R.subtitulo));
                celda.FixedHeight = 20f;
                celda.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                celda.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                tablaTotales.AddCell(celda);


                celda = new PdfPCell(new Phrase(new Chunk(principal.FirstOrDefault().valor_total.ToString(), R.subtitulo)));
                celda.FixedHeight = 20f;
                celda.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                celda.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                tablaTotales.AddCell(celda);

                celda = new PdfPCell(tablaTotales);
                tablaAgrupaDetalle = new PdfPTable(1);
                tablaAgrupaDetalle.TotalWidth = 550f;
                tablaAgrupaDetalle.LockedWidth = true;

                tablaAgrupaDetalle.AddCell(celda);
                doc.Add(tablaAgrupaDetalle);



                doc.Close();
                var pdf_generado = R.GenerarPDF();

                Response.Clear();
                Response.ClearHeaders();
                Response.AddHeader("Content-Type", "application/pdf");
                Response.AddHeader("Content-Length", pdf_generado.Length.ToString());
                Response.AddHeader("Content-Disposition", "inline; filename=file.pdf");
                Response.BinaryWrite(pdf_generado);
                Response.Flush();
                Response.End();


            }

        }


        public ActionResult Create(int id_oc)
        {
            DetalleOrdenCompra DetalleOC = new DetalleOrdenCompra();
            departamentos departamento = new departamentos();
            List<SelectListItem> lst = new List<SelectListItem>();
            AS2Context as2 = new AS2Context();
            as2oasis oasis = new as2oasis();

            lst.Add(new SelectListItem() { Text = "LABOVIDA", Value = "LABOV" });
            lst.Add(new SelectListItem() { Text = "LEBENFARMA", Value = "LEBEN" });
            lst.Add(new SelectListItem() { Text = "FARMALIGHT", Value = "FARMA" });
            lst.Add(new SelectListItem() { Text = "DANIVET", Value = "DANIV" });
            lst.Add(new SelectListItem() { Text = "ANYUPA", Value = "ANYUP" });
            lst.Add(new SelectListItem() { Text = "MEDITOTAL", Value = "MEDIT" });

            var datos_oc =
                oasis.ordenes_compra
                .AsEnumerable()
                .Where(x => x.id_orden_compra == id_oc)
                .Select(x => new
                {
                    x.id_empresa,
                    x.empresa,
                    fecha = x.fecha.ToShortDateString(),
                    x.id_cliente,
                    x.cliente,
                    x.valor_total
                })
                .FirstOrDefault();


            //join emp in as2.empresa on new {Empresa = emp }

            var proveedores =
               as2.empresa
               .AsEnumerable()
               .Where(x => x.indicador_proveedor == true && x.activo == true)
               .GroupBy(x => new { x.identificacion, x.email1, x.nombre_comercial })
               .Select(x => new empresa
               {
                   identificacion = x.Key.identificacion,
                   nombre_comercial = x.Key.nombre_comercial,
               })
               ;

            var lista_departamentos =
                oasis
               .departamentos
               .AsEnumerable()
               .Select(x => new departamentos
               {
                   id_departamento = x.id_departamento,
                   nombre = x.nombre
               })
               .OrderBy(x => x.nombre);


            ViewBag.Opciones = lst;
            ViewBag.ID_Empresa = datos_oc.id_empresa;
            ViewBag.Empresa = datos_oc.empresa;
            ViewBag.Fecha = datos_oc.fecha;
            ViewBag.Proveedor = datos_oc.cliente;
            ViewBag.ID_Proveedor = datos_oc.id_cliente;
            ViewBag.ValorTotal = datos_oc.valor_total;
            ViewBag.ID_OC = id_oc;
            //ViewBag.Proveedores = new SelectList(proveedores, "identificacion", "nombre_comercial");
            ViewBag.Departamentos = new SelectList(lista_departamentos, "id_departamento", "nombre");

            return View(DetalleOC);
        }
    }
}