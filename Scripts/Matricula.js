listar();

function listar() {

    $.get("Matricula/Listar", function (data) {  
        crearListado(["Id", "Periodo", "Grado","Seccion", "Alumno"], data);
    });
    $.get("Matricula/ListarPeriodos", function (data) {
        llenarCombo(data, document.getElementById("cboPeriodo"), true);
    });
    $.get("Matricula/ListarGradoSeccion", function (data) {
        llenarCombo(data, document.getElementById("cboGradoSeccion"), true);
    });
    $.get("Matricula/ListarAlumnos", function (data) {
        llenarCombo(data, document.getElementById("cboAlumno"), true);
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
    contenido += "<table id='tabla-matricula' class='table'>";
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
    borrarDatos();
    if (id != 0) {
        document.getElementById("tablacurso").innerHTML = "";
        $.get("Matricula/ObtenerMatricula/?id=" + id, function (data) {
            document.getElementById("cboAlumno").style.display = "none";
            document.getElementById("spnContenido").style.display = "none";
            document.getElementById("txtId").value = data.IIDMATRICULA;
            document.getElementById("cboPeriodo").value = data.IIDPERIODO;
            document.getElementById("cboGradoSeccion").value = data.IIDSECCION;
            document.getElementById("cboAlumno").value = data.IIDALUMNO;
        });
    } else {
        document.getElementById("cboAlumno").style.display = "block";
        document.getElementById("spnContenido").style.display = "block";
    }
    if (id != 0) {
        $.get("Matricula/ListarCursos/?id=" + id, function (data) {
            var contenido = "<tbody>";
            for (var i = 0; i < data.length; i++) {
                contenido += "<tr>";

                contenido += "<td>";
                if (data[i].bhabilitado == 1)
                    contenido += "<input class='checkbox' id=" + data[i].IIDCURSO + " type='checkbox' checked='true'>";
                else
                    contenido += "<input type='checkbox'" + data[i].IIDCURSO + "class='checkbox'/>";

                contenido += "</td>";


                contenido += "<td>";
                contenido += data[i].NOMBRE;
                contenido += "</td>";
                contenido += "</tr>";
            }
            contenido += "</tbody>";
            document.getElementById('tablacurso').innerHTML = contenido;
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

        var checkBoxes = document.getElementsByClassName("checkbox");
        if (checkBoxes.length == 0)
        {
            alert("No hay ni mothers seleccionado");
            return;
        }

        var c = 0;
        for (var i = 0; i < checkBoxes.length; i++) {
            if (checkBoxes[i].checked == true) {
                c++;
            }
        }
        if (c == 0)
        {
            alert("No ha seleccionado ningun curso");
            return;
        }

        var frm = new FormData();
        var id = document.getElementById("txtId").value;
        var periodo = document.getElementById("cboPeriodo").value;                
        var gradoseccion = document.getElementById("cboGradoSeccion").value;        
        console.log(gradoseccion);
        var alumno = document.getElementById("cboAlumno").value;        
        
        frm.append("IIDMATRICULA", id);
        frm.append("IIDPERIODO", periodo);
        frm.append("IIDGRADOSECCION", gradoseccion);        
        frm.append("IIDALUMNO", alumno);
        frm.append("BHABILITADO", 1);

        //Los campos habilitados
        var valorAEnviar = "";
        var valorADeshabilitar = "";
        var checkbox = document.getElementById('cboAlumno').value;
        var ncheckbox = checkbox.length;
        for (var i = 0; i < ncheckbox; i++)
        {
            if (checkbox[i].check == true) {
                valorAEnviar += checkbox[i].id;
                valorAEnviar += "$";
            }
            else
            {
                valorADeshabilitar += checkbox[i].id;
                valorADeshabilitar += "$";
            }
        }
        if (valorAEnviar !="")
            valorAEnviar = valorAEnviar.substring(0, valorAEnviar.length - 1);

        if (valorADeshabilitar != "")
            valorADeshabilitar = valorADeshabilitar.substring(0, valorADeshabilitar.length - 1);

        frm.append("valorAEnviar", valorAEnviar);
        frm.append("valorADeshabilitar", valorADeshabilitar);
        if (confirm("¿Desea realmente guardar?") == 1) {

            $.ajax({
                type: "POST",
                url: "Matricula/GuardarDatos",
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
            });
        }
    }
    else {

    }
}

function eliminar(id) {

    if (confirm("Desea eliminar") == 1) {
        $.get("Matricula/Eliminar/?id=" + id, function (data) {
            if (id == 0) {
                alert("Ocurrió un error");
                listar();
            } else {
                alert("Se eliminó correctamente");
                listar();
            }
        });
    }
}


function recuperar(idPeriodo, idGradoSeccion)
{    
    $.get("Matricula/ListarCursosPorPeriodoYGrado/?iidPeriodo=" + idPeriodo + "&iidGradoSeccion+" + idGradoSeccion, function (data) {
        var contenido = "<tbody>";
        for (var i = 0; i < data.length; i++) {
            contenido += "<tr>";
            contenido += "<td>";            
            contenido += "<input class='checkbox' id=" + data[i].IIDCURSO + " type='checkbox' checked='true'>";
            contenido += "</td>";
            contenido += "<td>";
            contenido += data[i].NOMBRE;
            contenido += "</td>";
            contenido += "</tr>";
        }
        contenido += "</tbody>";
        document.getElementById('tablacurso').innerHTML = contenido;
    });
}


var cboPeriodo = document.getElementById("cboPeriodo");
var cboGradoSeccion = document.getElementById("cboGradoSeccion");

cboPeriodo.onchange = function () {
    if (cboGradoSeccion.value != "" & cboPeriodo.value != "")
        recuperar(cboGradoSeccion.value, cboPeriodo.value); 
}
cboGradoSeccion.onchange = function () {
    if (cboGradoSeccion.value != "" & cboPeriodo.value != "")
        recuperar(cboGradoSeccion.value, cboPeriodo.value); 
}