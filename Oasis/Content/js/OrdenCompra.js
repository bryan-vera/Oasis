
var OCDetalle = [];


$('#submit').click(function () {
    var tbl = $('#tablaOrdenCompra tr:has(td)').map(function (i, v) {
        var $td = $('td', this);
        OCDetalle.push({  
            id_producto: $td.eq(0).children(0).val(),
            cantidad_producto: $td.eq(1).children(0).val(),
            descuento: $td.eq(3).children(0).val(),
            valor_linea: $td.eq(4).children(0).val() 
        })
    }).get();

    var datos = {
        empresa: $('#empresa').val().trim(),
        id_dpto: $('#id_dpto').val().trim(),
        id_proveedor: $('#id_proveedor').val().trim(),
        fecha_documento: $('#fecha_documento').val().trim(),
        valor_total: parseFloat($('#TotalPrincipalOC').val().trim()),
        descuento: parseFloat($('#SubtotalPrincipalOC').val().trim()),
        //iva: parseFloat($('#IVAPrincipalOC').val().trim()),
        iva: 0.00,
        ListaDeDetalleOrdenCompra: OCDetalle,
    };

    $.ajax({
        url:'Create',
        type: "POST",
        data: JSON.stringify(datos),
        dataType: "JSON",
        contentType: "application/JSON",
        success: function (d) {
            if (d.status == true) {
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
        //OCDetalle.push({
        //    id_producto: $("#productoDetalleOC").val().trim(),
        //    cantidad_producto: parseFloat($("#CantidadDetalleOC").val().trim()),
        //    descuento: parseFloat($("#DescuentoOC").val().trim()),
        //    valor_linea: parseFloat($("#SubtotalDetalleOC").val().trim())
        //});
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
        tdDSCTO.innerHTML = '<input type="number" class="form-control form-control-border DescuentoOC"  name="DescuentoOC" id = "DescuentoOC"    min = "0" style = "text-align: right;" />';
        tdSubt.innerHTML = '<input type="number" class="form-control form-control-border SubtotalDetalleOC"  name="SubtotalDetalleOC" id = "SubtotalDetalleOC"    min = "0" style = "text-align: right;" />';
        tdBorrar.innerHTML = '<button class="btn btn-danger BorrarFila" type="button"><i class="fa fa-trash"></i></button>';
        tr.appendChild(tdProd);
        tr.appendChild(tdCant);
        tr.appendChild(tdVU);
        tr.appendChild(tdDSCTO);
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
    var whichColumn = 4;
    var howManyRows = tableBody.rows.length;
    var subtotal = 0;
    var descuentoTotal = 0;
    for (i = 1; i <= howManyRows; i++) {
        cell = rows[i].cells[4];
        celdaDsct = rows[i].cells[3];
        subtotal += parseFloat($(cell).find(':input').val() == "" ? 0.00 : $(cell).find(':input').val());
        descuentoTotal += parseFloat($(celdaDsct).find(':input').val() == "" ? 0.00 : $(celdaDsct).find(':input').val());
        //subtotal += parseFloat(thisTextNode.value,2).toFixed(2);
    }
    $("#SubtotalPrincipalOC").val(subtotal.toFixed(2));
    $("#TotalPrincipalOC").val(subtotal.toFixed(2));
}


function ActivarProductos() {
    $('.js-data-proveedor-ajax').select2({
        //selectOnClose: true,
        minimumInputLength: 2,
        language: {
            inputTooShort: function () { return "Ingresar dos o más caracteres"; }
        },
        tags: [],
        ajax: {
            url: '/Proveedor/ObtenerProveedores',
            processResults: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            text: item.nombre_comercial,
                            id: item.identificacion
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
    });

    $('.js-data-example-ajax').select2({
        //selectOnClose: true,
        minimumInputLength: 2,
        language: {
            inputTooShort: function () { return "Ingresar dos o más caracteres"; }
        },
        tags: [],
        ajax: {
            url: '/Gastos/ObtenerProductos',
            processResults: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            text: item.descripcion,
                            id: item.id_producto,
                            valor_unitario: item.valor_unitario
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
        var valor_unitario = $(this).select2('data')[0].valor_unitario;
        var $tr = $(this).parents("tr");
        $(this).parent().siblings().each(function () {
            var elemento_ = $(this).find('input');
            if (elemento_.attr('id') == "ValorUnitarioOC") {
                elemento_.val(valor_unitario);
            }
        });
    });
}



//var viewModel = function () {

//    var self = this;

//    self.CustomerID = ko.observable();
//    self.CompanyName = ko.observable();
//    self.ContactName = ko.observable();
//    self.ContactTitle = ko.observable();
//    self.City = ko.observable();
//    self.Country = ko.observable();

//    self.customerList = ko.observableArray([]);

//    self.OrderID = ko.observable();
//    self.EmployeeID = ko.observable();
//    self.OrderDate = ko.observable();
//    self.RequiredDate = ko.observable();
//    self.ShippedDate = ko.observable();
//    self.ShipName = ko.observable();

//    self.OrdersList = ko.observableArray([]);

//    var CompanyUri = '/api/Company/';


//    function ajaxFunction(uri, method, data) {

//        return $.ajax({

//            type: method,
//            url: uri,
//            dataType: 'json',
//            contentType: 'application/json',
//            data: data ? JSON.stringify(data) : null

//        }).fail(function (jqXHR, textStatus, errorThrown) {

//            alert('Error: ' + errorThrown);

//        });
//    }

//    //Customer List function which returns all customers from database.  
//    function customerList() {

//        ajaxFunction(CompanyUri, 'GET').done(function (data) {

//            self.customerList(data);

//        });

//    }

//    //Detail Orders function accepts customer id as parameter and returns all orders related to customer id.  
//    self.detailOrders = function (customer) {

//        ajaxFunction(CompanyUri + customer.CustomerID, 'GET').done(function (data) {

//            self.OrdersList(data);

//        });

//    }

//    customerList();

//};

//ko.applyBindings(new viewModel()); 