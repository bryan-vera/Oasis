
$(".imprimir").click(function () {
    id_guia = this.dataset.id_guia_remision;
    codigoOrganizacion = this.dataset.codigo_organizacion;
    var EsLabovida = false;
    if (codigoOrganizacion == 69) {
        EsLabovida = true;
    }
    Swal.mixin({
        input: 'text',
        confirmButtonText: 'Siguiente &rarr;',
        showCancelButton: true,
        progressSteps: ['1', '2','3'],
        showLoaderOnConfirm: true,
    }).queue([
        {
            title: 'Generar guía',
            text: 'Ingrese el peso'
        },
        {
            title: 'Generar guía',
            text: 'Ingrese la cantidad de bultos'
        },
        {
            title: 'Generar guía',
            text: 'Ingrese la guía urbano'
        }
    ]).then((result) => {
        if (result.value) {
            peso = result.value[0];
            bultos = result.value[1];
            guia_urbano = result.value[2];
            $.ajax({
                url: 'GuiasRemision/Imprimir/' + id_guia,
                type: "GET",
                data: { peso: peso, bultos: bultos,guia_urbano: guia_urbano, EsLabovida: EsLabovida },
                success: function (d) {
                    if (d.length > 0) {
                        let pdfWindow = window.open("")
                        pdfWindow.document.write(
                            "<iframe width='100%' height='100%' src='data:application/pdf;base64," +
                            encodeURI(d) + "'></iframe>"
                        )

                        Toast.fire({
                            icon: 'success',
                            title: 'Se ha generado la OC de forma correcta.'
                        })
                    }
                },
                error: function (e) {
                    Toast.fire({
                        icon: 'error',
                        title: 'Hubo un error al intentar guardar.'
                    })
                }

            })
        }
    })
});

$("#GenerarReporteGuias").click(function () {
    let empresa;
    let indicador_factura;

    if ($("#fecha_factura").is(":disabled")) {
        indicador_factura = false;
    } else {
        indicador_factura = true;
    }

    if ($("#empresa").is(':disabled')) {
        empresa = "";
    } else {
        empresa = $("#empresa").val();
    }

    //var sucursal = $("#sucursal").val();
    var fecha_desde_factura = ConvertirFecha($('#fecha_factura').data('daterangepicker').startDate._d);
    var fecha_hasta_factura = ConvertirFecha($('#fecha_factura').data('daterangepicker').endDate._d);
    var fecha_desde_guia = ConvertirFecha($('#fecha_guia').data('daterangepicker').startDate._d);
    var fecha_hasta_guia = ConvertirFecha($('#fecha_guia').data('daterangepicker').endDate._d);


    iniciaLoading();
    $.ajax({
        url: 'ObtenerReporteGuia',
        type: 'GET',
        data: {
            empresa: empresa,
            //sucursal: sucursal,
            fecha_desde_factura: fecha_desde_factura,
            fecha_hasta_factura: fecha_hasta_factura,
            fecha_desde_guia: fecha_desde_guia,
            fecha_hasta_guia: fecha_hasta_guia,
            indicador_factura: indicador_factura
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
            var encabezado = ['EMPRESA', 'CLIENTE',
                'SECUENCIAL FACTURA', 'FECHA FACTURA', 'SECUENCIAL GUIA',
                'FECHA GUIA', 'CIUDAD',
                'PESO','BULTOS',
                'GUIA URBANO', 'NOTA GUIA'];
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
            Toast.fire({
                icon: 'error',
                title: 'Hubo un error al intentar generar.'
            })
        }
    })

});

$('#fecha_guia').daterangepicker({
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

$('#fecha_factura').daterangepicker({
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



$("#chkRegion").click(function () {
    $("#sucursal").attr('disabled', this.checked)
});

$("#chkEmpresa").click(function () {
    $("#empresa").attr('disabled', this.checked)
});

$("#chkFechaGuia").change(function () {
    $("#fecha_factura").prop('disabled', function (i, v) { return !v; });
    $("#fecha_guia").prop('disabled', function (i, v) { return !v; });


    //elemento.disabled = !elemento.disabled;
    //$("#fecha_factura").attr('disabled', this.checked)
});

$("#chkFechaFactura").change(function () {
    //var elemento = $("#fecha_guia");
    //elemento.prop('disabled', function (i, v) { return !v; });
    $("#fecha_factura").prop('disabled', function (i, v) { return !v; });
    $("#fecha_guia").prop('disabled', function (i, v) { return !v; });
    //elemento.disabled = !elemento.disabled;
    //$("#fecha_guia").attr('disabled', this.checked)
});