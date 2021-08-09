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

$('#fecha_corte').daterangepicker({
    locale: {
        format: 'DD/MM/YYYY', 
        "daysOfWeek": [
            "Do",
            "Lu",
            "Ma",
            "Mi",
            "Ju",
            "Vi",
            "Sa"
        ],
        "monthNames": [
            "Enero",
            "Febrero",
            "Marzo",
            "Abril",
            "Mayo",
            "Junio",
            "Julio",
            "Agosto",
            "Septiembre",
            "Octubre",
            "Noviembre",
            "Diciembre"
        ],
    },
    startDate: firstDay,
    endDate: lastDay
});

$("#GenerarNC").click(function () {
    var tipoCliente = [];
    var empresa = $("#empresa_ind").val();
    var sucursal = $("#sucursal_ind").val();
    var fecha_desde = ConvertirFecha($('#fecha_corte').data('daterangepicker').startDate._d);
    var fecha_hasta = ConvertirFecha($('#fecha_corte').data('daterangepicker').endDate._d);
    var localidad;

    if ($('#chkLocalidad').is(":checked")) {
        localidad = null
    } else {
        localidad = $("#localidad_ind").val();
    }

    $('.tipoCliente:checkbox:checked').each(function () {
        tipoCliente.push($(this).attr('name'));
    });

    var eleccion_reporte_general = $('#reporte_general')[0].checked;
    let id_visitador = null;
    if (!eleccion_reporte_general) {
        id_visitador = $('.js-data-vendedor-ajax').children("option:selected").val();
    }
    iniciaLoading();
    $.ajax({
        url: 'ObtenerNC',
        type: 'GET',
        data: {
            empresa: empresa,
            sucursal: sucursal,
            tipoCliente: JSON.stringify(tipoCliente),
            visitador: id_visitador,
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
            d = JSON.parse(d);
            var col = [];
            var encabezado = ["EMPRESA", "SUCURSAL", "RUC",
                "CLIENTE",
                "FECHA NC", "SECUENCIAL NC", "MOTIVO", "VALOR",
                "FECHA FACTURA", "FACTURA", "VISITADOR",
                "NOTA","DESCRIPCIÓN"
            ];
            for (var i = 0; i < d.length; i++) {
                for (var key in d[i]) {
                    if (col.indexOf(key) === -1) {
                        col.push(key);
                    }
                }
            }

            // CREATE DYNAMIC TABLE.
            var div = document.createElement("div");
            var row = document.createElement("div");
            row.className = "row col-md-4";
            var card = document.createElement("div");
            card.className = "card";
            card.style = "font-size: 15px;overflow-x: scroll;";
            var cardbody = document.createElement("div");
            cardbody.className = "card-body";
            var table = document.createElement("table");
            table.className = 'table table-hover tableDetalle';
            table.id = "tableDetalle";
            table.style = '';

            // CREATE HTML TABLE HEADER ROW USING THE EXTRACTED HEADERS ABOVE
            var thead = document.createElement("thead");
            table.appendChild(thead);
            var tr_head = document.createElement("tr");
            for (var i = 0; i < encabezado.length; i++) {
                var th = document.createElement("th");      // TABLE HEADER.
                th.innerHTML = encabezado[i];
                tr_head.appendChild(th);
            }

            thead.appendChild(tr_head);

            var tbody = document.createElement("tbody");
            table.appendChild(tbody);
            var tr_body = document.createElement("tr");
            // ADD JSON DATA TO THE TABLE AS ROWS.
            for (var i = 0; i < d.length; i++) {
                tr_body = document.createElement("tr");
                for (var j = 0; j < col.length; j++) {
                    var tabCell = tr_body.insertCell(-1);
                    tabCell.innerHTML = d[i][col[j]];
                    tabCell.style = ' white-space: nowrap;';
                }
                tbody.appendChild(tr_body);
            }

            cardbody.appendChild(table);
            card.appendChild(cardbody);
            div.appendChild(card);
            //CrearTablaDetalle(div.outerHTML, titulo);
            $('#contenedorTabla').append(div);
            $('#tableDetalle').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'copy', 'csv', 'excel', 'pdf', 'print'
                ],
                language: {
                    url: 'https://cdn.datatables.net/plug-ins/1.10.24/i18n/Spanish.json'
                }
            });
            cierraLoading();
        },
        error: function (e) {
            cierraLoading();
            Toast.fire({
                icon: 'error',
                title: 'Hubo un error al intentar generar.'
            })
        }
    })
});