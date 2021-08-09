using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml.Serialization;
using clsConectaMBA;

namespace Oasis.Models.ATS
{
    public class Retenciones
    {
        private string fechaInicio;
        private string fechaFin;

        public DateTime FechaInicio
        {
            get { return DateTime.Parse(fechaInicio); }
            set
            {
                fechaInicio = value.ToString("yyyy/MM/dd");
            }
        }
        public DateTime FechaFin
        {
            get { return DateTime.Parse(fechaFin); }
            set
            {
                fechaFin = value.ToString("yyyy/MM/dd");
            }
        }
        public string Empresa { get; set; }

        private const string numEstablecimiento = "001";
        private const string tipoIDInformante = "R";
        private const string codigoOperativo = "IVA";

        public Retenciones()
        {


        }

        public Iva ObtenerRetenciones()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            ConexionMba cs = new ConexionMba();
            var ConexionMBA = cs.getConexion();

            ////Info de empresa 
            //   < TipoIDInformante > R </ TipoIDInformante >
            //   < IdInformante > 0992412755001 </ IdInformante >
            //   < razonSocial > FARMALIGHT SA </ razonSocial >
            //   < Anio > 2019 </ Anio >
            //   < Mes > 07 </ Mes >
            //   < numEstabRuc > 001 </ numEstabRuc >
            //   < totalVentas > 188983.37 </ totalVentas >
            //   < codigoOperativo > IVA </ codigoOperativo >
            string cadena = "SELECT  `CORPORATION NAM`, `FED ID OR RUC` " +
                " FROM SIST_Parametros_Empresa " +
                " WHERE CORP = '" + Empresa + "'";

            OdbcCommand Comando = new OdbcCommand(cadena, ConexionMBA);
            OdbcDataAdapter adp = new OdbcDataAdapter();
            adp.SelectCommand = Comando;
            DataSet dst = new DataSet();
            adp.Fill(dst, "datos_empresa");

            var info_empresa = dst.Tables["datos_empresa"].AsEnumerable();

            Iva ats_xml = new Iva();
            ats_xml.TipoIDInformante = "R";
            ats_xml.IdInformante = info_empresa.Select(x => x.Field<string>(1)).FirstOrDefault();
            ats_xml.RazonSocial = info_empresa.Select(x => x.Field<string>(0)).FirstOrDefault().Replace(".", string.Empty); ;
            ats_xml.Anio = FechaInicio.ToString("yyyy");
            ats_xml.Mes = string.Format("{0:00}", FechaInicio.Month);
            ats_xml.NumEstabRuc = "001";
            ats_xml.CodigoOperativo = "IVA";


            //lectura de facturas
            cadena =
                "select cfp.empresa,cfp.codigo_factura, cfp.anulada, " +
                "cfp.valor_factura,cfp.valor_retencion,cfp.codigo_cliente_empresa," +
                "cif.mf_lista7,RIGHT(LEFT(cfp.numeroentextofactura,4),3) as ptoEmiRet, " +
                "RIGHT(cfp.numeroentextofactura,9) as secuencial, " +
                "cfp.factura_xml_codigo_acceso, cfp.numeroaprobseriedocumento, " +
                "cfp.confirmado  " +
                "from CLNT_FACTURA_PRINCIPAL cfp inner join " +
                "CONT_info_fiscal cif " +
                "on cfp.codigo_factura = cif.id_relacionado " +
                "where " +
                "cfp.fecha_factura >= '" + fechaInicio + "' " +
                "and cfp.fecha_factura <= '" + fechaFin + "' " +
                "and cfp.empresa = '" + Empresa + "' " +
                "and cif.mf_bool3 = 0 " +
                "and cfp.numeroaprobseriedocumento = '9999999999'  " +
                "and cfp.numeroentextofactura <> ''  " +
                "group by  cfp.empresa,cfp.codigo_factura, cfp.anulada, " +
                "cfp.valor_factura,cfp.valor_retencion,cfp.codigo_cliente_empresa," +
                "cif.mf_lista7,cfp.numeroentextofactura,cfp.factura_xml_codigo_acceso, cfp.numeroaprobseriedocumento," +
                "cfp.confirmado";

            Comando = new OdbcCommand(cadena, ConexionMBA);
            adp = new OdbcDataAdapter();
            adp.SelectCommand = Comando;
            adp.Fill(dst, "facturas");

            //lectura de nc
            cadena = "SELECT ncpp.CORP, ncpp.`DOC ID CORP`, ncpp.VOID , " +
                 " ncpp.`TOTAL AMOUNT`,ncpp.`TOTAL AMOUNT`, ncpp.`PARTY CODE`, " +
                 " clie.IDENTIFICACION_FISCAL, RIGHT(LEFT(ncpp.Document_ID_EnTexto,4),3) as ptoEmi, " +
                 " RIGHT(ncpp.Document_ID_EnTexto,9) as secuencial," +
                 " ncpp.nota_xml_codigo_acceso , ncpp.numeroaprobseriedocumento, " +
                 " ncpp.aproved,ncpp.CD  " +
                 " FROM " +
                 " CONT_info_fiscal cif INNER JOIN(" +
                 " NTDC_Client_Prov_Principal ncpp INNER JOIN clnt_ficha_principal clie " +
                 " on ncpp.`party code` = clie.codigo_cliente_empresa " +
                 " ) on ncpp.`DOC ID CORP` = cif.id_relacionado " +
                 " where  " +
                 //" ncpp.CD = 'CR' and " +
                 //" ncpp.APROVED = CAST('TRUE' AS BOOLEAN) AND " +
                 " ncpp.`PRINTED DATE` >= '" + fechaInicio + "' AND " +
                 " ncpp.`PRINTED DATE` <= '" + fechaFin + "' AND " +
                 " ncpp.CLIENT = CAST('TRUE' AS BOOLEAN) AND " +
                 " ncpp.CORP = '" + Empresa + "' AND " +
                 " ncpp.PRINTED = 'PRINTED' AND " +
                "  cif.mf_bool3 = 0 AND  " +
                 " ncpp.numeroaprobseriedocumento='9999999999'";

            Comando = new OdbcCommand(cadena, ConexionMBA);
            adp = new OdbcDataAdapter();
            adp.SelectCommand = Comando;
            adp.Fill(dst, "nc");


            //lectura de claves de emision
            cadena = "select documento_id,documento_estado, " +
                " DATE_TO_CHAR(documento_fecha_proceso, 'dd[/]mm[/]yyyy') AS documento_fecha_proceso  from " +
                " SIST_fiscal_lotes where CORP='" + Empresa + "' and " +
                " documento_fecha>= '" + fechaInicio + "' and " +
                " documento_fecha<= '" + fechaFin + "' ";

            Comando = new OdbcCommand(cadena, ConexionMBA);
            adp = new OdbcDataAdapter();
            adp.SelectCommand = Comando;
            adp.Fill(dst, "claves_emision");

            var factura_cn_claves =
                (from fact in dst.Tables["facturas"].AsEnumerable()
                 join clav in dst.Tables["claves_emision"].AsEnumerable()
                 on fact.Field<string>(1) equals
                     clav.Field<string>(0) into clav_
                 from clav in clav_.DefaultIfEmpty()
                 select new
                 {
                     empresa = fact.Field<string>(0),
                     codigo_factura = fact.Field<string>(1),
                     anulada = fact.Field<Boolean>(2),
                     valor_factura = fact.Field<Single>(3),
                     valor_retencion = fact.Field<Single>(4),
                     codigo_cliente_empresa = fact.Field<String>(5),
                     formaPago = fact.Field<String>(6) == "" ? "20" : fact.Field<String>(6),
                     tipo_emision = clav == null
                     //|| clav.Field<DateTime>(2)== null
                     ? "F" : "E",
                     puntoEmi = fact.Field<string>(7) == ""? "001":fact.Field<string>(7),
                     secuencial = fact.Field<string>(8) == ""?"FISICA":fact.Field<string>(8),
                     codigo_acceso = fact.Field<string>(9) == "" ? fact.Field<string>(10) : fact.Field<string>(9)


                 }).ToList();

            //var nc_prov_con_claves =
            //    (from nc in dst.Tables["nc_proveedores"].AsEnumerable()
            //     join clav in dst.Tables["claves_emision"].AsEnumerable()
            //     on nc.Field<string>(1) equals
            //         clav.Field<string>(0) into clav_
            //     from clav in clav_.DefaultIfEmpty()
            //     select new
            //     {
            //         empresa = nc.Field<string>(0),
            //         codigo_nc = nc.Field<string>(1),
            //         anulada = nc.Field<Boolean>(2),
            //         valor_nc = nc.Field<Single>(3),
            //         codigo_proveedor_empresa = nc.Field<String>(5),
            //         formaPago = nc.Field<String>(6),
            //         tipo_emision = clav == null
            //         //|| clav.Field<DateTime>(2)== null
            //         ? "F" : "E",
            //         puntoEmi = nc.Field<string>(7),
            //         secuencial = nc.Field<string>(8),
            //         codigo_acceso = nc.Field<string>(9) == "" ? nc.Field<string>(10) : nc.Field<string>(9),
            //         tipo = nc.Field<string>(12)
            //     }).ToList();

            var nc_cn_claves =
                (from nc in dst.Tables["nc"].AsEnumerable()
                 join clav in dst.Tables["claves_emision"].AsEnumerable()
                 on nc.Field<string>(1) equals
                     clav.Field<string>(0) into clav_
                 from clav in clav_.DefaultIfEmpty()
                 select new
                 {
                     empresa = nc.Field<string>(0),
                     codigo_nc = nc.Field<string>(1),
                     anulada = nc.Field<Boolean>(2),
                     valor_nc = nc.Field<Single>(3),
                     codigo_cliente_empresa = nc.Field<String>(5),
                     formaPago = nc.Field<String>(6),
                     tipo_emision = clav == null
                     //|| clav.Field<DateTime>(2)== null
                     ? "F" : "E",
                     puntoEmi = nc.Field<string>(7),
                     secuencial = nc.Field<string>(8),
                     codigo_acceso = nc.Field<string>(9) == "" ? nc.Field<string>(10) : nc.Field<string>(9),
                     tipo = nc.Field<string>(12)

                 }).ToList();


            cadena = "select cfip.codigo_cliente_empresa,cif.mf_lista1 as tpIdCliente, " +
                "cfip.IDENTIFICACION_FISCAL as Identcliente " +
                "from CONT_Info_Fiscal " +
                "cif inner join CLNT_Ficha_Principal cfip " +
                "on cif.id_relacionado = cfip.codigo_cliente_empresa";

            Comando = new OdbcCommand(cadena, ConexionMBA);
            adp = new OdbcDataAdapter();
            adp.SelectCommand = Comando;
            adp.Fill(dst, "cliente_info");


            var fact_clnt =
                from fac in factura_cn_claves
                join cln in dst.Tables["cliente_info"].AsEnumerable()
                on fac.codigo_cliente_empresa equals cln.Field<string>(0)
                //into clientes
                //from cln in clientes
                select new
                {
                    fac.empresa,
                    fac.codigo_factura,
                    fac.anulada,
                    fac.valor_factura,
                    fac.valor_retencion,
                    fac.codigo_cliente_empresa,
                    fac.tipo_emision,
                    fac.formaPago,
                    fac.puntoEmi,
                    fac.secuencial,
                    fac.codigo_acceso,
                    tpIdCliente = cln.Field<string>(1) == "" ? "04" : cln.Field<string>(1),
                    Identificacion = cln.Field<string>(2)
                };

            var nc_clnt =
               from nc in nc_cn_claves
               join cln in dst.Tables["cliente_info"].AsEnumerable()
               on nc.codigo_cliente_empresa equals cln.Field<string>(0)
               //into clientes
               //from cln in clientes
               select new
               {
                   nc.empresa,
                   nc.codigo_nc,
                   nc.anulada,
                   nc.valor_nc,
                   nc.codigo_cliente_empresa,
                   nc.tipo_emision,
                   nc.formaPago,
                   nc.puntoEmi,
                   nc.secuencial,
                   nc.codigo_acceso,
                   nc.tipo,
                   tpIdCliente = cln.Field<string>(1) == "" ? "04" : cln.Field<string>(1),
                   Identificacion = cln.Field<string>(2)
               };

            var nc_clientes_no_anuladas =
                nc_clnt.Where(x => x.anulada == false && x.tipo == "CR").GroupBy(x => new
                {
                    x.codigo_cliente_empresa,
                    x.tipo_emision,
                    x.empresa,
                    x.formaPago,
                    x.tpIdCliente,
                    x.Identificacion

                }).Select(x => new {
                    x.Key.empresa,
                    x.Key.codigo_cliente_empresa,
                    x.Key.tipo_emision,
                    x.Key.formaPago,
                    x.Key.Identificacion,
                    x.Key.tpIdCliente,
                    valor_total = x.Sum(y => y.valor_nc),
                    num_comprobantes = x.Count()
                });

            var nc_clientes_anuladas =
               nc_clnt.Where(x => x.anulada == true);
            //&& x.Identificacion == "0992412755001"
            var ventas_clientes_no_anuladas =
                fact_clnt.Where(x => x.anulada == false).GroupBy(x => new
                {
                    x.tipo_emision,
                    x.empresa,
                    x.formaPago,
                    x.tpIdCliente,
                    x.Identificacion
                }).Select(x => new {
                    x.Key.empresa,
                    x.Key.tipo_emision,
                    x.Key.formaPago,
                    x.Key.Identificacion,
                    tpIdCliente = x.Key.tpIdCliente == "" ? "04" : x.Key.tpIdCliente,
                    valor_total = x.Sum(y => y.valor_factura),
                    valor_retencion = x.Sum(y => y.valor_retencion),
                    num_comprobantes = x.Count()
                });

            var ventas_clientes_anuladas =
                fact_clnt.Where(x => x.anulada == true);


            ats_xml.TotalVentas = string.Format("{0:0.00}",
                ventas_clientes_no_anuladas.Where(x => x.tipo_emision == "E").Sum(x => x.valor_total)
                -
                nc_clientes_no_anuladas.Where(x => x.tipo_emision == "E").Sum(x => x.valor_total)
                );

            float suma_baseImponible = 0;
            float suma_valorRenta = 0;

            List<DetalleVentas> ventas = new List<DetalleVentas>();

            foreach (var vent in ventas_clientes_no_anuladas)
            {
                var elem_venta = new DetalleVentas();
                elem_venta.TpIdCliente = vent.tpIdCliente;
                elem_venta.IdCliente = vent.Identificacion;
                elem_venta.ParteRelVtas = "NO";
                elem_venta.TipoComprobante = "18";
                elem_venta.TipoEmision = vent.tipo_emision;
                elem_venta.NumeroComprobantes = vent.num_comprobantes.ToString();
                elem_venta.BaseNoGraIva = "0";
                elem_venta.BaseImponible = string.Format("{0:0.00}", vent.valor_total);
                elem_venta.BaseImpGrav = "0.00";
                elem_venta.MontoIva = "0.00";
                elem_venta.MontoIce = "0.00";
                elem_venta.ValorRetIva = "0.00";
                elem_venta.ValorRetRenta = string.Format("{0:0.00}", vent.valor_retencion);
                var fpago = new FormasDePago();
                //fpago.FormaPago = vent.formaPago;
                elem_venta.FormasDePago = new FormasDePago() { FormaPago = vent.formaPago };
                //elem_venta.FormasDePago.FormaPago = vent.formaPago;
                ventas.Add(elem_venta);
                if (vent.tipo_emision == "F")
                {
                    suma_baseImponible += vent.valor_total;
                    suma_valorRenta += vent.valor_retencion;
                }
            }

            Console.WriteLine($"{suma_baseImponible},{suma_valorRenta}");

            foreach (var vent in nc_clientes_no_anuladas)
            {
                var elem_venta = new DetalleVentas();
                elem_venta.TpIdCliente = vent.tpIdCliente;
                elem_venta.IdCliente = vent.Identificacion;
                elem_venta.ParteRelVtas = "NO";
                elem_venta.TipoComprobante = "04";
                elem_venta.TipoEmision = vent.tipo_emision;
                elem_venta.NumeroComprobantes = vent.num_comprobantes.ToString();
                elem_venta.BaseNoGraIva = "0";
                elem_venta.BaseImponible = string.Format("{0:0.00}", vent.valor_total);
                elem_venta.BaseImpGrav = "0.00";
                elem_venta.MontoIva = "0.00";
                elem_venta.MontoIce = "0.00";
                elem_venta.ValorRetIva = "0.00";
                elem_venta.ValorRetRenta = "0.00";
                ventas.Add(elem_venta);
            }



            ats_xml.Ventas = new Ventas() { DetalleVentas = ventas };


            cadena = "select pfp.analisis1 as idProv, " +
                " ocpr.`DATE` as fechaRegistro, " +
                " LEFT(ocpr.`DOC REF`, 3) AS establecimiento, " +
                " RIGHT(LEFT(ocpr.`DOC REF`, 6), 3) AS puntoEmision, " +
                " RIGHT(ocpr.`DOC REF`, 9) AS secuencial, " +
                " ocpr.Codigo_alterno_retencion as codRetAir, " +
                " ocpr.BASIS as baseImpAir, " +
                " ocpr.PERCENTAGE as porcentajeAir, " +
                " ocpr.`RET AMOUNT` as valRetAir, " +
                " ocpr.`DOC ID CORP`, " +
                " ocpr.tipo_de_campo, " +
                " CONCAT('00', LEFT(ocpr.numeroretencionentexto,1)) as estabRet, " +
                " RIGHT(LEFT(ocpr.numeroretencionentexto, 4), 3) as puntoEmiRet, " +
                " RIGHT(ocpr.numeroretencionentexto, 9) AS secuencialRete, " +
                " ocpr.retencion_xml_codigo_acceso, " +
                " ocpr.numeroaprobseriedocumento, " +
                " ocpr.anulada,ocpr.confirmed, " +
                " pfp.TotalProductosSinIVa+pfp.TotalServiciosSinIVa as TotaSinIVa, " +
                " pfp.TotalProductosConIVa + pfp.TotalServiciosConIVa as TotalConIVa, " +
                " (pfp.Monto_Impuesto_1 + pfp.`AMOUNT TAX2`) as MontoIVA " +
                " from " +
                " OTR_Client_Prov_Retencion_HIST ocpr " +
                " INNER JOIN  PROV_FACTURA_PRINCIPAL pfp " +
                " on ocpr.`DOC ID CORP` = pfp.DOC_ID_CORP " +
                " where ocpr.CORP = '" + Empresa + "' and " +
                " ocpr.`DATE`>='" + fechaInicio + "' and " +
                " ocpr.`DATE`<='" + fechaFin + "' and " +
                " ocpr.tipo_de_campo <>'I' and " +
            //" RIGHT(ocpr.`DOC REF`, 9) = '000000032' and " +
            //" ocpr.retencion_xml_codigo_acceso = '0206202101090708384400120031040005966460206664619' and " +
            //" RIGHT(ocpr.`DOC REF`, 9) = '000028519' and "+
            //" ocpr.anulada = cast('FALSE' as boolean) and " +
            //" ocpr.confirmed = CAST('TRUE' as Boolean) and " +
            " ocpr.type='P' "
            //" and pfp.analisis1 = '1706284922001' " +
            //" and  ocpr.`DOC REF` like '%000010529%' "
            ;

            Comando = new OdbcCommand(cadena, ConexionMBA);
            adp = new OdbcDataAdapter();
            adp.SelectCommand = Comando;
            adp.Fill(dst, "retenciones");

            cadena = " select " +
                " cif.mf_lista1 as codSustento , " +
                " cif.mf_lista2 as tipoComprobante, " +
                "cif.mf_alfa1 as autorizacion, " +
                "(pfp.TotalProductosSinIva + pfp.TotalServiciosSinIVA) as baseImponible  , " +
                "pfp.BaseGravadaCR as baseImpGrav , " +
                "pfp.`AMOUNT TAX2`  as montoIva , " +
                "pfp.doc_id_corp, " +
                "pfp.vendor_id_corp, " +
                "cif.mf_alfa3 as ICE " +
                "from " +
                "prov_factura_principal pfp  inner join  cont_info_fiscal cif " +
                "on pfp.doc_id_corp = cif.id_relacionado " +
                "where " +
                "pfp.confirmed = cast('true' as boolean) and " +
                "pfp.void = cast('false' as boolean) and " +
                "cif.mf_lista1 <> '' ";

            Comando = new OdbcCommand(cadena, ConexionMBA);
            adp = new OdbcDataAdapter();
            adp.SelectCommand = Comando;
            adp.Fill(dst, "Detalle_Retenciones");


            //Lectura NC de proveedores 
            cadena = "SELECT ncpp.CORP, ncpp.`DOC ID CORP`, ncpp.`ARRIVAL DATE`, " +
                " ncpp.VOID ,  ncpp.`TOTAL AMOUNT`," +
                " ncpp.`TOTAL AMOUNT`, ncpp.`PARTY CODE`, prov.`RUC or FED ID`," +
                " LEFT(pfp.DOC_REFERENCE, 3) AS establecimiento_fact," +
                "  RIGHT(LEFT(pfp.DOC_REFERENCE, 6), 3) AS puntoEmision_fact, " +
                " RIGHT(pfp.DOC_REFERENCE, 9) AS secuencial_fact, " +
                "LEFT(cif.mf_alfa1, 3) as establecimiento, " +
                "RIGHT(LEFT(cif.mf_alfa1, 6), 3) as ptoEmi,   " +
                "RIGHT(cif.mf_alfa1, 9) as secuencial,cif.mf_alfa2,  " +
                "cif.mf_alfa2, cif.mf_lista1 , ncpp.aproved,ncpp.CD,ciffac.mf_alfa1 'claveAccesoFact'  " +
                "FROM  CONT_info_fiscal ciffac inner join  " +
                "( PROV_FACTURA_PRINCIPAL pfp inner join ( CONT_info_fiscal cif    " +
                "INNER JOIN(NTDC_Client_Prov_Principal ncpp " +
                "INNER JOIN prov_ficha_principal prov  on  " +
                "ncpp.`party code` = prov.CODIGO_PROVEEDOR_EMPRESA )  " +
                "on ncpp.`DOC ID CORP` = cif.id_relacionado ) " +
                " on pfp.DOC_ID_CORP = ncpp.FROM_INVOICE_ID ) " +
                " on pfp.DOC_ID_CORP  = ciffac.id_relacionado  " +
                " where " +
                " ncpp.`ARRIVAL DATE` >= '" + fechaInicio + "' AND " +
                " ncpp.`ARRIVAL DATE` <= '" + fechaFin + "' AND " +
                " ncpp.VENDOR = CAST('TRUE' AS BOOLEAN) " +
                " AND ncpp.CORP = '" + Empresa + "' " +
                " AND cif.MF_Bool1  = 0  " +
                " AND ncpp.APROVED = CAST('TRUE' as boolean) " +
                " AND ncpp.VOID = CAST('FALSE' as boolean) ";



            Comando = new OdbcCommand(cadena, ConexionMBA);
            adp = new OdbcDataAdapter();
            adp.SelectCommand = Comando;
            adp.Fill(dst, "nc_proveedores");

            cadena = "SELECT  ncpp.`DOC ID CORP`,ncpd.TYPE, ncpd.AMOUNT from " +
                " NTDC_Client_Prov_Detalle ncpd inner join " +
                " NTDC_Client_Prov_Principal ncpp  " +
                " on ncpp.`DOC ID CORP` = ncpd.`DOC ID CORP` " +
                " where " +
                //" ncpp.`DOC ID CORP` = '00000000000001875-DANIV-D' " +
                " ncpp.VENDOR = CAST('TRUE' AS BOOLEAN) AND " +
                " ncpp.`ARRIVAL DATE` >= '" + fechaInicio + "' AND " +
                " ncpp.`ARRIVAL DATE` <= '" + fechaFin + "' AND " +
                " ncpp.CORP = '" + Empresa + "' AND  " +
                " ncpd.TYPE = 'PNC04' ";

            Comando = new OdbcCommand(cadena, ConexionMBA);
            adp = new OdbcDataAdapter();
            adp.SelectCommand = Comando;
            adp.Fill(dst, "detalle_proveedores_nc_imp");

            cadena = "SELECT  ncpp.`DOC ID CORP`,ncpd.TYPE, ncpd.AMOUNT from " +
                " NTDC_Client_Prov_Detalle ncpd inner join " +
                " NTDC_Client_Prov_Principal ncpp  " +
                " on ncpp.`DOC ID CORP` = ncpd.`DOC ID CORP` " +
                " where " +
                //" ncpp.`DOC ID CORP` = '00000000000001875-DANIV-D' " +
                " ncpp.VENDOR = CAST('TRUE' AS BOOLEAN) AND " +
                " ncpp.`ARRIVAL DATE` >= '" + fechaInicio + "' AND " +
                " ncpp.`ARRIVAL DATE` <= '" + fechaFin + "' AND " +
                " ncpp.CORP = '" + Empresa + "' AND  " +
                " ncpd.TYPE = 'PNC08' ";

            Comando = new OdbcCommand(cadena, ConexionMBA);
            adp = new OdbcDataAdapter();
            adp.SelectCommand = Comando;
            adp.Fill(dst, "detalle_proveedores_nc_iva");

            var nc_prov_totales =
                (from nc in dst.Tables["nc_proveedores"].AsEnumerable()
                 join det in dst.Tables["detalle_proveedores_nc_imp"].AsEnumerable()
                 on nc.Field<string>(1) equals
                     det.Field<string>(0) into nc_
                 from det in nc_.DefaultIfEmpty()
                 join detiva in dst.Tables["detalle_proveedores_nc_iva"].AsEnumerable()
                 on nc.Field<string>(1) equals
                     detiva.Field<string>(0) into nc_2
                 from detiva in nc_2.DefaultIfEmpty()
                 select new
                 {
                     codSustento = "01",
                     tpIdProv = "01",
                     idProv = nc.Field<string>(7),
                     tipoComprobante = "04",
                     parteRel = "NO",
                     fechaRegistro = nc.Field<DateTime>(2).ToString("dd/MM/yyyy"),
                     establecimiento = nc.Field<string>(11),
                     puntoEmision = nc.Field<string>(12),
                     secuencial = nc.Field<string>(13),
                     fechaEmision = nc.Field<DateTime>(2).ToString("dd/MM/yyyy"),
                     autorizacion = nc.Field<string>(14).Trim('\r', '\n'),
                     baseNoGraIva = "0.00",
                     baseImponible = "0.00",
                     baseImpGrav = det == null ? "0.00" : string.Format("{0:0.00}", det.Field<float>(2)),
                     baseImpExe = "0.00",
                     montoIce = "0.00",
                     montoIva = detiva == null ? "0.00" : string.Format("{0:0.00}", detiva.Field<float>(2)),
                     valRetBien10 = "0.00",
                     valRetServ50 = "0.00",
                     valorRetServicios = "0.00",
                     valRetServ100 = "0.00",
                     totbasesImpReemb = "0.00",
                     formaPago = nc.Field<string>(16),
                     docModificado = "01",
                     estabModificado = nc.Field<string>(8),
                     ptoEmiModificado = nc.Field<string>(9),
                     secModificado = nc.Field<string>(10),
                     autModificado = nc.Field<string>(19).Trim('\r', '\n')
                 }).ToList();

            cadena = "select pfp.CODIGO_PROVEEDOR_EMPRESA,cif.mf_lista1 " +
                " from " +
                " CONT_Info_Fiscal cif inner join " +
                " PROV_FICHA_PRINCIPAL pfp " +
                " on cif.id_relacionado = pfp.CODIGO_PROVEEDOR_EMPRESA ";

            Comando = new OdbcCommand(cadena, ConexionMBA);
            adp = new OdbcDataAdapter();
            adp.SelectCommand = Comando;
            adp.Fill(dst, "detalle_proveedor");

            var retenciones =
                (from ret in dst.Tables["retenciones"].AsEnumerable()
                 select new
                 {
                     idProv = ret.Field<string>(0),
                     fechaRegistro = ret.Field<DateTime>(1),
                     establecimiento = ret.Field<string>(2),
                     puntoEmision = ret.Field<string>(3),
                     secuencial = ret.Field<string>(4),
                     codRetAir = ret.Field<string>(5),
                     baseImpAir = ret.Field<Single>(6),
                     porcentajeAir = ret.Field<Single>(7),
                     valRetAir = ret.Field<Single>(8),
                     doc_id_corp = ret.Field<string>(9),
                     tipo_de_campo = ret.Field<string>(10),
                     estabRet = ret.Field<string>(11),
                     puntoEmiRet = ret.Field<string>(12),
                     secuencialRet = ret.Field<string>(13),
                     codigo_acceso = ret.Field<string>(14) == "" ? ret.Field<string>(15).Trim('\r', '\n') : ret.Field<string>(14).Trim('\r', '\n'),
                     anulada = ret.Field<Boolean>(16),
                     confirmado = ret.Field<Boolean>(17),
                     TotalBaseImponible = ret.Field<Single>(18),
                     TotalBaseGrava = ret.Field<Single>(19),
                     montoIVA = ret.Field<Single>(20),
                 }).ToList();

            var retenciones_detalle =
                from ret in retenciones
                    //.Where(x => x.secuencial == "112008461")
                join det in dst.Tables["Detalle_Retenciones"].AsEnumerable()
                on ret.doc_id_corp equals
                    det.Field<string>(6) into ret_
                from det in ret_.DefaultIfEmpty()
                select new
                {
                    ret.idProv,
                    ret.fechaRegistro,
                    ret.establecimiento,
                    ret.puntoEmision,
                    ret.secuencial,
                    ret.codRetAir,
                    ret.baseImpAir,
                    ret.porcentajeAir,
                    ret.valRetAir,
                    ret.doc_id_corp,
                    ret.tipo_de_campo,
                    ret.estabRet,
                    ret.puntoEmiRet,
                    ret.secuencialRet,
                    codigo_acceso = ret.codigo_acceso.Trim('\r', '\n'),
                    codSustento = det == null ? "" : det.Field<string>(0),
                    tipoComprobante = det == null ? "" : det.Field<string>(1),
                    autorizacion = det == null ? "" : det.Field<string>(2),
                    baseImponible = det == null ? 0 : det.Field<Single>(3),
                    baseImpGrav = det == null ? 0 : det.Field<Single>(4),
                    //montoIva = det == null ? 0 : det.Field<Single>(5),
                    proveedor = det == null ? "" : det.Field<string>(7),
                    montoIce = det == null || det.Field<string>(8) == "" ? 0 : Convert.ToDecimal(det.Field<string>(8)),
                    ret.anulada,
                    ret.confirmado,
                    ret.TotalBaseGrava,
                    ret.TotalBaseImponible,
                    ret.montoIVA
                };


            var retencion_totales =
                from ret in retenciones_detalle
                join det in dst.Tables["detalle_proveedor"].AsEnumerable()
                on ret.proveedor equals
                    det.Field<string>(0) into ret_
                from det in ret_.DefaultIfEmpty()
                    //where ret.secuencialRet != ""
                select new
                {
                    ret.idProv,
                    ret.fechaRegistro,
                    ret.establecimiento,
                    ret.puntoEmision,
                    ret.secuencial,
                    codRetAir = ret.codRetAir.Trim(),
                    ret.baseImpAir,
                    ret.porcentajeAir,
                    ret.valRetAir,
                    ret.doc_id_corp,
                    ret.tipo_de_campo,
                    estabRet = ret.estabRet=="00"?"001":ret.estabRet,
                    puntoEmiRet = ret.puntoEmiRet == "" ? "001" : ret.puntoEmiRet,
                    secuencialRet = ret.secuencialRet == "" ? "ERROR" : ret.secuencialRet,
                    codigo_acceso = ret.codigo_acceso.Trim('\r', '\n'),
                    ret.codSustento,
                    ret.tipoComprobante,
                    autorizacion = ret.autorizacion == "" ? "ERROR" : ret.autorizacion.Trim('\r', '\n'),
                    ret.baseImponible,
                    ret.baseImpGrav,
                    //ret.montoIva,
                    ret.proveedor,
                    ret.montoIce,
                    tpidprov = det == null ? "" : det.Field<string>(1),
                    ret.anulada,
                    ret.confirmado,
                    ret.TotalBaseGrava,
                    ret.TotalBaseImponible,
                    ret.montoIVA
                };


            List<DetalleCompras> grupo_retenciones = new List<DetalleCompras>();


            //elem_venta.PuntoEmision = vent.puntoEmiRet;
            //elem_venta.SecuencialInicio = vent.secuencialRet;
            //elem_venta.SecuencialFin = vent.secuencialRet;
            //elem_venta.Autorizacion = vent.codigo_acceso;
            var retenciones_totales_confirmadas = retencion_totales.Where(x => x.confirmado == true && x.anulada == false);
            var retenciones_totales_anuladas = retencion_totales.GroupBy(x => new {
                x.puntoEmiRet,
                x.secuencialRet,
                x.codigo_acceso,
                x.anulada
            }).Where(x => x.Key.anulada == true);

            foreach (var datos in retenciones_totales_confirmadas.GroupBy(x => new {
                x.secuencial,
                x.codSustento,
                x.tpidprov,
                x.idProv,
                x.tipoComprobante,
                x.fechaRegistro,
                x.establecimiento,
                x.puntoEmision,
                x.autorizacion,
                x.TotalBaseImponible,
                x.TotalBaseGrava,
                x.montoIVA,
                x.montoIce,
                x.estabRet,
                x.puntoEmiRet,
                x.secuencialRet,
                x.codigo_acceso
            }))
            {
                var retencion = new DetalleCompras();
                var montoIVA = datos.Key.montoIVA;
                retencion.CodSustento = datos.Key.codSustento;
                retencion.TpIdProv = datos.Key.tpidprov;
                retencion.IdProv = datos.Key.idProv;
                retencion.TipoComprobante = datos.Key.tipoComprobante;
                retencion.ParteRel = "NO";
                retencion.FechaRegistro = datos.Key.fechaRegistro.ToString("dd/MM/yyyy");
                retencion.Establecimiento = datos.Key.establecimiento;
                retencion.PuntoEmision = datos.Key.puntoEmision;
                retencion.Secuencial = datos.Key.secuencial;
                retencion.FechaEmision = datos.Key.fechaRegistro.ToString("dd/MM/yyyy");
                retencion.Autorizacion = datos.Key.autorizacion;
                retencion.BaseNoGraIva = "0.00";
                retencion.BaseImponible = string.Format("{0:0.00}", datos.Key.TotalBaseImponible);
                retencion.BaseImpGrav = string.Format("{0:0.00}", datos.Key.TotalBaseGrava);
                retencion.BaseImpExe = "0.00";
                retencion.MontoIce = string.Format("{0:0.00}", datos.Key.montoIce);
                retencion.MontoIva = string.Format("{0:0.00}", datos.Key.montoIVA);
                retencion.ValRetBien10 = "0.00";
                retencion.ValRetServ20 = "0.00";
                retencion.ValorRetBienes = "0.00";
                retencion.ValRetServ50 = "0.00";
                retencion.ValorRetServicios = "0.00";
                retencion.ValRetServ100 = "0.00";
                var porcentaje = retencion_totales.Where(x => x.idProv == datos.Key.idProv && x.puntoEmiRet + x.puntoEmision + x.secuencial == x.puntoEmiRet + x.puntoEmision + datos.Key.secuencial && x.porcentajeAir >= 10).Select(x => x.porcentajeAir).FirstOrDefault();

                //switch (datos.Key.porcentajeAir)
                switch (porcentaje)
                {
                    case 10f:
                        retencion.ValRetBien10 = string.Format("{0:0.00}", (montoIVA * 0.1));
                        break;
                    case 20f:
                        retencion.ValRetServ20 = string.Format("{0:0.00}", (montoIVA * 0.2));
                        break;
                    case 30f:
                        retencion.ValorRetBienes = string.Format("{0:0.00}", (montoIVA * 0.3));
                        break;
                    case 50f:
                        retencion.ValRetServ50 = string.Format("{0:0.00}", (montoIVA * 0.5));
                        break;
                    case 70f:
                        retencion.ValorRetServicios = string.Format("{0:0.00}", (montoIVA * 0.7));
                        break;
                    case 100f:
                        retencion.ValRetServ100 = string.Format("{0:0.00}", (montoIVA * 1));
                        break;
                }

                retencion.TotbasesImpReemb = string.Format("{0:0.00}", "0");
                PagoExterior pagoext = new PagoExterior();
                pagoext.PagoLocExt = "01";
                pagoext.PaisEfecPago = "NA";
                pagoext.AplicConvDobTrib = "NA";
                pagoext.PagExtSujRetNorLeg = "NA";
                retencion.PagoExterior = pagoext;

                if (datos.Key.TotalBaseImponible + datos.Key.montoIVA + datos.Key.TotalBaseGrava > 1000)
                {
                    retencion.FormasDePago = new FormasDePago { FormaPago = "20" };
                }

                var air = new Air();
                List<DetalleAir> objeto = new List<DetalleAir>();

                var rtn_detalle = retencion_totales.Where(x => x.idProv == datos.Key.idProv && x.puntoEmiRet + x.puntoEmision + x.secuencial == x.puntoEmiRet + x.puntoEmision + datos.Key.secuencial && x.porcentajeAir <= 10);
                var cant_rtn_detalle = rtn_detalle.Count();
                foreach (var d in rtn_detalle)
                {
                    DetalleAir det = new DetalleAir();
                    det.CodRetAir = d.codRetAir;
                    det.BaseImpAir = string.Format("{0:0.00}", cant_rtn_detalle == 1 ? d.TotalBaseGrava + d.TotalBaseImponible : d.baseImpAir);
                    det.PorcentajeAir = string.Format("{0:0.00}", d.porcentajeAir);
                    det.ValRetAir = string.Format("{0:0.00}", d.valRetAir);
                    //air.DetalleAir = det;
                    objeto.Add(det);
                };

                air.DetalleAir = objeto;



                retencion.Air = air;
                retencion.PtoEmiRetencion1 = datos.Key.puntoEmiRet;
                retencion.EstabRetencion1 = datos.Key.estabRet;
                retencion.SecRetencion1 = datos.Key.secuencialRet;
                retencion.AutRetencion1 = datos.Key.codigo_acceso;
                retencion.FechaEmiRet1 = datos.Key.fechaRegistro.ToString("dd/MM/yyyy");



                grupo_retenciones.Add(retencion);
            };

            foreach (var datos in nc_prov_totales.GroupBy(x => new {
                x.secuencial,
                x.codSustento,
                x.tpIdProv,
                x.idProv,
                x.tipoComprobante,
                x.fechaRegistro,
                x.establecimiento,
                x.puntoEmision,
                x.autorizacion,
                x.montoIva,
                x.montoIce,
                x.estabModificado,
                x.ptoEmiModificado,
                x.secModificado,
                x.autModificado,
                x.baseImpGrav,
                x.formaPago,
                x.docModificado,
            }))
            {
                var retencion = new DetalleCompras();
                var montoIVA = datos.Key.montoIva;
                retencion.CodSustento = datos.Key.codSustento;
                retencion.TpIdProv = datos.Key.tpIdProv;
                retencion.IdProv = datos.Key.idProv;
                retencion.TipoComprobante = datos.Key.tipoComprobante;
                retencion.ParteRel = "NO";
                retencion.FechaRegistro = datos.Key.fechaRegistro;
                retencion.Establecimiento = datos.Key.establecimiento;
                retencion.PuntoEmision = datos.Key.puntoEmision;
                retencion.Secuencial = datos.Key.secuencial;
                retencion.FechaEmision = datos.Key.fechaRegistro;
                retencion.Autorizacion = datos.Key.autorizacion;
                retencion.BaseNoGraIva = "0.00";
                retencion.BaseImponible = "0.00";
                retencion.BaseImpGrav = string.Format("{0:0.00}", datos.Key.baseImpGrav);
                retencion.BaseImpExe = "0.00";
                retencion.MontoIce = string.Format("{0:0.00}", datos.Key.montoIce);
                retencion.MontoIva = string.Format("{0:0.00}", datos.Key.montoIva);
                retencion.ValRetBien10 = "0.00";
                retencion.ValRetServ20 = "0.00";
                retencion.ValorRetBienes = "0.00";
                retencion.ValRetServ50 = "0.00";
                retencion.ValorRetServicios = "0.00";
                retencion.ValRetServ100 = "0.00";
                retencion.TotbasesImpReemb = string.Format("{0:0.00}", "0");
                PagoExterior pagoext = new PagoExterior();
                pagoext.PagoLocExt = "01";
                pagoext.PaisEfecPago = "NA";
                pagoext.AplicConvDobTrib = "NA";
                pagoext.PagExtSujRetNorLeg = "NA";
                retencion.PagoExterior = pagoext;
                retencion.FormasDePago = new FormasDePago { FormaPago = datos.Key.formaPago };

                retencion.DocModificado = datos.Key.docModificado;
                retencion.EstabModificado = datos.Key.estabModificado;
                retencion.PtoEmiModificado = datos.Key.ptoEmiModificado;
                retencion.SecModificado = datos.Key.secModificado;
                retencion.AutModificado = datos.Key.autModificado;

                grupo_retenciones.Add(retencion);
            };

            ats_xml.Compras = new Compras() { DetalleCompras = grupo_retenciones };

            var venta_establecimiento = new VentaEst()
            {
                CodEstab = "001",
                VentasEstab = string.Format("{0:0.00}", ats_xml.TotalVentas),
                IvaComp = "0.00"
            };

            ats_xml.VentasEstablecimiento = new VentasEstablecimiento() { VentaEst = venta_establecimiento };



            List<DetalleAnulados> anulados = new List<DetalleAnulados>();

            foreach (var vent in ventas_clientes_anuladas)
            {
                var elem_venta = new DetalleAnulados();
                elem_venta.TipoComprobante = "18";
                elem_venta.Establecimiento = "001";
                elem_venta.PuntoEmision = vent.puntoEmi;
                elem_venta.SecuencialInicio = vent.secuencial;
                elem_venta.SecuencialFin = vent.secuencial;
                elem_venta.Autorizacion = vent.codigo_acceso;
                anulados.Add(elem_venta);
            }

            foreach (var vent in nc_clientes_anuladas)
            {
                var elem_venta = new DetalleAnulados();
                elem_venta.TipoComprobante = vent.tipo == "CR" ? "04" : "05";
                elem_venta.Establecimiento = "001";
                elem_venta.PuntoEmision = vent.puntoEmi;
                elem_venta.SecuencialInicio = vent.secuencial;
                elem_venta.SecuencialFin = vent.secuencial;
                elem_venta.Autorizacion = vent.codigo_acceso;
                anulados.Add(elem_venta);
            }


            foreach (var vent in retenciones_totales_anuladas)
            {
                var elem_venta = new DetalleAnulados();
                elem_venta.TipoComprobante = "07";
                elem_venta.Establecimiento = "001";
                elem_venta.PuntoEmision = vent.Key.puntoEmiRet;
                elem_venta.SecuencialInicio = vent.Key.secuencialRet;
                elem_venta.SecuencialFin = vent.Key.secuencialRet;
                elem_venta.Autorizacion = vent.Key.codigo_acceso;
                anulados.Add(elem_venta);
            }

            ats_xml.Anulados = new Anulados() { DetalleAnulados = anulados };
            cs.cerrarConexion();
            //var serializer = new XmlSerializer(typeof(Iva));




            return ats_xml;

            //using (var stream = new StreamWriter("C:\\Users\\BVera.LABOVIDA\\Documents\\test1.xml"))
            //    serializer.Serialize(stream, ats_xml);

            //string textLocal = File.ReadAllText(@"C:\Users\BVera.LABOVIDA\Documents\test1.xml");
            //string textRemote = File.ReadAllText(@"C:\Users\BVera.LABOVIDA\Documents\original.xml");

            //var doc = new XmlDocument();
            //doc.Load(@"C:\Users\BVera.LABOVIDA\Documents\testing.xml");


        }




        private void RetFisica()
        {
            var retencion = new DetalleCompras
            {
                CodSustento = "02",
            };

            var serializer = new XmlSerializer(typeof(comprobanteRetencion));
            using (var stream = new StreamWriter("C:\\Users\\BVera.LABOVIDA\\Documents\\test.xml"))
                serializer.Serialize(stream, retencion);
        }

    }
}