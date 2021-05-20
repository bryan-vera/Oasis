
$('#fecha_desde_presupuesto').datetimepicker({
    locale: 'es',
    format: 'DD/MM/yyyy'
});

$('#fecha_hasta_presupuesto').datetimepicker({
    locale: 'es',
    format: 'DD/MM/yyyy'
});


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
        title: '<strong>' + titulo + '</strong>',
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

function verDetallePresupuesto(id_presupuesto) {
    event.preventDefault();
    let direccionUrl = 'ObtenerDetallePresupuesto';
    GenerarDatos(id_presupuesto, direccionUrl, "Detalle de presupuesto");
}

function ActivarVendedores() {
    $('.js-data-vendedor-ajax').select2({
        //selectOnClose: true,
        minimumInputLength: 2,
        language: {
            inputTooShort: function () { return "Ingresar dos o más caracteres"; }
        },
        tags: [],
        ajax: {
            url: '/Presupuesto/ObtenerVendedores',
            processResults: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            text: item.username,
                            id: item.id_vendedor
                        }
                    })
                };
            },
            dataType: 'json',
            data: function (params) {
                var query = {
                    textoBusqueda: params.term,
                    empresa: $('#empresa').val(),
                    sucursal: $('#sucursal').val()
                }
                return query;
            }
        }
    }).on('change', function (e) {
        var id_vendedor = $(this).select2('data')[0].id;
        $(e.target).parent().parent().find($('input#id_vendedor')).val(id_vendedor);
        //$(e.target).closest('#id_vendedor').val(id_vendedor);
    });
}




$('.js-data-vendedor-ajax').on('select2:select', function (e) {
    var id_vendedor = $(this).select2('data')[0].id_vendedor;
    $(e.target).closest('#id_vendedor').val(id_vendedor);
});

$("#GenerarCartera").click(function () {
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
            //div.className = "col-md-12"; 
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

            card.appendChild(table);
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

});



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

            //$('#contenedorTabla').remove();
            //var contenedorTabla = document.createElement("div");
            //contenedorTabla.className = "col-md-12";
            //contenedorTabla.id = "contenedorTabla"
            //var row = document.createElement("div");
            //row.className = "row";
            //row.appendChild(contenedorTabla);
            //$('#contenedorPrimario').append(row);

            //d = JSON.parse(d);
            //var col = [];
            //var encabezado = ['ID', 'Vendedor',
            //    'Cuota ventas', 'Ventas', '%',
            //    'Cuota cobros', 'Cobros',
            //    ''];

            //for (var i = 0; i < d.length; i++) {
            //    for (var key in d[i]) {
            //        if (col.indexOf(key) === -1) {
            //            col.push(key);
            //        }
            //    }
            //}

            //var card = document.createElement("div");
            //card.className = "card";
            //card.style = "font-size: 15px;overflow-x: scroll;";
            //var table = document.createElement("table");
            //table.className = 'table table-hover tablePresupuesto';
            //table.id = "tablePresupuesto";
            //table.style = '';

            //// CREATE HTML TABLE HEADER ROW USING THE EXTRACTED HEADERS ABOVE
            //var thead = document.createElement("thead");
            //table.appendChild(thead);
            //var tr_head = document.createElement("tr");
            //for (var i = 0; i < encabezado.length; i++) {
            //    var th = document.createElement("th");      // TABLE HEADER.
            //    th.style = 'text-align:center';
            //    th.innerHTML = encabezado[i];
            //    tr_head.appendChild(th);
            //}

            //thead.appendChild(tr_head);

            //var tbody = document.createElement("tbody");
            //table.appendChild(tbody);
            //var tr_body = document.createElement("tr");
            //// ADD JSON DATA TO THE TABLE AS ROWS.
            //for (var i = 0; i < d.length; i++) {
            //    tr_body = document.createElement("tr");
            //    td = document.createElement("td");
            //    td.style = 'text-align:center;';
            //    span = document.createElement("span");
            //    span.className = "datosLinea";
            //    span.dataset.id_vendedor = d[i].id_vendedor;
            //    span.dataset.empresa = d[i].empresa;
            //    span.dataset.sucursal = d[i].sucursal;
            //    span.dataset.fecha_desde = d[i].fecha_desde;
            //    span.dataset.fecha_hasta = d[i].fecha_hasta;
            //    span.dataset.tipocliente = d[i].tipoCliente;
            //    span.type = 'hidden';
            //    td.append(span);
            //    id_vendedor = document.createTextNode(d[i].id_vendedor);
            //    td.appendChild(id_vendedor);
            //    tr_body.appendChild(td);
            //    td = document.createElement("td");
            //    nombre_vendedor = document.createTextNode(d[i].nombre_vendedor);
            //    td.style = 'text-align:center;';
            //    td.appendChild(nombre_vendedor);
            //    tr_body.appendChild(td);
            //    td = document.createElement("td");
            //    valor_venta = document.createTextNode(formatoValor(d[i].valor_venta));
            //    td.style = 'text-align:center;';
            //    td.appendChild(valor_venta);
            //    tr_body.appendChild(td);
            //    td = document.createElement("td");
            //    ventas_neta = document.createTextNode(formatoValor(d[i].ventas_neta));
            //    td.style = 'text-align:center;';
            //    td.appendChild(ventas_neta);
            //    tr_body.appendChild(td);
            //    td = document.createElement("td");
            //    alcance_venta = document.createTextNode(d[i].alcance_venta.toFixed(2));
            //    td.style = 'text-align:center;';
            //    td.appendChild(alcance_venta);
            //    tr_body.appendChild(td);
            //    td = document.createElement("td");
            //    valor_cobro = document.createTextNode(formatoValor(d[i].valor_cobro));
            //    td.style = 'text-align:center;';
            //    td.appendChild(valor_cobro);
            //    tr_body.appendChild(td);
            //    td = document.createElement("td");
            //    total_cobros = document.createTextNode(formatoValor(d[i].total_cobros));
            //    td.style = 'text-align:center;';
            //    td.appendChild(total_cobros);
            //    tr_body.appendChild(td);
            //    td = document.createElement("td");
            //    alcance_cobro = document.createTextNode(d[i].alcance_cobro.toFixed(2));
            //    td.style = 'text-align:center;';
            //    td.appendChild(alcance_cobro);
            //    tr_body.appendChild(td);



            //    tr_body.append(documen)






            $('#contenedorTabla').remove();
            var contenedorTabla = document.createElement("div");
            contenedorTabla.className = "col-md-12";
            contenedorTabla.id = "contenedorTabla"
            var row = document.createElement("div");
            row.className = "row";
            row.appendChild(contenedorTabla);
            $('#contenedorPrimario').append(row);
            $('.container-fluid').add('<div class="row"><div id="tablaPresupuesto" class="col-md-12"></div></div>');



            var sTxt = '<div class="card"><table class="table table-hover" id="tablePresupuesto">';
            sTxt += '<thead><tr><th style="text-align:center">ID</th><th  style="text-align:center">Vendedor</th><th style="text-align:center">Cuota ventas</th>';
            sTxt += '<th style="text-align:center">Ventas</th><th style="text-align:center">%</th>';
            sTxt += '<th style="text-align:center">Cuota cobros</th><th style="text-align:center">Cobros</th>';
            sTxt += '<th style="text-align:center">%</th><th style="text-align:center"></th></tr></thead> ';
            sTxt += '<tbody>';
            $.each(JSON.parse(d), function (index, p) {
                sTxt += '<tr>';
                sTxt += '<td style="text-align:center">';
                sTxt += '<span class="datosLinea" data-id_vendedor=' + p.id_vendedor + ' data-empresa="' + empresa + '" data-sucursal="' + sucursal + '" data-fecha_desde="' + fecha_desde + '" data-fecha_hasta="' + fecha_hasta + '" data-tipocliente="' + tipoCliente + '" hidden></span>';
                sTxt += '' + p.id_vendedor + '</td>';
                sTxt += '<td style="text-align:center">' + p.nombre_vendedor + '</td>';
                sTxt += '<td style="text-align:center">' + formatoValor(p.valor_venta) + '</td>';
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
                sTxt += '    <a class="dropdown-item" href = "#" onClick="verVentasXVendedor(' + p.id_vendedor + ');" > Ventas </a> ';
                sTxt += '    <a class="dropdown-item" href = "#" onClick="verCobrosXVendedor(' + p.id_vendedor + ');" > Cobros </a> ';
                sTxt += '    <a class="dropdown-item" href = "#" onClick="verNCXVendedor(' + p.id_vendedor + ');"> N/C</a> ';
                sTxt += '</div></td>';
                sTxt += '</tr> ';
            });
            sTxt += '</tbody><tfoot><tr><td>Sum</td><td>$180</td></tr></tfoot></table></div>';

            $('#contenedorTabla').append(sTxt);

        },
        error: function (e) {
            Toast.fire({
                icon: 'error',
                title: 'Hubo un error al intentar generar.'
            })
        }
    })

});
