using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiPrimeraAplicacionWeb.Controllers
{
    public class PaginaPrincipalController : Controller
    {
        // GET: PaginaPrincipal
        public ActionResult Index()
        {
            int idUsuario = (int) Session["idUsuario"];
            

            using (PruebaDataContext db = new PruebaDataContext()) 
            {
                string nombreCompleto = "";
                Usuario user = db.Usuario.Where(p => p.IIDUSUARIO == idUsuario).First();
                if (user.TIPOUSUARIO == 'D')
                {
                    Docente docente = db.Docente.Where(p => p.IIDDOCENTE == user.IID).First();
                    nombreCompleto = docente.NOMBRE + " " + docente.APPATERNO + " " + docente.APMATERNO;
                    ViewBag.nombreCompleto = nombreCompleto;
                    ViewBag.nombreCompleto = nombreCompleto;
                }
                else 
                {
                    Alumno alumno = db.Alumno.Where(p => p.IIDALUMNO== user.IID).First();
                    nombreCompleto = alumno.NOMBRE + " " + alumno.APPATERNO + " " + alumno.APMATERNO;
                }
            }

            
            return View();  
        }
    }
}