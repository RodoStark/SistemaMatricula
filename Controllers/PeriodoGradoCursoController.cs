using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiPrimeraAplicacionWeb.Controllers
{
    public class PeriodoGradoCursoController : Controller
    {
        // GET: PeriodoGradoCurso
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListarPeriodoGradoCurso() {
            PruebaDataContext db = new PruebaDataContext();
            var list = from pgc in db.PeriodoGradoCurso
                       join per in db.Periodo
                       on pgc.IIDPERIODO equals per.IIDPERIODO
                       join grad in db.Grado
                       on pgc.IIDGRADO equals grad.IIDGRADO
                       join cur in db.Curso
                       on pgc.IIDCURSO equals cur.IIDCURSO
                       where pgc.BHABILITADO.Equals(1)
                       select new
                       {
                           pgc.IID,
                           NOMBREPERIODO = per.NOMBRE,
                           NOMBREGRADO = grad.NOMBRE,
                           NOMBRECURSO = cur.NOMBRE
                       };

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperarInformacion(int id)
        {
            PruebaDataContext db = new PruebaDataContext();
            var list = db.PeriodoGradoCurso.Where(p => p.IID.Equals(id)).
                Select(p => new
                {
                    p.IID,
                    p.IIDPERIODO,
                    p.IIDGRADO,
                    p.IIDCURSO

                });

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarPeriodo()
        {
            PruebaDataContext db = new PruebaDataContext();
            var list = db.Periodo.Where(p => p.BHABILITADO.Equals(1)).
                Select(p => new
                {
                    IID = p.IIDPERIODO,
                    p.NOMBRE
                });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarCurso()
        {
            PruebaDataContext db = new PruebaDataContext();
            var list = db.Curso.Where(p => p.BHABILITADO.Equals(1)).
                Select(p => new
                {
                    IID = p.IIDCURSO,
                    p.NOMBRE
                });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarGrado()
        {
            PruebaDataContext db = new PruebaDataContext();
            var list = db.Grado.Where(p => p.BHABILITADO.Equals(1)).
                Select(p => new
                {
                    IID = p.IIDGRADO,
                    p.NOMBRE
                });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public int GuardarDatos(PeriodoGradoCurso oPeriodoGradoCurso){
            PruebaDataContext db = new PruebaDataContext();
            int nregistrosAfectados = 0;
            try
            {
                int id = oPeriodoGradoCurso.IID;
                if (oPeriodoGradoCurso.IID.Equals(0))
                {
                    int nveces = db.PeriodoGradoCurso.Where(p=>p.IIDCURSO.Equals(oPeriodoGradoCurso.IIDCURSO)
                    &&p.IIDGRADO.Equals(oPeriodoGradoCurso.IIDGRADO)
                    &&p.IIDPERIODO.Equals(oPeriodoGradoCurso.IIDPERIODO)).Count();
                    if (nveces == 0)
                    {
                        db.PeriodoGradoCurso.InsertOnSubmit(oPeriodoGradoCurso);
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
                    int nveces = db.PeriodoGradoCurso.Where(p => p.IIDCURSO.Equals(oPeriodoGradoCurso.IIDCURSO)
                    && p.IIDGRADO.Equals(oPeriodoGradoCurso.IIDGRADO)
                    && p.IIDPERIODO.Equals(oPeriodoGradoCurso.IIDPERIODO)
                    && !p.IID.Equals(oPeriodoGradoCurso.IID)).Count();

                    if (nveces == 0)
                    {
                        PeriodoGradoCurso obj = db.PeriodoGradoCurso.Where(p => p.IID.Equals(id)).First();
                        obj.IIDCURSO = oPeriodoGradoCurso.IIDCURSO;
                        obj.IIDGRADO = oPeriodoGradoCurso.IIDGRADO;
                        obj.IIDPERIODO = oPeriodoGradoCurso.IIDPERIODO;
                        db.SubmitChanges();
                        nregistrosAfectados = 1;
                    }
                    else
                    {
                        nregistrosAfectados = -1;
                    }
                }
            }
            catch 
            {
                nregistrosAfectados = 0;
            }
            return nregistrosAfectados;
        }

        public int Eliminar(int id) {
            PruebaDataContext db = new PruebaDataContext();
            int nregistrosAfectados = 0;
            try
            {
                PeriodoGradoCurso obj = db.PeriodoGradoCurso.Where(p => p.IID.Equals(id)).First();
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