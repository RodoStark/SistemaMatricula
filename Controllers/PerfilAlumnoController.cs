using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiPrimeraAplicacionWeb.Controllers
{
    public class PerfilAlumnoController : Controller
    {
        // GET: PerfilAlumno
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListarPeriodosMatricula(int idPeriodo) 
        {
            PruebaDataContext db = new PruebaDataContext();
            int idUsusario = (int)Session["idUsuario"];
            Usuario usuario = db.Usuario.Where(p => p.IIDUSUARIO == idUsusario && p.TIPOUSUARIO.Equals('A')).First();
            int idAlumno = (int)usuario.IID;

            var lista = (from matricula in db.Matricula
                         join detalleMatricula in db.DetalleMatricula
                         on matricula.IIDMATRICULA equals
                         detalleMatricula.IIDMATRICULA
                         join curso in db.Curso 
                         on detalleMatricula.IIDCURSO equals curso.IIDCURSO
                         where matricula.IIDALUMNO == idAlumno
                         && matricula.IIDPERIODO == idPeriodo
                         && detalleMatricula.bhabilitado ==1
                         select new
                         {
                             matricula.IIDMATRICULA,
                             detalleMatricula.NOTA1,
                             detalleMatricula.NOTA2,
                             detalleMatricula.NOTA3,
                             detalleMatricula.NOTA4,
                             detalleMatricula.PROMEDIO
                         }).ToList();


            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarComboPeriodo() 
        {
            PruebaDataContext db = new PruebaDataContext();
            int idUsuario = (int)Session["idUsuario"];
            Usuario usuario = db.Usuario.Where(p=>p.IIDUSUARIO.Equals(idUsuario)&&p.TIPOUSUARIO.Equals('A')).First();

            int idAlumno = (int)usuario.IID;
            var listarPeriodo = (from matricula in db.Matricula
                                 join periodo in db.Periodo
                                 on matricula.IIDPERIODO equals periodo.IIDPERIODO
                                 where matricula.BHABILITADO == 1
                                 select new
                                 {
                                     periodo.IIDPERIODO,
                                     periodo.NOMBRE 
                                 }).Distinct();
            return Json(listarPeriodo, JsonRequestBehavior.AllowGet);
        }
    }
}