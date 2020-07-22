using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiPrimeraAplicacionWeb.Controllers
{
    public class PaginaController : Controller
    {
        // GET: Pagina
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListarPaginas() 
        {
            PruebaDataContext db = new PruebaDataContext();
            var lista = db.Pagina.Where(p=>p.BHABILITADO==1)
                        .Select(p => new {
                            p.IIDPAGINA,
                            p.MENSAJE,
                            p.CONTROLADOR,
                            p.ACCION
                        }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        

        public JsonResult RecuperarInformacion(int idPagina)
        {
            PruebaDataContext db = new PruebaDataContext();
            var pagina = db.Pagina.Where(p => p.IIDPAGINA.Equals(idPagina))
                .Select(p=>new {
                    p.IIDPAGINA,
                    p.MENSAJE,
                    p.CONTROLADOR,
                    p.ACCION
                }).First();

            return Json(pagina, JsonRequestBehavior.AllowGet);
        }

        public int guardarDatos(Pagina pagina)
        {
            PruebaDataContext db = new PruebaDataContext();
            int nregistrosAfectados = 0;
            try
            {
                if (pagina.IIDPAGINA == 0)
                {
                    int nveces = db.Pagina.Where(p => p.MENSAJE.Equals(pagina.MENSAJE)).Count();
                    if (nveces == 0)
                    {
                        db.Pagina.InsertOnSubmit(pagina);
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
                    int nveces = db.Pagina.Where(p => p.MENSAJE.Equals(pagina.MENSAJE) && !p.IIDPAGINA.Equals(pagina.IIDPAGINA)).Count();

                    if (nveces == 0)
                    {
                        Pagina paginaSel = db.Pagina.Where(p => p.IIDPAGINA.Equals(pagina.IIDPAGINA)).First();
                        paginaSel.MENSAJE= pagina.MENSAJE;
                        paginaSel.CONTROLADOR= pagina.CONTROLADOR;
                        paginaSel.ACCION= pagina.ACCION;
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
    }
}