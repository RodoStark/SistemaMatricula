using MiPrimeraAplicacionWeb.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiPrimeraAplicacionWeb.Controllers
{
    [Seguridad]
    public class CursoController : Controller
    {
        // GET: Curso
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult recuperarDatos(int id){
            PruebaDataContext db = new PruebaDataContext();
            var lista = db.Curso.Where(p=>p.BHABILITADO.Equals(1)
            && p.IIDCURSO.Equals(id))
                .Select(p => new { p.IIDCURSO,p.NOMBRE,p.DESCRIPCION}).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);

        }

        public string mensaje()
        { return "Bienvenido al curso madafakar"; }

        public string saludo(string nombre)
        {
            return ("Hola como estas: " + nombre);
        }

        public string saludoCompleto(string nombre, string apellido)
        {
            return ("Hola como estas: " + nombre + " " + apellido);
        }

        public JsonResult listarCursos()
        {
            PruebaDataContext bd = new PruebaDataContext();
            var lista = bd.Curso.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new { p.IIDCURSO, p.NOMBRE, p.DESCRIPCION }).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult buscarCursoPorNombre(string nombre){
            PruebaDataContext db = new PruebaDataContext();
            var lista = db.Curso.Where(p => p.BHABILITADO.Equals(1) && p.NOMBRE.Contains(nombre))
                .Select(p => new { p.IIDCURSO, p.NOMBRE, p.DESCRIPCION }).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int guardarDatos(Curso curso) {
            PruebaDataContext db = new PruebaDataContext();
            int nregistrosAfectados = 0;
            try
            {
                if (curso.IIDCURSO == 0)
                {
                    int nveces = db.Curso.Where(p => p.NOMBRE.Equals(curso.NOMBRE)).Count();
                    if (nveces == 0)
                    {
                        db.Curso.InsertOnSubmit(curso);
                        db.SubmitChanges();
                        nregistrosAfectados = 1;
                    }
                    else 
                    {
                        nregistrosAfectados = -1;
                    }

                }
                else {
                    int nveces = db.Curso.Where(p => p.NOMBRE.Equals(curso.NOMBRE)&&!p.IIDCURSO.Equals(curso.IIDCURSO)).Count();

                    if (nveces == 0)
                    {
                        Curso cursoSel = db.Curso.Where(p => p.IIDCURSO.Equals(curso.IIDCURSO)).First();
                        cursoSel.NOMBRE = curso.NOMBRE;
                        cursoSel.DESCRIPCION = curso.DESCRIPCION;
                        db.SubmitChanges();
                        nregistrosAfectados = 1;

                    }
                    else 
                    {
                        nregistrosAfectados = -1;
                    }

                }
            }
            catch (Exception ex) 
            {
                nregistrosAfectados = 0;
            }
            return nregistrosAfectados;
        }

        public int eliminar(Curso curso) {
            int nregistrosAfectados = 0;
            PruebaDataContext db = new PruebaDataContext();
            try
            {
                Curso cursoSel = db.Curso.Where(p => p.IIDCURSO.Equals(curso.IIDCURSO)).First();                
                cursoSel.BHABILITADO = 0;                
                db.SubmitChanges();
                nregistrosAfectados = 1;
            }
            catch (Exception ex) { 
            }

            return nregistrosAfectados;

        }

    }
}