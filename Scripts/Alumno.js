$("#dtFechaNacimiento").datepicker(
    {
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true
    }
);

listar();
function listar() {
    $.get("Alumno/listarAlumnos", function (data) {
        crearListado(["Id", "Nombre", "Apellido Paterno", "Apellido Materno", "Telefono Padre"], data);
    });
}


$.get("Alumno/listarSexo", function (data) {
    llenarCombo(data, document.getElementById('cboSexo'), true);
    llenarCombo(data, document.getElementById('cboSexoPopup'), true);
});


var btnBuscar = document.getElementById("btnBuscar");


btnBuscar.onclick = function () {
    var iidsexo = document.getElementById("cboSexo").value;    
    if (iidsexo == "") {
        listar();
    }
    else {
        $.get("Alumno/filtrarAlumnoPorSexo/?iidsexo=" + iidsexo, function (data) {

            crearListado(["Id", "Nombre", "Apellido Paterno", "Apellido Materno", "Telefono Padre"], data);
        });
    }
}

var btnLimpiar = document.getElementById("btnLimpiar");
btnLimpiar.onclick = function () {
    listar();
}



function llenarCombo(data, control,primerElemento){
    var contenido = "";
    if (primerElemento == true) {
        contenido += "<option value=''>--Seleccione--</option>";
    }

    for (var i = 0; i < data.length; i++) {
        contenido += "<option value= '" + data[i].IID + "'>";
        contenido += data[i].NOMBRE;
        contenido += "</option>";
    }
    console.log(contenido);
    control.innerHTML = contenido;
}




function crearListado(arrayColumnas, data) {

    var contenido = "";
    contenido += "<table id='tabla-alumno' class= 'table'>";
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

    $("#tabla-alumno").dataTable({
        searching: false
    });
}



function eliminar(id) {
    if (confirm("Desea eliminar?") == 1) {
        $.get("Alumno/eliminar?id= " + id, function (data) {
            if (data == 0) {
                alert("Ocurrió un error")
            }
            else {
                alert("Se eliminó correctamente");
                listar();
            }
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



function abrirModal(id) {


    var controlesObligatorios = document.getElementsByClassName("obligatorio");
    var ncontroles = controlesObligatorios.length;
    for (var i = 0; i < ncontroles; i++) {
        controlesObligatorios[i].parentNode.classList.remove("error");
    }


    if (id == 0) {
        borrarDatos();
        document.getElementById("lblTitulo").innerHTML = "Agregar Alumno";
    }
    else {
        $.get("Alumno/recuperarInformacion/?id=" + id, function (data) {
            document.getElementById("lblTitulo").innerHTML = "Editar Alumno";
            console.log(data);
            document.getElementById("txtIdAlumno").value = data[0].IIDALUMNO;
            document.getElementById("txtNombre").value = data[0].NOMBRE;
            document.getElementById("txtApellidoPaterno").value = data[0].APPATERNO;

            document.getElementById("txtApellidoMaterno").value = data[0].APMATERNO;
            document.getElementById("cboSexoPopup").value = data[0].IIDSEXO;
            document.getElementById("dtFechaNacimiento").value = data[0].FECHANAC;

            document.getElementById("txtTelefonoPadre").value = data[0].TELEFONOPADRE;
            document.getElementById("txtTelefonoMadre").value = data[0].TELEFONOMADRE;
            document.getElementById("txtNumhermanos").value = data[0].NUMEROHERMANOS;
        });
    }  
}


function agregar(){

    if (datosObligatorios()) {

        var frm = new FormData();
        var idAlumno = document.getElementById("txtIdAlumno").value;
        var nombre = document.getElementById("txtNombre").value;
        var apPaterno = document.getElementById("txtApellidoPaterno").value;
        var apMaterno = document.getElementById("txtApellidoMaterno").value;

        var fechaNac = document.getElementById("dtFechaNacimiento").value;
        var idSexo = document.getElementById("cboSexoPopup").value;
        var telefonoPadre = document.getElementById("txtTelefonoPadre").value;
        var telefonoMadre = document.getElementById("txtTelefonoMadre").value;
        var numeroHermanos = document.getElementById("txtNumhermanos").value;

        frm.append("IIDALUMNO", idAlumno);
        frm.append("NOMBRE", nombre);
        frm.append("APPATERNO", apPaterno);
        frm.append("APMATERNO", apMaterno);

        frm.append("FECHANACIMIENTO", fechaNac);
        frm.append("IIDSEXO", idSexo);
        frm.append("TELEFONOPADRE", telefonoPadre);
        frm.append("TELEFONOMADRE", telefonoMadre);
        frm.append("NUMEROHERMANOS", numeroHermanos);

        frm.append("BHABILITADO", 1);

        if (confirm("¿Desea guardar los cambios?") == 1) {

            $.ajax({
                type: "POST",
                url: "Alumno/guardarDatos",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data == -1) {
                        alert("Ya existe el alumno");
                    }
                    else if (data == 0) {
                        alert("Ocurrio error");
                    }
                    else
                    {
                        alert('Se ejecuto correctamente');
                        listar();
                        document.getElementById("btnCancelar").click();
                    }
                }
            });
        }

    }
    else { alert('cayo aqui el mf'); }
}