/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace Oasis.Models.ATS
{
	[XmlRoot(ElementName = "pagoExterior")]
	public class PagoExterior
	{
		[XmlElement(ElementName = "pagoLocExt")]
		public string PagoLocExt { get; set; }
		[XmlElement(ElementName = "paisEfecPago")]
		public string PaisEfecPago { get; set; }
		[XmlElement(ElementName = "aplicConvDobTrib")]
		public string AplicConvDobTrib { get; set; }
		[XmlElement(ElementName = "pagExtSujRetNorLeg")]
		public string PagExtSujRetNorLeg { get; set; }
	}

	[XmlRoot(ElementName = "detalleAir")]
	public class DetalleAir
	{
		[XmlElement(ElementName = "codRetAir")]
		public string CodRetAir { get; set; }
		[XmlElement(ElementName = "baseImpAir")]
		public string BaseImpAir { get; set; }
		[XmlElement(ElementName = "porcentajeAir")]
		public string PorcentajeAir { get; set; }
		[XmlElement(ElementName = "valRetAir")]
		public string ValRetAir { get; set; }
	}

	[XmlRoot(ElementName = "air")]
	public class Air
	{
		[XmlElement(ElementName = "detalleAir")]
		public List<DetalleAir> DetalleAir { get; set; }
	}

	[XmlRoot(ElementName = "detalleCompras")]
	public class DetalleCompras
	{
		[XmlElement(ElementName = "codSustento")]
		public string CodSustento { get; set; }
		[XmlElement(ElementName = "tpIdProv")]
		public string TpIdProv { get; set; }
		[XmlElement(ElementName = "idProv")]
		public string IdProv { get; set; }
		[XmlElement(ElementName = "tipoComprobante")]
		public string TipoComprobante { get; set; }
		[XmlElement(ElementName = "parteRel")]
		public string ParteRel { get; set; }
		[XmlElement(ElementName = "fechaRegistro")]
		public string FechaRegistro { get; set; }
		[XmlElement(ElementName = "establecimiento")]
		public string Establecimiento { get; set; }
		[XmlElement(ElementName = "puntoEmision")]
		public string PuntoEmision { get; set; }
		[XmlElement(ElementName = "secuencial")]
		public string Secuencial { get; set; }
		[XmlElement(ElementName = "fechaEmision")]
		public string FechaEmision { get; set; }
		[XmlElement(ElementName = "autorizacion")]
		public string Autorizacion { get; set; }
		[XmlElement(ElementName = "baseNoGraIva")]
		public string BaseNoGraIva { get; set; }
		[XmlElement(ElementName = "baseImponible")]
		public string BaseImponible { get; set; }
		[XmlElement(ElementName = "baseImpGrav")]
		public string BaseImpGrav { get; set; }
		[XmlElement(ElementName = "baseImpExe")]
		public string BaseImpExe { get; set; }
		[XmlElement(ElementName = "montoIce")]
		public string MontoIce { get; set; }
		[XmlElement(ElementName = "montoIva")]
		public string MontoIva { get; set; }
		[XmlElement(ElementName = "valRetBien10")]
		public string ValRetBien10 { get; set; }
		[XmlElement(ElementName = "valRetServ20")]
		public string ValRetServ20 { get; set; }
		[XmlElement(ElementName = "valorRetBienes")]
		public string ValorRetBienes { get; set; }
		[XmlElement(ElementName = "valRetServ50")]
		public string ValRetServ50 { get; set; }
		[XmlElement(ElementName = "valorRetServicios")]
		public string ValorRetServicios { get; set; }
		[XmlElement(ElementName = "valRetServ100")]
		public string ValRetServ100 { get; set; }
		[XmlElement(ElementName = "totbasesImpReemb")]
		public string TotbasesImpReemb { get; set; }
		[XmlElement(ElementName = "pagoExterior")]
		public PagoExterior PagoExterior { get; set; }
		[XmlElement(ElementName = "formasDePago")]
		public FormasDePago FormasDePago { get; set; }
		[XmlElement(ElementName = "air")]
		public Air Air { get; set; }
		[XmlElement(ElementName = "estabRetencion1")]
		public string EstabRetencion1 { get; set; }
		[XmlElement(ElementName = "ptoEmiRetencion1")]
		public string PtoEmiRetencion1 { get; set; }
		[XmlElement(ElementName = "secRetencion1")]
		public string SecRetencion1 { get; set; }
		[XmlElement(ElementName = "autRetencion1")]
		public string AutRetencion1 { get; set; }
		[XmlElement(ElementName = "fechaEmiRet1")]
		public string FechaEmiRet1 { get; set; }
		[XmlElement(ElementName = "docModificado")]
		public string DocModificado { get; set; }
		[XmlElement(ElementName = "estabModificado")]
		public string EstabModificado { get; set; }
		[XmlElement(ElementName = "ptoEmiModificado")]
		public string PtoEmiModificado { get; set; }
		[XmlElement(ElementName = "secModificado")]
		public string SecModificado { get; set; }
		[XmlElement(ElementName = "autModificado")]
		public string AutModificado { get; set; }

	}

	[XmlRoot(ElementName = "formasDePago")]
	public class FormasDePago
	{
		[XmlElement(ElementName = "formaPago")]
		public string FormaPago { get; set; }
	}

	[XmlRoot(ElementName = "compras")]
	public class Compras
	{
		[XmlElement(ElementName = "detalleCompras")]
		public List<DetalleCompras> DetalleCompras { get; set; }
	}

	[XmlRoot(ElementName = "detalleVentas")]
	public class DetalleVentas
	{
		[XmlElement(ElementName = "tpIdCliente")]
		public string TpIdCliente { get; set; }
		[XmlElement(ElementName = "idCliente")]
		public string IdCliente { get; set; }
		[XmlElement(ElementName = "parteRelVtas")]
		public string ParteRelVtas { get; set; }
		[XmlElement(ElementName = "tipoComprobante")]
		public string TipoComprobante { get; set; }
		[XmlElement(ElementName = "tipoEmision")]
		public string TipoEmision { get; set; }
		[XmlElement(ElementName = "numeroComprobantes")]
		public string NumeroComprobantes { get; set; }
		[XmlElement(ElementName = "baseNoGraIva")]
		public string BaseNoGraIva { get; set; }
		[XmlElement(ElementName = "baseImponible")]
		public string BaseImponible { get; set; }
		[XmlElement(ElementName = "baseImpGrav")]
		public string BaseImpGrav { get; set; }
		[XmlElement(ElementName = "montoIva")]
		public string MontoIva { get; set; }
		[XmlElement(ElementName = "montoIce")]
		public string MontoIce { get; set; }
		[XmlElement(ElementName = "valorRetIva")]
		public string ValorRetIva { get; set; }
		[XmlElement(ElementName = "valorRetRenta")]
		public string ValorRetRenta { get; set; }
		[XmlElement(ElementName = "formasDePago")]
		public FormasDePago FormasDePago { get; set; }
		[XmlElement(ElementName = "docModificado")]
		public string DocModificado { get; set; }
		[XmlElement(ElementName = "estabModificado")]
		public string EstabModificado { get; set; }
		[XmlElement(ElementName = "ptoEmiModificado")]
		public string PtoEmiModificado { get; set; }
		[XmlElement(ElementName = "secModificado")]
		public string SecModificado { get; set; }
		[XmlElement(ElementName = "autModificado")]
		public string AutModificado { get; set; }
	}

	[XmlRoot(ElementName = "ventas")]
	public class Ventas
	{
		[XmlElement(ElementName = "detalleVentas")]
		public List<DetalleVentas> DetalleVentas { get; set; }
	}

	[XmlRoot(ElementName = "ventaEst")]
	public class VentaEst
	{
		[XmlElement(ElementName = "codEstab")]
		public string CodEstab { get; set; }
		[XmlElement(ElementName = "ventasEstab")]
		public string VentasEstab { get; set; }
		[XmlElement(ElementName = "ivaComp")]
		public string IvaComp { get; set; }
	}

	[XmlRoot(ElementName = "ventasEstablecimiento")]
	public class VentasEstablecimiento
	{
		[XmlElement(ElementName = "ventaEst")]
		public VentaEst VentaEst { get; set; }
	}

	[XmlRoot(ElementName = "detalleAnulados")]
	public class DetalleAnulados
	{
		[XmlElement(ElementName = "tipoComprobante")]
		public string TipoComprobante { get; set; }
		[XmlElement(ElementName = "establecimiento")]
		public string Establecimiento { get; set; }
		[XmlElement(ElementName = "puntoEmision")]
		public string PuntoEmision { get; set; }
		[XmlElement(ElementName = "secuencialInicio")]
		public string SecuencialInicio { get; set; }
		[XmlElement(ElementName = "secuencialFin")]
		public string SecuencialFin { get; set; }
		[XmlElement(ElementName = "autorizacion")]
		public string Autorizacion { get; set; }
	}

	[XmlRoot(ElementName = "anulados")]
	public class Anulados
	{
		[XmlElement(ElementName = "detalleAnulados")]
		public List<DetalleAnulados> DetalleAnulados { get; set; }
	}

	[XmlRoot(ElementName = "iva")]
	public class Iva
	{
		[XmlElement(ElementName = "TipoIDInformante")]
		public string TipoIDInformante { get; set; }
		[XmlElement(ElementName = "IdInformante")]
		public string IdInformante { get; set; }
		[XmlElement(ElementName = "razonSocial")]
		public string RazonSocial { get; set; }
		[XmlElement(ElementName = "Anio")]
		public string Anio { get; set; }
		[XmlElement(ElementName = "Mes")]
		public string Mes { get; set; }
		[XmlElement(ElementName = "numEstabRuc")]
		public string NumEstabRuc { get; set; }
		[XmlElement(ElementName = "totalVentas")]
		public string TotalVentas { get; set; }
		[XmlElement(ElementName = "codigoOperativo")]
		public string CodigoOperativo { get; set; }
		[XmlElement(ElementName = "compras")]
		public Compras Compras { get; set; }
		[XmlElement(ElementName = "ventas")]
		public Ventas Ventas { get; set; }
		[XmlElement(ElementName = "ventasEstablecimiento")]
		public VentasEstablecimiento VentasEstablecimiento { get; set; }
		[XmlElement(ElementName = "anulados")]
		public Anulados Anulados { get; set; }
	}

}