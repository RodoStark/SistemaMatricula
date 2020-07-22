listar();

function listar() {
    $.get("Pagina/ListarPagina", function (data) {
        crearListado(["Id Pagina", "Mensaje", "Controlador", "Accion"], data);
    });
}

function crearListado(arrayColumnas, data) {
    var contenido = "";
    contenido += "<table id='tabla-docente' class='table'>";
    contenido += "<thead>";
    contenido += "<tr>";

    for (var i = 0; i < arrayColumnas.length; i++) {
        contenido += "<td>";
        contenido += arrayColumnas[i];
        contenido += "</td>";
    }

    contenido += "<td>Operaciones</td>";
    contenido += "</tr>";
    contenido += "</thead>";
    var llaves = Object.keys(data[0]);

    contenido += "<tbody>";
    for (var i = 0; i < data.length; i++) {
        contenido += "<tr>";
        for (var j = 0; j < llaves.length; j++) {
            var valorLlave = llaves[j];
            contenido += "<td>";
            contenido += data[i][valorLlave];
            contenido += "</td>";
        }
        var llaveId = llaves[0];
        contenido += "<td>";
        contenido += "<button class = 'btn btn-primary' onclick ='abrirModal(" + data[i][llaveId] + ")' data-toggle='modal' data-target='#myModal'><i class='glyphicon glyphicon-edit'></i></button> ";
        contenido += "<button class = 'btn btn-danger' onclick='eliminar(" + data[i][llaveId] + ")'data-toggle='modal' data-target='#myModal'><i class='glyphicon glyphicon-trash'></i></button>";
        contenido += "</td>";


        contenido += "</tr>";
    }
    contenido += "</tbody>";
    contenido += "</table>";

    document.getElementById("tabla").innerHTML = contenido;

    $("#tabla-docente").dataTable({
        searching: false
    });
}



function llenarCombo(data, control, primerElemento) {
    var contenido = "";
    if (primerElemento == true) {
        contenido += "<option value=''>--Seleccione--</option>";
    }

    for (var i = 0; i < data.length; i++) {
        contenido += "<option value= '" + data[i].IID + "'>";
        contenido += data[i].NOMBRE;
        contenido += "</option>";
    }
    control.innerHTML = contenido;
}

$.get("Alumno/listarSexo", function (data) {
    //llenarCombo(data, document.getElementById('cboSexo'), true);
    llenarCombo(data, document.getElementById('cboSexoPopup'), true);
});


function borrarDatos() {
    var controles = document.getElementsByClassName("borrar");


    var ncontroles = controles.length;
    for (var i = 0; i < ncontroles; i++) {
        controles[i].value = "";
    }
}

function datosObligatorios() {
    var exito = true;
    var controlesObligatorios = document.getElementsByClassName("obligatorio");
    var ncontroles = controlesObligatorios.length;
    for (var i = 0; i < ncontroles; i++) {
        if (controlesObligatorios[i].value == "") {
            exito = false;
            controlesObligatorios[i].parentNode.classList.add("error");
        } else {
            controlesObligatorios[i].parentNode.classList.remove("error");
        }
    }
    return exito;
}


function eliminar(id) {

    if (confirm("Desea eliminar") == 1) {
        $.get("Docente/eliminar/?id=" + id, function (data) {
            if (id == 0) {
                alert("Ocurrió un error");
            } else {
                alert("Se eliminó correctamente");
                listar();
            }
        });
    }
}


function abrirModal(id) {


    var controlesObligatorios = document.getElementsByClassName("obligatorio");
    var ncontroles = controlesObligatorios.length;
    for (var i = 0; i < ncontroles; i++) {
        controlesObligatorios[i].parentNode.classList.remove("error");
    }


    if (id == 0) {
        borrarDatos();
    }
    else {
        $.get("Pagina/RecuperarInformacion/?id=" + id, function (data) {
            console.log(data);
            document.getElementById("txtIdPagina").value = data[0].IIDPAGINA;
            document.getElementById("txtMensaje").value = data[0].MENSAJE;
            document.getElementById("txtControlador").value = data[0].CONTROLADOR;
            document.getElementById("txtAccion").value = data[0].ACCION;
        });
    }
}

function agregar() {
    if (datosObligatorios()) {
        var frm = new FormData();
        var idPagina = document.getElementById("txtIdPagina").value;
        var controlador = document.getElementById("txtControlador").value;
        var mensaje = document.getElementById("txtMensaje").value;
        var accion = document.getElementById("txtAccion").value;
        

        frm.append("IIDPAGINA", idPagina);
        frm.append("CONTROLADOR", controlador);
        frm.append("MENSAJE", mensaje);
        frm.append("ACCION", accion);
        
        if (confirm("¿Desea guardar los cambios?") == 1) {

            $.ajax({
                type: "POST",
                url: "Pagina/GuardarDatos",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data == 0) {
                        alert("Ocurrió un error");
                    }
                    else if (data == -1) {
                        alert("ocurrio un error");
                    }
                    else {
                        alert("se ejecutó correctamente");
                        listar();
                        document.getElementById("btnCancelar").click();
                    }
                }
            });
        }

    }
    else { alert('Valio mothers'); }
}


