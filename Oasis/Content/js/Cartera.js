
var empresa;
var sucursal;
var localidad;
var tipoCliente = [];
var id_visitador;

function ParametrosCartera() {
    empresa = $("#empresa_ind").val();
    sucursal = $("#sucursal_ind").val();
    id_visitador = $('.js-data-vendedor-ajax').children("option:selected").val();
    if ($('#chkLocalidad').is(":checked")) {
        localidad = null
    } else {
        localidad = $("#localidad_ind").val();
    }

    $('.tipoCliente:checkbox:checked').each(function () {
        tipoCliente.push($(this).attr('name'));
    });
}


$('#DetalleIndividual').click(function () {
    
    ParametrosCartera();
    iniciaLoading();
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
            row.className = "row";
            row.appendChild(contenedorTabla);
            $('#contenedorPrimario').append(row);
            d = JSON.parse(d);

            var detalleAgrupado = {};
            for (var key in d) {
                var identificacion = d[key].identificacion;
                if (!detalleAgrupado[identificacion]) {
                    detalleAgrupado[identificacion] = [];
                }
                detalleAgrupado[identificacion].push(d[key]);
            }

            var col = [];
            var encabezado = [
                '1', '2', '3',
                '4', '5', '6',
                '7', '8', '9',
                '10', '11', '12'];

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
            //var th = document.createElement("th");
            //th.innerHTML = "1";
            //th.colSpan = 12;
            //tr_head.appendChild(th);
            //thead.appendChild(tr_head);
            for (var i = 0; i < encabezado.length; i++) {
                var th = document.createElement("th");      // TABLE HEADER.
                th.innerHTML = encabezado[i];
                th.style = "display:none";
                tr_head.appendChild(th);
            }

            thead.appendChild(tr_head);
            var tbody = document.createElement("tbody");
            table.appendChild(tbody);
            var tr_body = document.createElement("tr");
            var tabCell;
            // ADD JSON DATA TO THE TABLE AS ROWS.
            Object.values(detalleAgrupado).forEach(val => {
                tr_body = document.createElement("tr");
                tabCell = tr_body.insertCell(-1);
                tabCell.colSpan = 2;
                tabCell.innerHTML = '<span style="font-weight: 800;">RUC:</span> '+ val[0].identificacion;
                for (var j = 0; j < 1; j++) {
                    var tabCell = tr_body.insertCell(-1);
                    tabCell.style = 'display: none;';
                }

                //tabCell.style = ' white-space: nowrap;';
                tabCell = tr_body.insertCell(-1);
                tabCell.colSpan = 7;
                tabCell.innerHTML = '<span style="font-weight: 800;">Cliente:</span> '+ val[0].nombre_comercial;
                for (var j = 0; j < 6; j++) {
                    var tabCell = tr_body.insertCell(-1);
                    tabCell.style = 'display: none;';
                }
                //tabCell.style = ' white-space: nowrap;';
                //tabCell = tr_body.insertCell(-1);
                //tabCell.innerHTML = '';
                //tabCell.style = ' white-space: nowrap;';


                tabCell = tr_body.insertCell(-1);
                tabCell.colSpan = 3;
                //tabCell.innerHTML = val[0].contacto;

                for (var j = 0; j < 2; j++) {
                    var tabCell = tr_body.insertCell(-1);
                    tabCell.style = 'display: none;';
                }

                tbody.appendChild(tr_body);


                tr_body = document.createElement("tr");
                tabCell = tr_body.insertCell(-1);
                tabCell.colSpan = 6;
                tabCell.innerHTML = '<span style="font-weight: 800;">Dirección:</span> '+ val[0].direccion;

                for (var j = 0; j < 5; j++) {
                    var tabCell = tr_body.insertCell(-1);
                    tabCell.style = 'display: none;';
                }


                tabCell = tr_body.insertCell(-1);
                tabCell.colSpan = 3;
                tabCell.innerHTML = '<span style="font-weight: 800;">Parroquia:</span> '+ val[0].parroquia;

                for (var j = 0; j < 2; j++) {
                    var tabCell = tr_body.insertCell(-1);
                    tabCell.style = 'display: none;';
                }

                tabCell = tr_body.insertCell(-1);
                tabCell.colSpan = 3;
                tabCell.innerHTML = '<span style="font-weight: 800;">Ciudad:</span> '+val[0].ciudad;

                for (var j = 0; j < 2; j++) {
                    var tabCell = tr_body.insertCell(-1);
                    tabCell.style = 'display: none;';
                }

                tbody.appendChild(tr_body);



                tr_body = document.createElement("tr");
                tr_body.style = 'font-weight: 800;';
                tabCell = tr_body.insertCell(-1);
                tabCell.innerHTML = 'N. Doc.';
                tabCell = tr_body.insertCell(-1);
                tabCell.innerHTML = 'Tipo Doc.';
                tabCell.style = ' width: 10%;';
                tabCell = tr_body.insertCell(-1);
                tabCell.innerHTML = 'Emisión';
                tabCell = tr_body.insertCell(-1);
                tabCell.innerHTML = 'Vencimiento';
                tabCell = tr_body.insertCell(-1); 
                tabCell.innerHTML = 'Dias vencidos';
                tabCell = tr_body.insertCell(-1);
                tabCell.innerHTML = 'Cheque tránsito';
                tabCell = tr_body.insertCell(-1);
                tabCell.innerHTML = 'Valor factura';
                tabCell = tr_body.insertCell(-1);
                tabCell.innerHTML = 'Valor Pagado';
                tabCell = tr_body.insertCell(-1);
                tabCell.innerHTML = '0-60 Días';
                tabCell.style = ' width: 10%;';
                tabCell = tr_body.insertCell(-1);
                tabCell.innerHTML = '61-90 Días';
                tabCell.style = ' width: 10%;';
                tabCell = tr_body.insertCell(-1);
                tabCell.innerHTML = '91-120 Días';
                tabCell.style = ' width: 10%;';
                tabCell = tr_body.insertCell(-1);
                tabCell.innerHTML = '+ 120 días';
                tabCell.style = ' width: 10%;';
                tbody.appendChild(tr_body);

                var totalvalorFactura = 0;
                var totalvalorChequeTransito = 0; 
                var totalvalorPagado = 0;
                var totalvalor0_60 = 0;
                var totalvalor61_90 = 0;
                var totalvalor91_120 = 0;
                var totalvalor120 = 0;

                Object.values(val).forEach(val => {
                    var valor0_60 = 0;
                    var valor61_90 = 0;
                    var valor91_120 = 0;
                    var valor_120 = 0;
                    var diasVencido = parseInt(val.dias_diferencia);

                    if (diasVencido <= 60) { valor0_60 = val.saldo_pendiente } else
                        if (diasVencido >= 61 && diasVencido <= 90) { valor61_90 = val.saldo_pendiente } else
                            if (diasVencido >= 91 && diasVencido <= 120) { valor91_120 = val.saldo_pendiente } else
                                if (diasVencido >= 121) { valor_120 = val.saldo_pendiente } 


                    tr_body = document.createElement("tr");
                    tabCell = tr_body.insertCell(-1);
                    tabCell.innerHTML = val.secuencial_factura;

                    tabCell = tr_body.insertCell(-1);
                    tabCell.innerHTML = val.tipo_documento;

                    tabCell = tr_body.insertCell(-1);
                    tabCell.innerHTML = val.fecha_factura;

                    tabCell = tr_body.insertCell(-1);
                    tabCell.innerHTML = val.fecha_vencimiento;

                    tabCell = tr_body.insertCell(-1);
                    tabCell.innerHTML = val.dias_diferencia;

                    tabCell = tr_body.insertCell(-1);
                    tabCell.innerHTML = formatoValor(val.totalChequePost);

                    tabCell = tr_body.insertCell(-1);
                    tabCell.innerHTML = formatoValor(val.valor_factura);

                    tabCell = tr_body.insertCell(-1);
                    tabCell.innerHTML = formatoValor(val.valor_factura - val.saldo_pendiente);

                    tabCell = tr_body.insertCell(-1);
                    tabCell.innerHTML = formatoValor(valor0_60);

                    tabCell = tr_body.insertCell(-1);
                    tabCell.innerHTML = formatoValor(valor61_90);

                    tabCell = tr_body.insertCell(-1);
                    tabCell.innerHTML = formatoValor(valor91_120);

                    tabCell = tr_body.insertCell(-1);
                    tabCell.innerHTML = formatoValor(valor_120);
                    tbody.appendChild(tr_body);

                     totalvalorFactura += val.valor_factura;
                     totalvalorChequeTransito += val.totalChequePost;
                     totalvalorPagado += (val.valor_factura-val.saldo_pendiente);
                     totalvalor0_60 += valor0_60;
                     totalvalor61_90 += valor61_90;
                     totalvalor91_120 += valor91_120;
                     totalvalor120 += valor_120;

                });

                tr_body = document.createElement("tr");
                tabCell = tr_body.insertCell(-1);
                tabCell.colSpan = 5;
                tabCell.innerHTML = '<span style="font-weight: 800;">Total cliente:</span> ';
                for (var j = 0; j < 4; j++) {
                    var tabCell = tr_body.insertCell(-1);
                    tabCell.style = 'display: none;';
                }
                tabCell = tr_body.insertCell(-1);
                tabCell.innerHTML = formatoValor(totalvalorChequeTransito);
                tabCell = tr_body.insertCell(-1);
                tabCell.innerHTML = formatoValor(totalvalorFactura);
                tabCell = tr_body.insertCell(-1);
                tabCell.innerHTML = formatoValor(totalvalorPagado);
                tabCell = tr_body.insertCell(-1);
                tabCell.innerHTML = formatoValor(totalvalor0_60);
                tabCell = tr_body.insertCell(-1);
                tabCell.innerHTML = formatoValor(totalvalor61_90);
                tabCell = tr_body.insertCell(-1);
                tabCell.innerHTML = formatoValor(totalvalor91_120);
                tabCell = tr_body.insertCell(-1);
                tabCell.innerHTML = formatoValor(totalvalor120);
                
                tbody.appendChild(tr_body);


                tr_body = document.createElement("tr");

                tabCell = tr_body.insertCell(-1);
                //tabCell = tr_body.insertCell(-1);
                tabCell.colSpan = 12;
                //tabCell.innerHTML = '-- Separador de bajo presupuesto --';
                for (var j = 0; j < 11; j++) {
                    var tabCell = tr_body.insertCell(-1);
                    tabCell.style = 'display: none;';
                }
                tbody.appendChild(tr_body);
                //for (var i = 0; i < 12; i++) {
                //    tabCell = tr_body.insertCell(-1);
                //}
                //tbody.appendChild(tr_body);
                //console.log(val);
            });

            //for (var i = 0; i < detalleAgrupado; i++) {
            //    tr_body = document.createElement("tr");
            //    //for (var j = 0; j < col.length; j++) {
            //    var tabCell = tr_body.insertCell(-1);
            //    tabCell.innerHTML = d[i].nombre_comercial;
            //    tabCell.style = ' white-space: nowrap;';
            //    //}
            //    tbody.appendChild(tr_body);
            //}

            cardbody.appendChild(table);
            card.appendChild(cardbody);
            div.appendChild(card);

            $('#contenedorTabla').append(div);
            $('#tableDetalle').DataTable({
                dom: 'Bfrtip',
                "ordering": false,
                "searching": false,
                "pageLength": 100,
                buttons: [
                   
                    'copy', 'csv', 'excel', {
                        extend: 'pdfHtml5',
                        orientation: 'landscape',
                        pageSize: 'LEGAL'
                    }, 'print'
                ],
                language: {
                    url: 'https://cdn.datatables.net/plug-ins/1.10.24/i18n/Spanish.json'
                }
            });
            cierraLoading();
            Swal.fire(
                '¡Atención!',
                'La información aquí presentada se encuentra bajo revisión de la jefatura de crédito, una vez sea esta confirmada se podrá hacer uso como datos fiables hasta tanto solo será usada para verificación.',
                'info'
            );
        },
        error: function (e) {
            Toast.fire({
                icon: 'error',
                title: 'Hubo un error al intentar generar.'
            })
        }
    })



});

$("#GenerarCartera").click(function () {
    ParametrosCartera();

    var eleccion_cartera_general = $('#cartera_general')[0].checked;

    if (eleccion_cartera_general) {
        iniciaLoading();
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
                cierraLoading();
            },
            error: function (e) {
                Toast.fire({
                    icon: 'error',
                    title: 'Hubo un error al intentar generar.'
                })
            }
        })
    } else {
        iniciaLoading();
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
                    'TIPO DOC',
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
                cierraLoading();
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
