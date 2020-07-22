using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiPrimeraAplicacionWeb.Controllers
{
    public class PeriodoController : Controller
    {
        // GET: Periodo
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult listarPeriodo()
        {
            PruebaDataContext db = new PruebaDataContext();
            var lista = (db.Periodo.Where(p => p.BHABILITADO.Equals(1)).Select(p => new { p.IIDPERIODO, p.NOMBRE, FECHAINICIO = ((DateTime)p.FECHAINICIO).ToShortDateString(), FECHAFIN = ((DateTime)p.FECHAFIN).ToShortDateString() })).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult buscarPeriodoPorNombre(string nombrePeriodo)
        {
            PruebaDataContext db = new PruebaDataContext();
            var lista = (db.Periodo.Where(p => p.BHABILITADO.Equals(1)&&p.NOMBRE.Contains(nombrePeriodo)).Select(p => new { p.IIDPERIODO, p.NOMBRE, FECHAINICIO = ((DateTime)p.FECHAINICIO).ToShortDateString(), FECHAFIN = ((DateTime)p.FECHAFIN).ToShortDateString() })).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int eliminar(Periodo periodo)
        {
            PruebaDataContext db = new PruebaDataContext();

            int nregistrosAfectados = 0;
            try 
            {
                int idPediodo = periodo.IIDPERIODO;
                Periodo obj = db.Periodo.Where(p => p.IIDPERIODO.Equals(idPediodo)).First();
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

        public JsonResult recuperarInformacion(int id) {
            PruebaDataContext db = new PruebaDataContext();
            var lista = db.Periodo.Where(p => p.IIDPERIODO.Equals(id)).Select(
                p=>new
                {
                    p.IIDPERIODO,
                    p.NOMBRE,
                    FECHAINICIOCADENA = ((DateTime)p.FECHAINICIO).ToShortDateString(),
                    FECHAFINCADENA = ((DateTime)p.FECHAFIN).ToShortDateString(),
                }
                );
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int guardarDatos(Periodo periodo)
        {
            PruebaDataContext db = new PruebaDataContext();
            int nregistrosAfectados = 0;

            try
            {
                int idPeriodo = periodo.IIDPERIODO;
                if (idPeriodo >= 1)
                {
                    int nveces = db.Periodo.Where(p => p.NOMBRE.Equals(periodo.NOMBRE) && !p.IIDPERIODO.Equals(idPeriodo)).Count();
                    if (nveces == 0)
                    {
                        Periodo obj = db.Periodo.Where(p => p.IIDPERIODO.Equals(idPeriodo)).First();
                        obj.NOMBRE = periodo.NOMBRE;
                        obj.FECHAINICIO = periodo.FECHAINICIO;
                        obj.FECHAFIN = periodo.FECHAFIN;
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
                    int nveces = db.Periodo.Where(p => p.NOMBRE.Equals(periodo.NOMBRE)).Count();
                    if (nveces == 0)
                    {
                        db.Periodo.InsertOnSubmit(periodo);
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

    }



}