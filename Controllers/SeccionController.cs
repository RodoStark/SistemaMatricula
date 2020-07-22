using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiPrimeraAplicacionWeb.Controllers
{
    public class SeccionController : Controller
    {
        // GET: Seccion
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult listarSeccion()
        {
            PruebaDataContext db = new PruebaDataContext();
            var lista = db.Seccion.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new
                {
                    p.IIDSECCION,
                    p.NOMBRE
                });
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
    }
}