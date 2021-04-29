
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

function CrearTablaDetalle(datos, titulo) {
    Swal.fire({
        title: '<strong>'+titulo+'</strong>',
        icon: 'info',
        width: 1100,
        html: datos,
        customClass: 'swal-wide',
        showCloseButton: true,
        focusConfirm: false,
        confirmButtonText:
            '<i class="fa fa-thumbs-up"></i> Ok!'
    })
}

function buildHtmlTable(selector) {
    var columns = addAllColumnHeaders(myList, selector);

    for (var i = 0; i < myList.length; i++) {
        var row$ = $('<tr/>');
        for (var colIndex = 0; colIndex < columns.length; colIndex++) {
            var cellValue = myList[i][columns[colIndex]];
            if (cellValue == null) cellValue = "";
            row$.append($('<td/>').html(cellValue));
        }
        $(selector).append(row$);
    }
}

$(".GenerarExcelDetalle").click(function () {
    $("#tableDetalle").table2excel({
        exclude: ".noExl",
        name: "Detalle OASIS",
        filename: "Detalle",//do not include extension
        //fileext: ".xlsx", // file extension
        exclude_img: true,
        exclude_links: true,
        exclude_inputs: true,
        preserveColors: true
    });
});

$("#GenerarExcel").click(function () {
    $("#tablePresupuesto").table2excel({
    exclude: ".noExl",
    name: "Consolidado OASIS",
    filename: "Consolidado",//do not include extension
    //fileext: ".xlsx", // file extension
    exclude_img: true,
    exclude_links: true,
    exclude_inputs: true,
    preserveColors: true
    });
});

function addAllColumnHeaders(myList, selector) {
    var columnSet = [];
    var headerTr$ = $('<tr/>');

    for (var i = 0; i < myList.length; i++) {
        var rowHash = myList[i];
        for (var key in rowHash) {
            if ($.inArray(key, columnSet) == -1) {
                columnSet.push(key);
                headerTr$.append($('<th/>').html(key));
            }
        }
    }
    $(selector).append(headerTr$);

    return columnSet;
}

function GenerarDatos(id_vendedor, direccionURL, titulo) {
    let spanOculto;
    spanOculto = $('.table-hover tbody tr td span.datosLinea[data-id_vendedor="' + id_vendedor + '"]')[0].dataset;

    $.ajax({
        url: direccionURL,
        type: 'GET',
        data: {
            empresa: spanOculto.empresa,
            sucursal: spanOculto.sucursal,
            fecha_desde: spanOculto.fecha_desde,
            fecha_hasta: spanOculto.fecha_hasta,
            tipoCliente: JSON.stringify(spanOculto.tipocliente),
            vendedor: spanOculto.id_vendedor
        },
        dataType: "JSON",
        contentType: "application/JSON",
        success: function (d) {
            d = JSON.parse(d);
            var col = [];
            for (var i = 0; i < d.length; i++) {
                for (var key in d[i]) {
                    if (col.indexOf(key) === -1) {
                        col.push(key);
                    }
                }
            }

            // CREATE DYNAMIC TABLE.
            var div = document.createElement("div");
            div.className = "col-md-12";
            //var btnDescargar = document.createElement("button");
            //btnDescargar.className = "btn bg-gradient-success float-left GenerarExcelDetalle";
            //btnDescargar.id = "GenerarExcelDetalle";
            //btnDescargar.onclick = "";
            //btnDescargar.innerText="Descargar"
            var row = document.createElement("div");
            row.className = "row col-md-4";
            var card = document.createElement("div");
            card.className = "card";
            card.style = "font-size: 15px;overflow-x: scroll;";
            var table = document.createElement("table");
            table.className = 'table table-hover tableDetalle';
            table.id = "tableDetalle";
            table.style = '';

            // CREATE HTML TABLE HEADER ROW USING THE EXTRACTED HEADERS ABOVE
            var thead = document.createElement("thead");
            table.appendChild(thead);
            var tr_head = document.createElement("tr");
            for (var i = 0; i < col.length; i++) {
                var th = document.createElement("th");      // TABLE HEADER.
                th.innerHTML = col[i];
                tr_head.appendChild(th);
            }

            thead.appendChild(tr_head);

            var tbody = document.createElement("tbody");
            table.appendChild(tbody);
            //thead.appendChild(tr);
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

            //row.appendChild(btnDescargar)
            card.appendChild(table);
            //div.appendChild(row);
            div.appendChild(card);
            CrearTablaDetalle(div.outerHTML, titulo);
            $('#tableDetalle').DataTable({
                //"dom": '<"top"i>rt<"bottom"flp><"clear">',
                dom: 'Bfrtip',
                //"paging": true,
                //"ordering": true,
                //"info": true,
                buttons: [
                    'copy', 'csv', 'excel', 'pdf', 'print'
                ],
                language: {
                    url: 'https://cdn.datatables.net/plug-ins/1.10.24/i18n/Spanish.json'
                }
            });
        },
        error: function (e) {
            Toast.fire({
                icon: 'error',
                title: 'Hubo un error al intentar generar.'
            })
        }
    })

}

function verVentasXVendedor(id_vendedor) {
    event.preventDefault();
    let direccionUrl = 'ObtenerVentasPorVendedor';
    GenerarDatos(id_vendedor, direccionUrl, "Detalle de ventas");
}

function verNCXVendedor(id_vendedor) {
    event.preventDefault();
    let direccionUrl = 'ObtenerNCPorVendedor';
    GenerarDatos(id_vendedor, direccionUrl, "Detalle de NC");
}

function verCobrosXVendedor(id_vendedor) {
    event.preventDefault();
    let direccionUrl = 'ObtenerCobrosPorVendedor';
    GenerarDatos(id_vendedor, direccionUrl, "Detalle de cobros");
}

$("botonVerDetalle").click(function () {
    event.preventDefault();
})

$("#GenerarCartera").click(function () {
    var tipoCliente = [];
    var empresa = $("#empresa_ind").val();
    var sucursal = $("#sucursal_ind").val();
    var localidad;

    if ($('#chkLocalidad').is(":checked")) {
        localidad = null
    } else {
        localidad = $("#localidad_ind").val();
    }

    $('.tipoCliente:checkbox:checked').each(function () {
        tipoCliente.push($(this).attr('name'));
    });

    var eleccion_cartera_general = $('#cartera_general')[0].checked;

    if (eleccion_cartera_general) {
        $.ajax({
            url: 'ObtenerCartera',
            type: 'GET',
            data: {
                empresa: empresa,
                sucursal: sucursal,
                //localidad: this.localidad,
                tipoCliente: JSON.stringify(tipoCliente)
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
                var encabezado = ['EMPRESA', 'SUCURSAL',
                    'RUC', 'CLIENTE', 'CATEGORIA',
                    'VENDEDOR EN CLIENTE', 'VENDEDOR EN FACTURA',
                    'SECUENCIAL',
                    'FECHA FACTURA', 'FECHA VENCIMIENTO',
                    'PROVINCIA', 'CIUDAD', 'PARROQUIA', 'DIRECCION',
                    'VALOR FACTURA', 'CHQ. POST.', 'SALDO PENDIENTE',
                    'DIAS EMITIDAS', 'DIAS VENCIDA'];
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
            },
            error: function (e) {
                Toast.fire({
                    icon: 'error',
                    title: 'Hubo un error al intentar generar.'
                })
            }
        })
    } else {
        var id_visitador = $('.js-data-vendedor-ajax').children("option:selected").val();
        $.ajax({
            url: 'ObtenerCarteraVisitador',
            type: 'GET',
            data: {
                empresa: empresa,
                sucursal: sucursal,
                //localidad: this.localidad,
                tipoCliente: JSON.stringify(tipoCliente),
                visitador: id_visitador
            },
            dataType: "JSON",
            contentType: "application/JSON",
            success: function (d) {
                $('#contenedorTabla').remove();
                var contenedorTabla = document.createElement("div");
                contenedorTabla.className = "col-md-12";
                contenedorTabla.id = "contenedorTabla"
                var row = document.createElement("div");
                row.className = " row";
                row.appendChild(contenedorTabla);
                $('#contenedorPrimario').append(row);
                d = JSON.parse(d);
                var col = [];
                var encabezado = ['EMPRESA', 'SUCURSAL',
                    'RUC', 'CLIENTE', 'CATEGORIA',
                    'VENDEDOR EN CLIENTE', 'VENDEDOR EN FACTURA',
                    'SECUENCIAL',
                    'FECHA FACTURA', 'FECHA VENCIMIENTO',
                    'PROVINCIA', 'CIUDAD', 'PARROQUIA', 'DIRECCION',
                    'VALOR FACTURA', 'CHQ. POST.', 'SALDO PENDIENTE',
                    'DIAS EMITIDAS', 'DIAS VENCIDA'];
                for (var i = 0; i < d.length; i++) {
                    for (var key in d[i]) {
                        if (col.indexOf(key) === -1) {
                            col.push(key);
                        }
                    }
                }

                // CREATE DYNAMIC TABLE.
                var div = document.createElement("div");
                //div.className = "col-md-12"; 
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
            },
            error: function (e) {
                Toast.fire({
                    icon: 'error',
                    title: 'Hubo un error al intentar generar.'
                })
            }
        })
    }
});

function GraficoBarras(identificador,tipoGrafico,presupuesto) {
    var ctx = document.getElementById(identificador).getContext('2d');

    const dataLength = presupuesto.length;
    /* Create color array */
    const colorScale = d3.interpolateInferno;

    const colorRangeInfo = {
        colorStart: 0.1,
        colorEnd: 1,
        useEndAsStart: false,
    };
    var COLORS = interpolateColors(dataLength, colorScale, colorRangeInfo);
    var myChart = new Chart(ctx, {
        type: tipoGrafico,
        data: {
            labels: presupuesto.map(presupuesto => presupuesto.nombre_vendedor),
            datasets: [{
                label: 'Ventas',
                data: presupuesto.map(presupuesto => presupuesto.ventas_neta),
                backgroundColor: COLORS,
                hoverBackgroundColor: COLORS,
                parsing: {
                    yAxisKey: 'ventas_neta'
                },
                borderWidth: 1
            },
            {
                label: 'Cobros',
                data: presupuesto.map(presupuesto => presupuesto.total_cobros),
                backgroundColor: COLORS,
                hoverBackgroundColor: COLORS,
                parsing: {
                    yAxisKey: 'total_cobros'
                },
                borderWidth: 1
            }]
        },
        options: {
            //responsive: false,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}

function calculatePoint(i, intervalSize, colorRangeInfo) {
    var { colorStart, colorEnd, useEndAsStart } = colorRangeInfo;
    return (useEndAsStart
        ? (colorEnd - (i * intervalSize))
        : (colorStart + (i * intervalSize)));
}
/* Must use an interpolated color scale, which has a range of [0, 1] */
function interpolateColors(dataLength, colorScale, colorRangeInfo) {
    var { colorStart, colorEnd } = colorRangeInfo;
    var colorRange = colorEnd - colorStart;
    var intervalSize = colorRange / dataLength;
    var i, colorPoint;
    var colorArray = [];

    for (i = 0; i < dataLength; i++) {
        colorPoint = calculatePoint(i, intervalSize, colorRangeInfo);
        colorArray.push(colorScale(colorPoint));
    }

    return colorArray;
}

function GeneraGraficos(presupuesto) {
    $('#rowcontenedorGraficos').remove();
    var elementos = $.parseHTML('<h3 class="card-title"><i class="fas fa-th mr-1"></i>  Totales                </h3>        <div class="card-tools">            <button type="button" class="btn bg-defaultbtn-sm" data-card-widget="collapse">                <i class="fas fa-minus"></i>            </button>            <button type="button" class="btn bg-defaultbtn-sm" data-card-widget="remove">                <i class="fas fa-times"></i>            </button>');
    var canva = document.createElement("canvas");
    canva.id = "contenedorBarra";
    canva.width = 400;
    canva.height = 200;
    var canva2 = document.createElement("canvas");
    canva2.id = "contenedorPastel";
    canva2.width = 400;
    canva2.height = 200;
    var cardHeader = document.createElement("div");
    cardHeader.className = "card-header border-0";
    cardHeader.appendChild(elementos[0]);
    cardHeader.appendChild(elementos[1]);
    cardHeader.appendChild(elementos[2]);
    var cardHeader2 = document.createElement("div");
    cardHeader2.className = "card-header border-0";
    cardHeader2.appendChild(elementos[0]);
    cardHeader2.appendChild(elementos[1]);
    cardHeader2.appendChild(elementos[2]);
    var cardBody = document.createElement("div");
    cardBody.className = "card-body";
    cardBody.appendChild(canva);
    var cardBody2 = document.createElement("div");
    cardBody2.className = "card-body";
    cardBody2.appendChild(canva2);
    var cardBarra = document.createElement("div");
    cardBarra.className = "card";
    cardBarra.appendChild(cardHeader);
    cardBarra.appendChild(cardBody);
    var cardPastel = document.createElement("div");
    cardPastel.className = "card";
    cardPastel.appendChild(cardHeader2);
    cardPastel.appendChild(cardBody2);
    var contenedorGraficos2 = document.createElement("div");
    contenedorGraficos2.className = "col-md-12";
    contenedorGraficos2.appendChild(cardPastel)
    var contenedorGraficos = document.createElement("div");
    contenedorGraficos.className = "col-md-12";
    contenedorGraficos.appendChild(cardBarra);
    contenedorGraficos2.appendChild(cardPastel);
    var row2 = document.createElement("div");
    row2.className = "row";
    row2.appendChild(contenedorGraficos);
    row2.appendChild(contenedorGraficos2);
    row2.id = "rowcontenedorGraficos";
    $('#contenedorPrimario').append(row2);
    GraficoBarras('contenedorBarra', 'bar',presupuesto);
    GraficoBarras('contenedorPastel', 'pie',presupuesto);
}


$("#GenerarPresupuesto").click(function () {
    var tipoCliente = [];
    var empresa = $("#empresa").val();
    var sucursal = $("#sucursal").val();
    var localidad;
    if ($('#chkLocalidad').is(":checked")) {
        localidad = null
    } else {
        localidad = $("#localidad").val();
    }
    $('.tipoCliente:checkbox:checked').each(function () {
        tipoCliente.push($(this).attr('name'));
    });
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
            fecha_hasta: fecha_hasta,
            tipoCliente: JSON.stringify(tipoCliente)
        },
        dataType: "JSON",
        contentType: "application/JSON",
        success: function (d) {

            $('#rowcontenedorTabla').remove();
            var contenedorTabla = document.createElement("div");  
            contenedorTabla.className = "col-md-12";
            contenedorTabla.id = "contenedorTabla"
            var row = document.createElement("div");      
            row.className = "row";
            row.appendChild(contenedorTabla);
            row.id = "rowcontenedorTabla";
            $('#contenedorPrimario').append(row);

            var div = document.createElement("div");
            var row = document.createElement("div");
            row.className = "row col-md-4";
            var card = document.createElement("div");
            card.className = "card";
            card.style = "font-size: 15px;overflow-x: scroll;";
            var cardbody = document.createElement("div");
            cardbody.className = "card-body";
            //var table = document.createElement("table");
            //table.className = 'table table-hover tableDetalle';
            //table.id = "tablaPresupuesto";
            //table.style = '';
            //$('.container-fluid').add('<div class="row"><div id="tablaPresupuesto" class="col-md-12"></div></div>');
            var totalPresupuestoVentas = 0;
            var totalVentas = 0;
            var totalAlcanceVentas = 0;
            var totalPresupuestoCobros = 0;
            var totalCobros = 0;
            var totalAlcanceCobros = 0;


            var sTxt = '<table class="table table-hover" id="tablePresupuesto">';
            sTxt += '<thead><tr><th style="text-align:center">ID</th><th  style="text-align:center">Vendedor</th><th style="text-align:center">Cuota ventas</th>';
            sTxt += '<th style="text-align:center">Ventas</th><th style="text-align:center">%</th>';
            sTxt += '<th style="text-align:center">Cuota cobros</th><th style="text-align:center">Cobros</th>';
            sTxt += '<th style="text-align:center">%</th><th style="text-align:center"></th></tr></thead> ';
            sTxt += '<tbody>';
            $.each(JSON.parse(d), function (index, p) {
                sTxt += '<tr>';
                sTxt += '<td style="text-align:center">';
                sTxt += '<span class="datosLinea" data-id_vendedor=' + p.id_vendedor + ' data-empresa="' + empresa+'" data-sucursal="'+sucursal+'" data-fecha_desde="'+fecha_desde+'" data-fecha_hasta="'+fecha_hasta+'" data-tipocliente="'+tipoCliente+'" hidden></span>';
                sTxt += '' + p.id_vendedor + '</td>';
                sTxt += '<td style="text-align:center">' + p.nombre_vendedor + '</td>';
                sTxt += '<td style="text-align:center">' + formatoValor(p.valor_venta)  + '</td>';
                sTxt += '<td style="text-align:center">' + formatoValor(p.ventas_neta) + '</td>';
                sTxt += '<td style="text-align:center">' + p.alcance_venta.toFixed(2) + '%</td>';
                sTxt += '<td style="text-align:center">' + formatoValor(p.valor_cobro) + '</td>';
                sTxt += '<td style="text-align:center">' + formatoValor(p.total_cobros) + '</td>';
                sTxt += '<td style="text-align:center">' + p.alcance_cobro.toFixed(2) + '%</td>';
                sTxt += '<td style="text-align:center" class="noExl"> <div class="btn-group">';
                sTxt += '<button type="button" class="btn btn-info" data-toggle="dropdown" id="botonVerDetalle"><i class="fas fa-search"></i></button>';
                sTxt += '    <button type = "button" class="btn btn-info dropdown-toggle dropdown-hover dropdown-icon" data-toggle="dropdown" aria-expanded="false"> ';
                sTxt += '    <span class="sr-only"> Toggle Dropdown</span> ';
                sTxt += '</button> ';
                sTxt += '    <div class="dropdown-menu" role = "menu" style = ""> ';
                sTxt += '    <a class="dropdown-item" href = "#" onClick="verVentasXVendedor('+p.id_vendedor+');" > Ventas </a> ';
                sTxt += '    <a class="dropdown-item" href = "#" onClick="verCobrosXVendedor(' + p.id_vendedor +');" > Cobros </a> ';
                sTxt += '    <a class="dropdown-item" href = "#" onClick="verNCXVendedor(' + p.id_vendedor +');"> N/C</a> '; 
                sTxt += '</div></td>';
                sTxt += '</tr> ';
                totalPresupuestoVentas += p.valor_venta;
                totalVentas += p.ventas_neta;
                totalPresupuestoCobros += p.valor_cobro;
                totalCobros += p.total_cobros;
            });
            sTxt += '</tbody>';
            sTxt += '<tfoot style="font-weight: 800;"><tr><td></td><td class="centrar" >TOTAL</td><td class="centrar">' + formatoValor(totalPresupuestoVentas)+ '</td>';
            sTxt += '<td class="centrar">' + formatoValor(totalVentas) + '</td>';
            sTxt += '<td class="centrar">' + ((totalVentas / totalPresupuestoVentas) * 100).toFixed(2) + '%</td>';
            sTxt += '<td class="centrar">' + formatoValor(totalPresupuestoCobros) + '</td>';
            sTxt += '<td class="centrar">' + formatoValor(totalCobros) + '</td>';
            sTxt += '<td class="centrar">' + ((totalCobros / totalPresupuestoCobros) * 100).toFixed(2) + '%</td>';
            sTxt += '<td class="centrar"></td>';
            sTxt += '</tr></tfoot>';
            sTxt += '</table>';

            cardbody.append($.parseHTML(sTxt)[0]);
            card.appendChild(cardbody);
            div.appendChild(card);
            $('#contenedorTabla').append(div);
            $('#tablePresupuesto').DataTable({
                "processing": true, // for show progress bar
                "paging": false,
                "bInfo": false,
                //"scrollY": "500px",
                dom: 'Bfrtip',
                "buttons": [
                    'copy', 'csv', 'excel', 'pdf', 'print'
                    //{ "extend": 'copy', "text": 'Copiar', "className": 'btn btn-default btn-xs' },
                    //{ "extend": 'pdf', "text": 'PDF', "className": 'btn btn-default btn-xs' },
                    //{ "extend": 'excel', "text": 'Excel', "className": 'btn btn-default btn-xs' },
                    //{ "extend": 'print', "text": 'Imprimir', "className": 'btn btn-default btn-xs' },
                ],
                initComplete: function () {
                    $('.buttons-pdf').html('<i class="far fa-file-excel"></i>')
                },
                //buttons: [
                //    'copy', 'csv', 'excel', 'pdf', 'print'
                //],
                language: {
                    url: 'https://cdn.datatables.net/plug-ins/1.10.24/i18n/Spanish.json'
                }
            });
            var presupuesto = JSON.parse(d);
            var vendedores = presupuesto.map(presupuesto => presupuesto.nombre_vendedor);
            var ventas = presupuesto.map(presupuesto => presupuesto.ventas_neta);
            var cobros = presupuesto.map(presupuesto => presupuesto.total_cobros);
            GeneraGraficos(presupuesto);
        },
        error: function (e) {
            Toast.fire({
                icon: 'error',
                title: 'Hubo un error al intentar generar.'
            })
        }
    })

    
});
