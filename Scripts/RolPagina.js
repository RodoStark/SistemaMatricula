listar();

function listar(){
    $.get("RolPagina/ListarRol", function (data) {
        crearListado(["Id Rol", "Nombre", "Descripción"], data);
    });
}


var btnBuscar = document.getElementById("btnBuscar");
btnBuscar.onclick = function () {
    var nombre = document.getElementById("txtnombre").value;
    $.get("Curso/buscarCursoPorNombre/?nombre=" + nombre, function (data) {
        crearListado(data);
    })

}

var btnLimpiar = document.getElementById("btnLimpiar");
btnLimpiar.onclick = function () {
    $.get("Curso/listarCursos", function (data) {
        crearListado(data);
    });
    document.getElementById("txtnombre").value = "";
}


function llenarCombo(data, control) {
    var contenido = "";
    for (var i = 0; i < data.length; i++) {
        contenido += "<option value'" + data[i].IID + "'>";
        contenido += data[i].NOMBRE;
        contenido += "</option>";
    }
    control.innerHTML = contenido;
}


function crearListado(arrayColumnas, data) {

    var contenido = "";
    contenido += "<table id='tabla-curso' class= 'table'>";
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

    $("#tabla-curso").dataTable({
        searching: false
    });
}

var idRol;
function abrirModal(id) {

    idRol = id;
    var controlesObligatorios = document.getElementsByClassName("obligatorio");
    var ncontroles = controlesObligatorios.length;
    for (var i = 0; i < ncontroles; i++) {
        controlesObligatorios[i].parentNode.classList.remove("error");
    }
    $.get("RolPagina/ListarPaginas/?id=" + id, function (data) {
        var contenido = "<tbody>";
        for (var i = 0; i < data.length; i++) {
            contenido += "<tr>";
            contenido += "<td>";
            contenido += "<input class='checkbox' id=" + data[i].IIDPAGINA + " type='checkbox'>";
            contenido += "</td>";
            contenido += "<td>";
            contenido += data[i].MENSAJE;
            contenido += "</td>";
            contenido += "</tr>";
        }
        contenido += "</tbody>";
        document.getElementById('tblPagina').innerHTML = contenido;

        if (id > 0)
        {
            obtenerPaginasRol();
        }        
    });
    
    if (id == 0) {
        borrarDatos();
    } 
}

function obtenerPaginasRol()
{
    $.get("RolPagina/listarRolPagina/?idRol=" + idRol, function (data) {
        var nregistros = data.length;
        for (var i = 0; i < nregistros; i++) {
            if (data[i].BHABILITADO == 1) {
                document.getElementById(data[i].IIDPAGINA).checked = true;
            }
        }
    });
    $.get("RolPagina/obtenerRol/?" + idRol, function (data) {    
        document.getElementById("txtRol").value = data.IIDROL;
        document.getElementById("txtNombre").value = data.NOMBRE;
        document.getElementById("txtDescripcion").value = data.DESCRIPCION;
    });
}

function borrarDatos() {
    var controles = document.getElementsByClassName("borrar");
    var ncontroles = controles.length;
    for (var i = 0; i < length; i++) {
        controles.value = "";
    }
}

function agregar() {
    if (datosObligatorios()) {
        var frm = new FormData();
        var id = document.getElementById("txtRol").value;
        var nombre = document.getElementById("txtNombre").value;
        var descripcion = document.getElementById("txtDescripcion").value;
        frm.append("IIDROL", id);
        frm.append("NOMBRE", nombre);
        frm.append("DESCRIPCION", descripcion);
        frm.append("BHABILITADO", 1);


        var checkbox = document.getElementByClassName("checkbox");
        var ncheckbox = checkbox.length;oment
        var dataEnviar = "";
        for (var i = 0; i < ncheckbox; i++) {
            if (checkbox[i].checked == true) {
                dataEnviar += checkbox[i].id;
                dataEnviar += "$";
            }
        }
        dataEnviar = dataEnviar.substring(0, dataEnviar.length - 1);
        frm.append("dataEnviar", dataEnviar);

        if (confirm("¿Desea realmente guardar?") == 1) {

            $.ajax({
                type: "POST",
                url: "RolPagina/guardarDatos",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {

                    if (data == 0) {
                        alert("Ocurrio un error");
                    }
                    else {
                        alert("Se ejecutó correctamente");
                        document.getElementById("btnCancelar").click();
                        listar();
                    }
                }
            });
        }
    }
    else {

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

    var frm = new FormData();
    frm.append("IIDCURSO", id);

    if (confirm("¿Desea realmente guardar?") == 1) {

        $.ajax({
            type: "POST",
            url: "Curso/eliminar",
            data: frm,
            contentType: false,
            processData: false,
            success: function (data) {
                if (data == -1) {
                    ("Ya existe el docente");
                }
                else if (data == 0) {
                    alert("Ocurrió un error");
                }
                else {
                    alert("Se ejecutó correctamente");
                    listar();
                    //document.getElementById("btnCancelar").click();
                }
            }
        });
    }
}