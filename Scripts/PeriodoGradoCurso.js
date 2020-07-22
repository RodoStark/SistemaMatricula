listar();
function listar() {

    $.get("PeriodoGradoCurso/ListarPeriodoGradoCurso", function (data) {
        console.log(data);
        crearListado(["Id", "Periodo", "Grado", "Curso"], data);
    });
    $.get("PeriodoGradoCurso/ListarPeriodo", function (data) {
        llenarCombo(data, document.getElementById("cboPeriodo"), true);
    });
    $.get("PeriodoGradoCurso/ListarGrado", function (data) {
        llenarCombo(data, document.getElementById("cboGrado"), true);
    });
    $.get("PeriodoGradoCurso/ListarCurso", function (data) {
        llenarCombo(data, document.getElementById("cboCurso"), true);
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

function crearListado(arrayColumnas, data) {

    
    var contenido = "";
    contenido += "<table id='tabla-periodogradocurso' class='table'>";
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

    $("#tabla-periodogradocurso").dataTable({
        searching: false
    });
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
        $.get("PeriodoGradoCurso/RecuperarInformacion/?id=" + id, function (data) {

            console.log(data);
            document.getElementById("txtId").value = data[0].IID;
            document.getElementById("cboPeriodo").value = data[0].IIDPERIODO;
            document.getElementById("cboGrado").value = data[0].IIDGRADO;
            document.getElementById("cboCurso").value = data[0].IIDCURSO;
        });
    }
}

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


function agregar() {
    if (datosObligatorios()) {
        var frm = new FormData();
        var id = document.getElementById("txtId").value;
        var periodo = document.getElementById("cboPeriodo").value;
        var grado = document.getElementById("cboGrado").value;
        var curso = document.getElementById("cboCurso").value;


        frm.append("IID", id);
        frm.append("IIDPERIODO", periodo);
        frm.append("IIDGRADO", grado);
        frm.append("IIDCURSO", curso);
        frm.append("BHABILITADO", 1);


        if (confirm("¿Desea realmente guardar?") == 1) {

            $.ajax({
                type: "POST",
                url: "PeriodoGradoCurso/guardarDatos",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data == -1) {
                        alert("Ya existe ese registro");
                    }
                    else if (data == 1) {
                        listar();
                        alert("Se ejecutó correctamente");
                        document.getElementById("btnCancelar").click();
                    }
                    else {
                        alert("Ocurrió un error");
                    }
                }
            })
        }

    }
    else {

    }    
}

function eliminar(id) {

    if (confirm("Desea eliminar") == 1) {
        $.get("PeriodoGradoCurso/Eliminar/?id=" + id, function (data) {
            if (id == 0) {
                alert("Ocurrió un error");
            } else {
                alert("Se eliminó correctamente");
                listar();
            }
        });
    }
}