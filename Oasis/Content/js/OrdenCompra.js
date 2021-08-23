
var OCDetalle = [];
$('.select-departamentos').select2({
    language: {
        inputTooShort: function () { return "Ingresar dos o más caracteres"; }
    },
});


$('#submit').click(function () {
    var valor_diferencia = parseFloat($("#DiferenciaTotalOC").val())
    if (valor_diferencia == 0) {
        var tbl = $('#tablaOrdenCompra tr:has(td)').map(function (i, v) {
            var $td = $('td', this);
            OCDetalle.push({
                id_producto: $td.eq(0).children(0).val(),
                cantidad_producto: $td.eq(1).children(0).val(),
                //descuento: $td.eq(3).children(0).val(),
                //subtotal_iva: $td.eq(3).children(0),
                valor_linea: $td.eq(3).children(0).val()
            })
        }).get();

        var datos = {
            id_organizacion: parseInt($('#empresa')[0].dataset.id_empresa),
            id_departamento: parseInt($('#id_departamento').val().trim()),
            id_proveedor: parseInt($('#id_proveedor')[0].dataset.id_proveedor),
            fecha_documento: $('#fecha_documento').val().trim(),
            valor_total: parseFloat($('#TotalPrincipalOC').val().trim()),
            id_oc_principal: id_oc,
            //descuento: parseFloat($('#SubtotalPrincipalOC').val().trim()),
            //iva: parseFloat($('#IVAPrincipalOC').val().trim()),
            iva: parseFloat($('#IVAPrincipalOC').val().trim()),
            ListaDeDetalleOrdenCompra: OCDetalle,
        };

        $.ajax({
            url: 'Create',
            type: "POST",
            data: JSON.stringify(datos),
            dataType: "JSON",
            contentType: "application/JSON",
            success: function (d) {
                if (d.status == true) {
                    Toast.fire({
                        icon: 'success',
                        title: 'Se ha generado la OC de forma correcta.',
                        timer: 1000,
                        didDestroy: () => {
                            window.location.href = '/DetalleOrdenCompra';
                        }
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
    } else {
        Toast.fire({
            icon: 'error',
            title: 'Se encuentran diferencias en el valor total.'
        });
    }
});

function CheckearDatosValidos() {
    var esValido = true;
    var cantidad = $('#tablaOrdenCompra tr:last').find(':input[id^="CantidadDetalleOC"]');
    var producto = $('#tablaOrdenCompra tr:last').find(':input[id^="productoDetalleOC"]');
   
    if (cantidad.val().trim() == "" || isNaN(cantidad.val().trim()) ) {
        Toast.fire({
            icon: 'error',
            title: 'Se debe aplicar una cantidad al producto.'
        })
        esValido = false;
    }

    if (producto.val() == "" || producto.val() == null) {
        Toast.fire({
            icon: 'error',
            title: 'Se debe escoger un producto.'
        })
        esValido = false;
    }

    return esValido;
}

function AgregarDetalle() {
    if (CheckearDatosValidos()) {
        let tablaOCDetalle = document.getElementById("tablaOrdenCompra");
        let tr = document.createElement("tr");
        let tdProd = document.createElement("td");
        let tdCant = document.createElement("td");
        let tdVU = document.createElement("td");
        let tdDSCTO = document.createElement("td");
        let tdSubt = document.createElement("td");
        let tdBorrar = document.createElement("td");
        tdProd.innerHTML = '<select class="js-data-example-ajax col-md-12" name="productoDetalleOC" id="productoDetalleOC"></select >';
        tdCant.innerHTML = '<input type="number" class="form-control form-control-border Cantidad" name="CantidadDetalleOC" id = "CantidadDetalleOC" min = "0" step = ".01" style = "text-align: right;" />';
        tdVU.innerHTML = '<input type="number" class="form-control form-control-border ValorUnitarioOC"  id = "ValorUnitarioOC"    min = "0" style = "text-align: right;" />';
        //tdDSCTO.innerHTML = '<input type="number" class="form-control form-control-border DescuentoOC"  name="DescuentoOC" id = "DescuentoOC"    min = "0" style = "text-align: right;" />';
        tdSubt.innerHTML = '<input type="number" class="form-control form-control-border SubtotalDetalleOC"  name="SubtotalDetalleOC" id = "SubtotalDetalleOC"    min = "0" style = "text-align: right;" disabled />';
        tdBorrar.innerHTML = '<button class="btn btn-danger BorrarFila" type="button"><i class="fa fa-trash"></i></button>';
        tr.appendChild(tdProd);
        tr.appendChild(tdCant);
        tr.appendChild(tdVU);
        //tr.appendChild(tdDSCTO);
        tr.appendChild(tdSubt);
        tr.appendChild(tdBorrar);
        tablaOCDetalle.tBodies[0].appendChild(tr);
    } else {
        return;
    }
}

function SumatoriaSubtotales() {
    var tableElem = window.document.getElementById("tablaOrdenCompra");
    var rows = tableElem.rows;
    var tableBody = tableElem.getElementsByTagName("tbody").item(0);
    var i;
    var valor_total_teorico = parseFloat($("#DiferenciaTotalOC").data("valortotal"));
    var whichColumn = 4;
    var howManyRows = tableBody.rows.length;
    var subtotal = 0;
    var total_iva = 0;
    var descuentoTotal = 0;
    var subtotal_sin_iva = 0;
    var total = 0;

    for (i = 1; i <= howManyRows; i++) {
        cell = rows[i].cells[3];
        //celdaDsct = rows[i].cells[3];
        linea_iva = $(rows[i].cells[2]).find(':input')[0].dataset.iva;
        total_iva += linea_iva == "true" ? parseFloat($(cell).find(':input').val())*0.12 : 0.00;
        subtotal += parseFloat($(cell).find(':input').val() == "" ? 0.00 : $(cell).find(':input').val()) + total_iva;
        subtotal_sin_iva += parseFloat($(cell).find(':input').val() == "" ? 0.00 : $(cell).find(':input').val());
        //descuentoTotal += parseFloat($(celdaDsct).find(':input').val() == "" ? 0.00 : $(celdaDsct).find(':input').val());
        //subtotal += parseFloat(thisTextNode.value,2).toFixed(2);
    }
    total = subtotal_sin_iva + total_iva;
    $("#SubtotalPrincipalOC").val((subtotal_sin_iva).toFixed(2));
    $("#IVAPrincipalOC").val(total_iva.toFixed(2));
    $("#TotalPrincipalOC").val((total).toFixed(2));
    $("#DiferenciaTotalOC").val((valor_total_teorico-total).toFixed(2));
    
}

function ActivarProductos() {

    $('.js-data-example-ajax').select2({
        //selectOnClose: true,
        minimumResultsForSearch: -1,
        minimumInputLength: 2,
        language: {
            inputTooShort: function () { return "Ingresar dos o más caracteres"; }
        },
        //tags: [],
        ajax: {
            url: '/Gastos/ObtenerProductos',
            processResults: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            text: item.descripcion,
                            id: item.id_producto,
                            valor_unitario: item.valor_unitario,
                            grava_iva: item.iva
                        }
                    })
                };
            },
            dataType: 'json',
            data: function (params) {
                var query = {
                    textoBusqueda: params.term,
                }
                return query;
            }
        }
    }).on('change', function (e) {
        var grava_iva = $(this).select2('data')[0].grava_iva;
        var valor_unitario;
        //if (grava_iva) {
        //    valor_unitario = Math.round($(this).select2('data')[0].valor_unitario*1.12,2);
        //} else {
            valor_unitario = $(this).select2('data')[0].valor_unitario;
        //}

        var $tr = $(this).parents("tr");
        $(this).parent().siblings().each(function () {
            var elemento_ = $(this).find('input');
            if (elemento_.attr('id') == "ValorUnitarioOC") {
                elemento_.val(valor_unitario);
                elemento_.attr('data-iva', grava_iva);
                //elemento_.attr('data-valor_iva', grava_iva);
            }
        });
    });
}

$('#tablaOrdenCompra').on('click', function (e) {
    //$(event.target).is('button') ? $(event.target) : $(event.target).parent();
    if (e.target.classList.contains("BorrarFila")) {
        var boton_borrar = e.target;
        $(boton_borrar).parents("tr").remove();
        SumatoriaSubtotales();
        //filasOC--;
    }
});

$('#tablaOrdenCompra').on('change', function (e) {
    if (e.target.tagName == "INPUT" || e.target.tagName == "SELECT") {
        var cambio_input = e.target;
        var cantidad = $(cambio_input).parents("tr").find('input[id="CantidadDetalleOC"]').val();
        var vu = $(cambio_input).parents("tr").find('input[id="ValorUnitarioOC"]').val();
        var subtotal_detalle = (cantidad * vu).toFixed(2);
        $(cambio_input).parents("tr").find('input[id="SubtotalDetalleOC"]').val(subtotal_detalle);
        SumatoriaSubtotales();
    }
});