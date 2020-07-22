using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiPrimeraAplicacionWeb.Controllers
{
    public class GradoSeccionAulaController : Controller
    {
        // GET: GradoSeccionAula
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Listar()
        {
            PruebaDataContext db = new PruebaDataContext();
            var lista = (from gradoseccionaula in db.GradoSeccionAula
                         join periodo in db.Periodo
                         on gradoseccionaula.IIDPERIODO equals periodo.IIDPERIODO
                         join grad in db.Grado
                         on gradoseccionaula.IIDGRADOSECCION equals grad.IIDGRADO
                         join aula in db.Aula
                         on gradoseccionaula.IIDAULA equals aula.IIDAULA
                         join curso in db.Curso
                         on gradoseccionaula.IIDCURSO equals curso.IIDCURSO
                         join docente in db.Docente
                         on gradoseccionaula.IIDDOCENTE equals docente.IIDDOCENTE
                         where gradoseccionaula.BHABILITADO.Equals(1)
                         select new
                         {
                             gradoseccionaula.IID,
                             NOMBREPERIODO = periodo.NOMBRE,
                             NOMBRESECCION = grad.NOMBRE,                             
                             NOMBRECURSO = curso.NOMBRE,
                             NOMBREDOCENTE = docente.NOMBRE,
                             NOMBREAULA = aula.NOMBRE

                         }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ListarPeriodos() 
        {
            PruebaDataContext db = new PruebaDataContext();
            var list = db.Periodo.Where(p => p.BHABILITADO.Equals(1)).
                Select(p => new
                {
                    IID = p.IIDPERIODO,
                    p.NOMBRE
                });
            return Json(list,JsonRequestBehavior.AllowGet);
        }


        public JsonResult ListarGradoSeccion()
        {
            PruebaDataContext db = new PruebaDataContext();
            var list = from gs in db.GradoSeccion
                       join grado in db.Grado
                       on gs.IIDGRADO equals grado.IIDGRADO
                       join seccion in db.Seccion
                       on gs.IIDSECCION equals seccion.IIDSECCION
                       select new
                       {
                           gs.IID,
                           NOMBRE = grado.NOMBRE + " - " + seccion.NOMBRE
                       };
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarAulas()
        {
            PruebaDataContext db = new PruebaDataContext();
            var list = db.Aula.Where(p => p.BHABILITADO.Equals(1)).
                Select(p => new
                {
                    IID = p.IIDAULA,
                    p.NOMBRE
                });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarDocentes()
        {
            PruebaDataContext db = new PruebaDataContext();
            var list = db.Docente.Where(p => p.BHABILITADO.Equals(1)).
                Select(p => new
                {
                    IID = p.IIDDOCENTE,
                    p.NOMBRE
                });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarCursos(int IIDPERIODO, int IIDGRADOSECCION) 
        {
            PruebaDataContext db = new PruebaDataContext();
            int iidGrado = (int) db.GradoSeccion.Where(p => p.IID.Equals(IIDGRADOSECCION)).First().IIDGRADO;
            var list = from pgc in db.PeriodoGradoCurso
                       join curso in db.Curso
                       on pgc.IIDCURSO equals curso.IIDCURSO
                       join periodo in db.Periodo
                       on pgc.IIDPERIODO equals periodo.IIDPERIODO
                       where pgc.BHABILITADO.Equals(1)
                       && pgc.IIDPERIODO.Equals(IIDPERIODO)
                       && pgc.IIDGRADO.Equals(iidGrado)
                       select new
                       {
                           IID = pgc.IIDCURSO,
                           curso.NOMBRE
                       };
             return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperarInformacion(int id) 
        {
            PruebaDataContext db = new PruebaDataContext();
            var lista = db.GradoSeccionAula.Where(p => p.IID.Equals(id)).Select(
                p => new
                {
                    p.IID,
                    p.IIDPERIODO,
                    p.IIDGRADOSECCION,
                    p.IIDCURSO,
                    p.IIDAULA,
                    p.IIDDOCENTE
                });
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int GuardarDatos(GradoSeccionAula oGradoSeccionAula)
        {
            int nregistrosAfectados = 0;
            PruebaDataContext db = new PruebaDataContext();
            try
            {
                int id = oGradoSeccionAula.IID;
                if (oGradoSeccionAula.Equals(0))
                {
                    int nveces = db.GradoSeccionAula.Where(p=>
                    p.IIDCURSO.Equals(oGradoSeccionAula.IIDCURSO)
                    && p.IIDPERIODO.Equals(oGradoSeccionAula.IIDPERIODO)
                    && p.IIDGRADOSECCION.Equals(oGradoSeccionAula.IIDGRADOSECCION)).Count();

                    if (nveces == 0)
                    {
                        db.GradoSeccionAula.InsertOnSubmit(oGradoSeccionAula);
                        db.SubmitChanges();
                        nregistrosAfectados = 1;
                    }
                    else 
                    {
                        nregistrosAfectados = -1;
                    }
                }
                else
                {
                    int nveces = db.GradoSeccionAula.Where(p =>/* p.IIDAULA.Equals(oGradoSeccionAula.IIDAULA)
                    &&*/ p.IIDCURSO.Equals(oGradoSeccionAula.IIDCURSO)
                    && p.Periodo.Equals(oGradoSeccionAula.IIDPERIODO)
                    && p.IIDGRADOSECCION.Equals(oGradoSeccionAula.IIDGRADOSECCION)
                    && !p.IID.Equals(oGradoSeccionAula.IID)).Count();

                    if (nveces == 0)
                    {
                        GradoSeccionAula obj = db.GradoSeccionAula.Where(p => p.IID.Equals(id)).First();
                        obj.IID = oGradoSeccionAula.IID;
                        obj.IIDPERIODO = oGradoSeccionAula.IIDPERIODO;
                        obj.IIDGRADOSECCION = oGradoSeccionAula.IIDGRADOSECCION;
                        obj.IIDCURSO = oGradoSeccionAula.IIDCURSO;
                        obj.IIDAULA = oGradoSeccionAula.IIDAULA;
                        obj.IIDDOCENTE = oGradoSeccionAula.IIDDOCENTE;
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


        public int Eliminar(int id)
        {
            int nregistrosAfectados = 0;
            PruebaDataContext db = new PruebaDataContext();
            try
            {
                GradoSeccionAula obj = db.GradoSeccionAula.Where(p => p.IID.Equals(id)).First();
                obj.BHABILITADO = 0;
                db.SubmitChanges();
                nregistrosAfectados = 1;
            }
            catch
            {
                nregistrosAfectados = 0;
            }
            return nregistrosAfectados;
        }


    }
}