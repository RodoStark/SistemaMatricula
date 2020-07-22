using MiPrimeraAplicacionWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiPrimeraAplicacionWeb.Filters
{
    public class Seguridad:ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var usuario = HttpContext.Current.Session["idUsuario"];
            List<string> controladores = Variable.Controladores.Select(p=>p.ToUpper()).ToList();            
            string nombreControlador = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            if (usuario == null /*|| !controladores.Contains(nombreControlador.ToUpper())*/)
            {
                filterContext.Result = new RedirectResult("~/Login/Index");
            }
            base.OnActionExecuted(filterContext);
        }

    }
}