$("#dtFechaContrato").datepicker(
    {
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true
    }
);

listar();
listarComboModalidad();

function listarComboModalidad() {
    var modalidad = document.getElementById("cboTipoModalidad").value;
    $.get("Docente/listarModalidadContratato/", function (data) {            
        llenarCombo(data, document.getElementById('cboTipoModalidad'), true);
        llenarCombo(data, document.getElementById('cboModalidadContratoPopUp'), true); 
    })    
}


var cboTipoModalidad = document.getElementById("cboTipoModalidad");
cboTipoModalidad.onchange = function () {

    var iidmodalidad = document.getElementById("cboTipoModalidad").value;
    if (iidmodalidad == "") { listar();}
    else {
        $.get("Docente/filtrarDocentePorModalidad/?iidmodalidad=" + iidmodalidad, function (data) {
            crearListado(["Id Docente", "Nombre", "Apellido Paterno", "Apellido Materno", "Email"], data);
        });
    }
}



function listar() {
    $.get("Docente/listarDocente", function (data) {
        crearListado(["Id Docente", "Nombre", "Apellido Paterno", "Apellido Materno", "Email"], data);
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
        $.get("Docente/recuperarInformacion/?id=" + id, function (data) {
            console.log(data);
            document.getElementById("txtIdDocente").value = data[0].IIDDOCENTE;
            document.getElementById("txtNombre").value = data[0].NOMBRE;
            document.getElementById("txtApellidoPaterno").value = data[0].APPATERNO;

            document.getElementById("txtApellidoMaterno").value = data[0].APMATERNO;
            document.getElementById("txtDireccion").value = data[0].DIRECCION;
            document.getElementById("txtCelular").value = data[0].TELEFONOCELULAR;

            document.getElementById("txtTelefono").value = data[0].TELEFONOFIJO;
            document.getElementById("txtEmail").value = data[0].EMAIL;
            document.getElementById("cboSexoPopup").value = data[0].IIDSEXO;
            document.getElementById("dtFechaContrato").value = data[0].FECHACONTRACT;
            document.getElementById("cboModalidadContratoPopUp").value = data[0].IIDMODALIDADCONTRATO;
            document.getElementById("imgFoto").src = "data:image/png;base64," + data[0].FOTOMOSTRAR;
        });
    }
}

function agregar() {



    if (datosObligatorios()) {

        var frm = new FormData();
        var idDocente = document.getElementById("txtIdDocente").value;
        var nombre = document.getElementById("txtNombre").value;
        var apPaterno = document.getElementById("txtApellidoPaterno").value;
        var apMaterno = document.getElementById("txtApellidoMaterno").value;
        var direccion = document.getElementById("txtDireccion").value;
        var celular = document.getElementById("txtCelular").value;
        var telefono = document.getElementById("txtTelefono").value;        
        var email = document.getElementById("txtEmail").value;
        var idSexo= document.getElementById("cboSexoPopup").value;
        var fechaContrato = document.getElementById("dtFechaContrato").value;
        var contrato = document.getElementById("cboModalidadContratoPopUp").value;
        var imgFoto = document.getElementById("imgFoto").src.replace("data:image/png;base64,","");
       


        frm.append("IIDDOCENTE", idDocente);
        frm.append("NOMBRE", nombre);
        frm.append("APPATERNO", apPaterno);
        frm.append("APMATERNO", apMaterno);
        frm.append("DIRECCION", direccion);
        frm.append("TELEFONOCELULAR", celular);
        frm.append("TELEFONOFIJO", telefono);
        frm.append("EMAIL", email);
        frm.append("IIDSEXO", idSexo);
        frm.append("FECHACONTRATO", fechaContrato);
        frm.append("IIDMODALIDADCONTRATO", contrato);
        

        console.log(frm);

        if (confirm("¿Desea guardar los cambios?") == 1) {

            $.ajax({
                type: "POST",
                url: "Docente/guardarDatos",
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
    else { alert('cayo aqui el mf'); }
}

var btnFoto = document.getElementById("btnFoto");
btnFoto.onchange = function (e) {
    var file = document.getElementById("btnFoto").files[0];
    var reader = new FileReader();
    console.log(reader);    
    if (reader != null) {
        reader.onloadend = function () {
            var img = document.getElementById("imgFoto");
            img.src = reader.result;
            alert(reader.result.replace("data:image/png;base64,",""));
        }
    }
    reader.readAsDataURL(file);
}
