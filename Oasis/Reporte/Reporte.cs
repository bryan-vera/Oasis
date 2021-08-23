using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace Oasis
{

    public class Reporte
    {
        string empresa;
        Image logo;
        Document doc;
        MemoryStream memoryStream;



        public iTextSharp.text.Image LABO_LOGO = iTextSharp.text.Image.GetInstance(Properties.Resources.LABOV, BaseColor.WHITE);
        public iTextSharp.text.Image LEBEN_LOGO = iTextSharp.text.Image.GetInstance(Properties.Resources.LEBEN, BaseColor.WHITE);
        public iTextSharp.text.Image DANIV_LOGO = iTextSharp.text.Image.GetInstance(Properties.Resources.DANIV, BaseColor.WHITE);
        public iTextSharp.text.Image FARMA_LOGO = iTextSharp.text.Image.GetInstance(Properties.Resources.FARMA, BaseColor.WHITE);
        public iTextSharp.text.Image MEDIT_LOGO = iTextSharp.text.Image.GetInstance(Properties.Resources.MEDIT, BaseColor.WHITE);
        public iTextSharp.text.Image ANYUP_LOGO = iTextSharp.text.Image.GetInstance(Properties.Resources.ANYUP, BaseColor.WHITE);


        public Font titulo = FontFactory.GetFont("georgia", 13, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        public Font subtitulo_negrita = FontFactory.GetFont("georgia", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        public Font subtitulo = FontFactory.GetFont("georgia", 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
        public Font encabezado_tabla = FontFactory.GetFont("georgia", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        public Font detalle_encabezado = FontFactory.GetFont("georgia",10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        public Font detalle = FontFactory.GetFont("georgia", 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
        public Font encabezado_subtitulo = FontFactory.GetFont("georgia", 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

        public string Empresa { get => empresa; set => empresa = value; }
        public Image Logo1 { get => logo; set => logo = value; }
        public Document Doc { get => doc; set => doc = value; }
        public MemoryStream MemoryStream { get => memoryStream; set => memoryStream = value; }

        public void Escalar (float porcentaje)
        {
            this.logo.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
            this.logo.ScalePercent(porcentaje);
        }

        public void EscogerLogo()
        {
            switch (this.empresa)
            {
                case "LABOV":
                    this.logo = LABO_LOGO;
                    break;
                case "LEBEN":
                    this.logo = LEBEN_LOGO;
                    break;
                case "DANIV":
                    this.logo = DANIV_LOGO;
                    break;
                case "FARMA":
                    this.logo = FARMA_LOGO;
                    break;
                case "MEDIT":
                    this.logo = MEDIT_LOGO;
                    break;
                case "ANYUP":
                    this.logo = ANYUP_LOGO;
                    break;
                default:
                    this.logo = LABO_LOGO;
                    break;
            }
            Escalar(40f);
        } 

        //Este metodo recepta:
        //      flag: Booleano que de ser True convierte al reporte en horizontal 
        public Document CrearDoc(Boolean flag=false)
        {
            Document doc = new Document(PageSize.A4);
            EscogerLogo();
            if (flag)
                doc.SetPageSize(PageSize.A4.Rotate());
            this.doc = doc;
            return doc;
        }

        public Document CrearDocGuia()
        {
            var ancho = iTextSharp.text.Utilities.MillimetersToPoints(230);
            var largo = iTextSharp.text.Utilities.MillimetersToPoints(140);
            var pgSize = new iTextSharp.text.Rectangle(ancho, largo);
            var doc = new iTextSharp.text.Document(pgSize, 10f, 10f, 10f, 10f); 
            this.doc = doc;
            return doc;
        }

        public Document CrearDocA4()
        {
            var doc =  new Document(PageSize.A4);
            this.doc = doc;
            return doc;
        }

        public void InsertarTexto(PdfWriter writer,String text, int x, int y)
        {
            PdfContentByte cb = writer.DirectContent;
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            if (text.Length > 26)
            {
                var primera_linea = text.Substring(0, 26);
                var segunda_linea = text.Substring(26);
                cb.SaveState();
                cb.BeginText();
                cb.MoveText(x, y);
                cb.SetFontAndSize(bf, 8);
                cb.ShowText(primera_linea);
                cb.SetLeading(10);
                cb.NewlineText();
                cb.ShowText(segunda_linea);
                cb.EndText();
                cb.RestoreState();
            } else
            {
                cb.SaveState();
                cb.BeginText();
                cb.MoveText(x, y);
                cb.SetFontAndSize(bf, 10);
                cb.ShowText(text);
                cb.EndText();
                cb.RestoreState();
            }
        }


        public iTextSharp.text.Image Imagen(String ruta)
        {
            //@"Resources\labovida.jpg"
            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(ruta);
            return img;
        }

        public void Logo(string ruta, Document doc)
        {
            iTextSharp.text.Image img = Imagen(ruta);
            img.SetAbsolutePosition(100, 650);
            doc.Add(img);
            img.ScaleAbsolute(25f, 25F);
        }

        public iTextSharp.text.Font Fuente(Font _font)
        {
            iTextSharp.text.Font _standardFont = _font;
            return _standardFont;
        }

        public iTextSharp.text.Font Fuente_B()
        {
            // Creamos el tipo de Font que vamos utilizar
            iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(
                    iTextSharp.text.Font.FontFamily.HELVETICA, 6, iTextSharp.text.Font.BOLD,
                    BaseColor.BLACK);
            return _standardFont;
        }

        public byte[] GenerarPDF()
        {
            byte[] bytes = this.memoryStream.ToArray();
            memoryStream.Close();
            return bytes;
        }


        public PdfWriter CrearPDF()
        {
            PdfWriter writer = PdfWriter.GetInstance(this.doc,
                                      this.memoryStream);
            return writer;
        }

        public void Iniciar(Document doc)
        {
            doc.AddTitle("PDF LABO");
            doc.AddCreator("B. Vera");
            doc.Open();
        }

        public void Titulo(Document doc, string Titulo, iTextSharp.text.Font _standardFont)
        {
            Chunk _Titulo = new Chunk(Titulo, _standardFont);
            Paragraph __Titulo = new Paragraph(_Titulo);
            __Titulo.Alignment = Element.ALIGN_CENTER;
            doc.Add(__Titulo);
            doc.Add(Chunk.NEWLINE);
        }

        public PdfPTable TablaPDF(int columnas)
        {
            PdfPTable Tabla = new PdfPTable(columnas);
            //Crea borders del lado izquierdo y derecho de la celda
            Tabla.DefaultCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
            Tabla.HeaderRows = 1;
            return Tabla;
        }

        public float[] AnchoCols(Font font, params string[] headers)
        {
            var total = 0;
            var columns = headers.Length;
            var widths = new int[columns];
            for (var i = 0; i < columns; ++i)
            {
                var w = font.GetCalculatedBaseFont(true).GetWidth(headers[i]);
                total += w;
                widths[i] = w;
            }
            var result = new float[columns];
            for (var i = 0; i < columns; ++i)
            {
                result[i] = (float)widths[i] / total * 100;
            }
            return result;
        }

        //public void Contenido(int columnas, List<string> encabezados,
        //    DataGridView dataGridView1, PdfPTable Tabla,
        //    float[] widths, iTextSharp.text.Font _standardFont)
        //{

        //    //Se usa el 100% de la tabla 
        //    Tabla.WidthPercentage = 100;
        //    Tabla.SetWidths(widths);

        //    //Ancho de cada celda
        //    // Configuramos el título de las columnas de la tabla
        //    for (int i = 0; i <= columnas - 1; i++)
        //    {
        //        PdfPCell u = new PdfPCell(new Phrase(encabezados[i], _standardFont));
        //        u.BorderWidth = 0.5f;
        //        u.BorderWidthBottom = 0.75f;
        //        // Añadimos las celdas a la tabla
        //        Tabla.AddCell(u);

        //    }
        //}



        //Este metodo recoge los siguientes parametros:
        //          DATAGRIDVIEW: donde se esta realizando la consulta
        //          Tabla: Objeto tabla que se va a agregar al PDF
        //          _standardFont: Objeto de PDF# que contiene valores de fuente
        //          Celdas: Lista de celdas que contendran los valores 
        //
        //Este metodo genera un recorrido al DataGridView para ir poblando a las celdas

        //public void detalleContenido(DataGridView dataGridView1, PdfPTable Tabla,
        //                            iTextSharp.text.Font _standardFont, Document doc,
        //                            List<PdfPCell> Celdas)
        //{

        //    for (int rows = 0; rows < dataGridView1.Rows.Count; rows++)
        //    {
        //        for (int i = 0; i < Celdas.Count; i++)
        //        {
        //            string value = dataGridView1.Rows[rows].Cells[i].FormattedValue.ToString();
        //            Celdas[i] = new PdfPCell(new Phrase(value, _standardFont));
        //            Celdas[i].BorderWidth = 0.2f;
        //            Celdas[i].BorderWidthBottom = 0.75f;
        //            Tabla.AddCell(Celdas[i]);
        //        }

        //    }

        //    doc.Add(Tabla);
        //    //Genera encabezados en las siguientes paginas 
        //    Tabla.HeaderRows = 1;

        //}

        public void LineaTexto(Document doc, string linea, iTextSharp.text.Font font)
        {
            Chunk Line = new Chunk(linea, font);
            Paragraph _Line = new Paragraph(Line);
            _Line.Alignment = Element.ALIGN_CENTER;
            doc.Add(_Line);
            doc.Add(Chunk.NEWLINE);
        }


        //public void CreaReport(DataGridView dg, iTextSharp.text.Font font, iTextSharp.text.Font fontEnc,
        //                        Document doc, PdfWriter writer, float[] widths)
        //{
        //    List<string> lista1 = Encabezados(dg);
        //    var celdas = new List<PdfPCell>();
        //    celdas = SetCeldasPDF(lista1);
        //    PdfPTable Tabla = TablaPDF(lista1.Count);
        //    Contenido(lista1.Count, lista1, dg, Tabla, widths, fontEnc);
        //    detalleContenido(dg, Tabla, font, doc, celdas);
        //    Cerrar(doc, writer);
        //}

        public List<PdfPCell> SetCeldasPDF(List<string> lista1)
        {
            var celdas = new List<PdfPCell>();
            var _celdas = new PdfPCell();
            for (int i = 0; i < lista1.Count; i++)
            {
                celdas.Add(_celdas);
            }
            return celdas;
        }

        //public List<string> Encabezados(DataGridView dg)
        //{
        //    var celdas = new List<PdfPCell>();
        //    List<string> lista1 = new List<string>();
        //    for (int i = 0; i < dg.ColumnCount; i++)
        //    {
        //        lista1.Add(dg.Columns[i].HeaderText.ToString());
        //    }
        //    return lista1;
        //}



        public void Cerrar(Document doc, PdfWriter writer)
        {
            doc.Close();
            writer.Close();
            System.Diagnostics.Process.Start(@"C:\Reporteria\ReporteGeneral.pdf");
            doc.CloseDocument();
        }

        internal class RoundedBorder : IPdfPCellEvent
        {
            public void CellLayout(PdfPCell cell, Rectangle rect, PdfContentByte[] canvas)
            {
                PdfContentByte cb = canvas[PdfPTable.BACKGROUNDCANVAS];
                cb.RoundRectangle(
                  rect.Left + 1.5f,
                  rect.Bottom + 1.5f,
                  rect.Width - 1.5,
                  rect.Height - 1.5, 4
                );
                cb.SetColorStroke(new BaseColor(53, 101, 255));
                cb.SetColorFill(new BaseColor(53, 101, 255));
                cb.Stroke();
            }
        }

    }
}