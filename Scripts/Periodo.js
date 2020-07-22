$("#datepickerInicio").datepicker(
    {
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true
    }
);
$("#datepickerFin").datepicker(
    {
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true
    }
);

listar();

function listar() {
    $.get("Periodo/listarPeriodo", function (data) {
        crearListado(["ID Periodo", "Nombre", "Fecha Inicio", "Fecha Fin"], data);
    })
}




var nombrePeriodo = document.getElementById("txtnombre");
nombrePeriodo.onkeyup = function () {
    var nombre = document.getElementById("txtnombre").value;
    $.get("Periodo/buscarPeriodoPorNombre/?nombrePeriodo=" + nombre, function (data) {
        crearListado(data);
    });
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
        contenido += "<button class = 'btn btn-danger' onclick='eliminar(" + data[i][llaveId] + ")'data-toggle='modal' ><i class='glyphicon glyphicon-trash'></i></button>";
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


function borrarDatos() {
    var controles = document.getElementsByClassName("borrar");
    var ncontroles = controles.length;
    for (var i = 0; i < length; i++) {
        controles.value = "";
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

    if (confirm("¿Desea eliminar el registro?") == 1) {
        var frm = new FormData();
        frm.append("IIDPERIODO",id);
        $.ajax(
            {
                type:"POST",
                url: "Periodo/eliminar",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data == 0) {
                        alert("Ocurrió un error");
                    }
                    else {
                        alert("Se eliminó correctamente");
                        listar();
                    }
                }
            }
        );

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

        
        $.get("Periodo/recuperarInformacion/?id=" + id, function (data) {            
            console.log(data);
            document.getElementById("txtPeriodo").value = data[0].IIDPERIODO;
            document.getElementById("txtNombrePopup").value = data[0].NOMBRE;            
            document.getElementById("datepickerInicio").value = data[0].FECHAINICIOCADENA;                               
            document.getElementById("datepickerFin").value = data[0].FECHAFINCADENA;                               
        })
    }
}







function agregar() {

    if (datosObligatorios())
    {
        var frm = new FormData();
        var idperiodo = document.getElementById("txtPeriodo").value;
        var nombre = document.getElementById("txtNombrePopup").value;
        var fechaInicio = document.getElementById("datepickerInicio").value;
        var fechaFin = document.getElementById("datepickerFin").value;

        frm.append("IIDPERIODO", idperiodo);
        frm.append("NOMBRE", nombre);
        frm.append("FECHAINICIO", fechaInicio);
        frm.append("FECHAFIN", fechaFin);
        frm.append("BHABILITADO", 1);

        if (confirm("¿Desea realizar la operación?") == 1) {
            $.ajax({
                type: "POST",
                url: "Periodo/guardarDatos",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data != 0) {
                        listar();
                        alert("Se ejecutó correctamente");
                        document.getElementById("btnCancelar").click();
                    } else {
                        if (data == -1) {
                            alert("Ya existe el periodo");
                        } else
                        {
                            alert("Ocurrió un error");
                        }                    
                    }
                }
            });
        }

    }
    else
    {
    }
}