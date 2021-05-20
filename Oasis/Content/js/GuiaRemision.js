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
        progressSteps: ['1', '2'],
        showLoaderOnConfirm: true,
    }).queue([
        {
            title: 'Generar guía',
            text: 'Ingrese el peso'
        },
        'Ingrese la cantidad de bultos'
    ]).then((result) => {
        if (result.value) {
            peso_ = result.value[0];
            bultos_ = result.value[1];
            $.ajax({
                url: 'GuiasRemision/Imprimir/' + id_guia,
                type: "GET",
                data: { peso: peso_, bultos: bultos_, EsLabovida: EsLabovida },
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