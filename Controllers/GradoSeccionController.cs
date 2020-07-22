using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiPrimeraAplicacionWeb.Controllers
{
    public class GradoSeccionController : Controller
    {
        // GET: GradoSeccion
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListarGradoSeccion()
        {
            PruebaDataContext db = new PruebaDataContext();
            var lista = (from gradosec in db.GradoSeccion
                         join sec in db.Seccion
                         on gradosec.IIDSECCION equals sec.IIDSECCION
                         join grad in db.Grado
                         on gradosec.IIDGRADO equals grad.IIDGRADO
                         where gradosec.BHABILITADO.Equals(1)
                         select new
                         {
                             gradosec.IID,
                             NOMBREGRADO = grad.NOMBRE,
                             NOMBRESECCION = sec.NOMBRE
                         }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperarInformacion(int id)
        {
            PruebaDataContext db = new PruebaDataContext();
            var consulta = db.GradoSeccion.Where(p => p.IID.Equals(id)).
            Select(
            p => new
            {
                p.IID,
                p.IIDGRADO,
                p.IIDSECCION
            }
            );
            return Json(consulta, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarSeccion()
        {
            PruebaDataContext db = new PruebaDataContext();
            var lista = db.Seccion.Where(p => p.BHABILITADO.Equals(1)).
            Select(
                p => new
                {
                    IID = p.IIDSECCION,
                    p.NOMBRE
                });
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Listargrado()
        {
            PruebaDataContext db = new PruebaDataContext();
            var lista = db.Grado.Where(p => p.BHABILITADO.Equals(1)).
            Select(
                p => new
                {
                    IID = p.IIDGRADO,
                    p.NOMBRE
                });
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int GuardarDatos(GradoSeccion oGradoSeccion)
        {
            int nregistrosAfectados = 0;
            PruebaDataContext db = new PruebaDataContext();
            try
            {
                int id = oGradoSeccion.IID;
                if (id == 0)
                {
                    int nveces = db.GradoSeccion.Where(p => p.IIDGRADO.Equals(oGradoSeccion.IIDGRADO)
                    && p.IIDSECCION.Equals(oGradoSeccion.IIDSECCION)).Count();

                    if (nveces == 0)
                    {
                        db.GradoSeccion.InsertOnSubmit(oGradoSeccion);
                        db.SubmitChanges();
                        nregistrosAfectados = 1;
                    }
                    else 
                    {
                        nregistrosAfectados = 1;
                    }
                }
                else 
                {

                    int nveces = db.GradoSeccion.Where(p => p.IIDGRADO.Equals(oGradoSeccion.IIDGRADO)
                    && p.IIDSECCION.Equals(oGradoSeccion.IIDSECCION)&&!p.IID.Equals(oGradoSeccion.IID)).Count();

                    if (nveces == 0)
                    {
                        GradoSeccion obj = db.GradoSeccion.Where(p => p.IID.Equals(id)).First();
                        obj.IIDGRADO = oGradoSeccion.IIDGRADO;
                        obj.IIDSECCION = oGradoSeccion.IIDSECCION;
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
            int nregistrosAfectados = 0 ;
            PruebaDataContext db = new PruebaDataContext();
            try
            {
                GradoSeccion obj = db.GradoSeccion.Where(p => p.IID.Equals(id)).First();
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