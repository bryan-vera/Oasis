using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Oasis.Models;

namespace Oasis.Controllers.Contabilidad
{
    public class ContabilidadController : Controller
    {
        // GET: Contabilidad
        public ActionResult Autorizaciones()
        {
            return View();
        }

        [HttpPost]
        public JsonResult RevisarTXT(HttpPostedFileBase file)
        {
            string contents = string.Empty; 
            string sep = "\t";

            using (var ms = new System.IO.MemoryStream())
            {
                file.InputStream.CopyTo(ms);
                ms.Position = 0;
                contents = new StreamReader(ms).ReadToEnd();
            }

            contents=contents.Replace("\n","\t");
            contents = contents.Replace("Factura\n", "Factura ");

            var array = contents.Split(sep.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            
            DataTable tablaAutorizaciones = new DataTable();
            // create columns
            for (int i = 0; i < 10; i++)
            {
                tablaAutorizaciones.Columns.Add();
            }

            for (int j = 1; j < array.Length-20; j++)
            {
                DataRow row = tablaAutorizaciones.NewRow();
                for (int i = 0; i < 10; i++)
                {
                    row[i] = array[11+j+i];
                }
                j = j + 10;
                // add the current row to the DataTable
                tablaAutorizaciones.Rows.Add(row);
            }

            var claveAccesos = tablaAutorizaciones.AsEnumerable()
                .Select(x=>new
                {
                   factura = x.Field<string>(0),
                   proveedor = x.Field<string>(2),
                   claveAcceso =  x.Field<string>(8)
                })
                .ToList()
                ;

            using (var context = new AS2Context())
            {

                var AS2ClavesAcceso =
                   context.factura_proveedorSRI
                   .Select(x => x.autorizacion)
                   .ToList();

                var listaClavesPendientes =
                    claveAccesos
                        .AsEnumerable()
                        .Where(x => !AS2ClavesAcceso.Contains(x.claveAcceso));

                var clavesSRI_json = JsonConvert.SerializeObject(listaClavesPendientes, Formatting.Indented);
                return Json(clavesSRI_json, JsonRequestBehavior.AllowGet);                
                    
            }
          
    }
    }
}
