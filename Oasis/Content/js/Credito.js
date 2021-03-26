
var OCDetalle = [];

function ConvertirFecha(fecha) {
    var date = fecha;
    var day = date.getDate();       // yields date
    var month = date.getMonth() + 1;    // yields month (add one as '.getMonth()' is zero indexed)
    var year = date.getFullYear();  // yields year
    var hour = date.getHours();     // yields hours
    var minute = date.getMinutes(); // yields minutes
    var second = date.getSeconds(); // yields seconds
    return day + "/" + month + "/" + year + " " + hour + ':' + minute + ':' + second; 
}

function formatoValor(nStr) {
    nStr = nStr.toFixed(2);
    nStr += '';
    var x = nStr.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

$("#GenerarPresupuesto").click(function () {
    var empresa = $("#empresa").val();
    var sucursal = $("#sucursal").val();
    var localidad;

    if ($('#chkLocalidad').is(":checked")) {
        localidad = null
    } else {
        localidad = $("#localidad").val();
    }

    var fecha_desde = ConvertirFecha($('#fecha_presupuesto').data('daterangepicker').startDate._d);
    var fecha_hasta = ConvertirFecha($('#fecha_presupuesto').data('daterangepicker').endDate._d);

    $.ajax({
        url: 'ObtenerPresupuesto',
        type: 'GET',
        data: {
            empresa: empresa,
            sucursal: sucursal,
            //localidad: this.localidad,
            fecha_desde: fecha_desde,
            fecha_hasta: fecha_hasta
        },
        dataType: "JSON",
        contentType: "application/JSON",
        success: function (d) {
            $('#contenedorTabla').remove();
            var contenedorTabla = document.createElement("div");  
            contenedorTabla.className = "col-md-12";
            contenedorTabla.id = "contenedorTabla"
            var row = document.createElement("div");      
            row.className = "row";
            row.appendChild(contenedorTabla);
            $('#contenedorPrimario').append(row);
            //$('.container-fluid').add('<div class="row"><div id="tablaPresupuesto" class="col-md-12"></div></div>');
            var sTxt = '<div class="card"><table class="table table-hover">';
            sTxt += '<thead><tr><th style="text-align:center">ID</th><th  style="text-align:center">Vendedor</th><th style="text-align:center">Cuota ventas</th>';
            sTxt += '<th style="text-align:center">Ventas</th><th style="text-align:center">%</th>';
            sTxt += '<th style="text-align:center">Cuota cobros</th><th style="text-align:center">Cobros</th>';
            sTxt += '<th style="text-align:center">%</th></tr></thead> ';
            sTxt += '<tbody>';
            $.each(JSON.parse(d), function (index, p) {
                sTxt += '<tr><td style="text-align:center">' + p.id_vendedor + '</td>';
                sTxt += '<td style="text-align:center">' + p.nombre_vendedor + '</td>';
                sTxt += '<td style="text-align:center">' + formatoValor(p.valor_venta)  + '</td>';
                sTxt += '<td style="text-align:center">' + formatoValor(p.ventas_neta) + '</td>';
                sTxt += '<td style="text-align:center">' + p.alcance_venta.toFixed(2) + '%</td>';
                sTxt += '<td style="text-align:center">' + formatoValor(p.valor_cobro) + '</td>';
                sTxt += '<td style="text-align:center">' + formatoValor(p.total_cobros) + '</td>';
                sTxt += '<td style="text-align:center">' + p.alcance_cobro.toFixed(2) + '%</td></tr>';
            });
            sTxt += '</tbody></table></div>';

            $('#contenedorTabla').append(sTxt);

            //Toast.fire({
            //    icon: 'success',
            //    title: 'Se ha generado la OC de forma correcta.'
            //})

            //if (d.length > 0) {
            //    let pdfWindow = window.open("")
            //    pdfWindow.document.write(
            //        "<iframe width='100%' height='100%' src='data:application/pdf;base64," +
            //        encodeURI(d) + "'></iframe>"
            //    )
            //    Toast.fire({
            //        icon: 'success',
            //        title: 'Se ha generado la OC de forma correcta.'
            //    })
            //}
        },
        error: function (e) {
            Toast.fire({
                icon: 'error',
                title: 'Hubo un error al intentar generar.'
            })
        }
    })

});
