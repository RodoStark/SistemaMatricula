var btnIngresar = document.getElementById("ingresar");
btnIngresar.onclick = function () {
    var usuario = document.getElementById("usuario").value;
    var contra = document.getElementById("contra").value;

    if (usuario == "")
    {
        alert("Agrega un p**0 usuario");
        return;
    }

    if (contra == "") {
        alert("Escribe la p**@ contra");
        return;
    }

    $.get("Login/ValidarUsuario/?usuario=" + usuario + "&contra=" + contra, function (data) {    
        if (data == 1) {
            document.location.href = "PaginaPrincipal/Index";
        }
        else
        {
            alert("No existe alv el usuario y me vale bhe");
        }
    });
}