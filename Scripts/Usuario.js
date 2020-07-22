listar();

function listar() {
    $.get("Usuario/ListarUsuarios", function (data) {        
        console.log(data);
        var size = Object.keys(data).length;
        console.log(size);
        if (size > 0)
            crearListado(["Id Usuario", "Nombre Usuario", "Nombre Completo", "Rol", "Tipo"], data);
        else
            alert('Chtm :D');
    });
    $.get("Usuario/ListarRol", function (data) {
        llenarCombo(data, document.getElementById("cboRol"), true);
    });
    $.get("Usuario/ListarPersonas", function (data) {
        llenarCombo(data, document.getElementById("cboPersona"), true);
    });
}

function llenarCombo(data, control, primerElemento) {
    var contenido = "";
    if (primerElemento == true){
        contenido += "<option value=''>--Seleccione--</option>";
    }

    for (var i = 0; i < data.length; i++) {
        contenido += "<option value= '" + data[i].IID + "'>";
        contenido += data[i].NOMBRE;
        contenido += "</option>";
    }
    control.innerHTML = contenido;
}


function abrirModal(id) {
    var controlesObligatorios = document.getElementsByClassName("obligatorio");
    var ncontroles = controlesObligatorios.length;
    for (var i = 0; i < ncontroles; i++){
        controlesObligatorios[i].parentNode.classList.remove("error");
    }

    if (id == 0) {
        document.getElementById("lblContra").style.display = "block"; 
        document.getElementById("txtContra").style.display = "block"; 
        document.getElementById("lblPersona").style.display = "block"; 
        document.getElementById("cboPersona").style.display = "block"; 
        borrarDatos();
    }

    else {
        document.getElementById("txtContra").value = "1";
        document.getElementById("cboPersona").value = "2";
        document.getElementById("lblContra").style.display = "none";
        document.getElementById("txtContra").style.display = "none";
        document.getElementById("lblPersona").style.display = "none";
        document.getElementById("cboPersona").style.display = "none"; 

        $.get("Usuario/RecuperarInformacion/?idUsuario=" + id, function (data) {            
            document.getElementById("txtIdUsuario").value = data.IIDUSUARIO;            
            document.getElementById("txtNombreUsuario").value = data.NOMBREUSUARIO;
            document.getElementById("cboRol").value = data.IIDROL;
        });
    }
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
        var idUsuario = 0;//document.getElementById("txtIdUsuario").value;
        var nombreUsuario = document.getElementById("txtUsuario").value;
        var contra = document.getElementById("txtContra").value;
        var persona = document.getElementById("cboPersona").value;
        var rol = document.getElementById("cboRol").value;
        var nombrePersona = document.getElementById("cboPersona").options[document.getElementById("cboPersona").selectedIndex].text;
        frm.append("IIDUSUARIO", idUsuario);
        frm.append("NOMBREUSUARIO", nombreUsuario);
        frm.append("CONTRA", contra);
        frm.append("IID", persona);
        frm.append("IIDROL", rol);
        frm.append("nombreCompleto", nombrePersona);
        frm.append("BHABILITADO",1);

        if (confirm("¿Desea realmente guardar?") == 1) {
            $.ajax({
                type: "POST",
                url: "Usuario/GuardarDatos",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data == 1) {
                        alert("Se guardo correctamente");
                        document.getElementById("btnCancelar").click();
                        listar();
                    }
                    else
                    {
                        alert("Ya valio mothers");
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
    console.log(data);
    var llaves = Object.keys(data[0]);
    contenido += "<tbody>";
    for (var i = 0; i < data.length; i++){
        contenido += "<tr>";
        for (var j = 0; j < llaves.length; j++) {
            var valorLlave = llaves[j];
            contenido += "<td>";
            contenido += data[i][valorLlave];
            contenido += "</td>";
        }
        var llaveId = llaves[0];
        contenido += "<td>"
        contenido += "<button class = 'btn btn-primary' onclick ='abrirModal(" + data[i][llaveId] + ")' data-toggle='modal' data-target='#myModal'><i class='glyphicon glyphicon-edit'></i></button> ";
        contenido += "<button class = 'btn btn-danger' onclick='eliminar(" + data[i][llaveId] + ")'data-toggle='modal' data-target='#myModal'><i class='glyphicon glyphicon-trash'></i></button>";
        contenido += "</td>";
        contenido += "</tr>";
    }
    contenido += "</tbody>";
    contenido += "</table>";
    document.getElementById("tabla").innerHTML = contenido;
    $("#tablas").dataTable({
        searching: false
    });
}