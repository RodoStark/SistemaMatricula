using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace MiPrimeraAplicacionWeb.Controllers
{
    public class RolPaginaController : Controller
    {
        // GET: RolPagina
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListarRol() 
        {
            using (PruebaDataContext db = new PruebaDataContext()) 
            {
                var list = db.Rol.Where(p => p.BHABILITADO == 1).
                    Select(p => new
                    {
                        p.IIDROL,
                        p.NOMBRE,
                        p.DESCRIPCION
                    }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ListarPaginas() 
        {
            using (PruebaDataContext db = new PruebaDataContext())
            {
                var list = db.Pagina.Where(p => p.BHABILITADO == 1).
                    Select(p => new
                    {
                        p.IIDPAGINA,
                        p.MENSAJE,
                        p.BHABILITADO
                    }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ObtenerRol(int idRol)
        {
            using (PruebaDataContext db = new PruebaDataContext())
            {
                var list = db.Rol.Where(p => p.IIDROL== idRol).
                    Select(p => new
                    {
                        p.IIDROL,
                        p.NOMBRE,
                        p.DESCRIPCION
                    }).First();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        public int guardarDatos(Rol oRolCLS, String dataAEnviar) 
        {
            int respuesta = 0;
            try 
            {
                using (PruebaDataContext db = new PruebaDataContext()) 
                {
                    using (var transaccion = new TransactionScope()) 
                    {
                        if (oRolCLS.IIDROL == 0)
                        {
                            Rol rol = new Rol();
                            rol.NOMBRE = oRolCLS.NOMBRE;
                            rol.DESCRIPCION = oRolCLS.DESCRIPCION;
                            rol.BHABILITADO = oRolCLS.BHABILITADO;
                            db.Rol.InsertOnSubmit(rol);
                            db.SubmitChanges();

                            String[] codigo = dataAEnviar.Split('$');
                            for (int i = 0; i < codigo.Length; i++)
                            {
                                RolPagina rolPagina = new RolPagina();
                                rolPagina.IIDROL = rol.IIDROL;
                                rolPagina.IIDPAGINA = int.Parse(codigo[i]);
                                rolPagina.BHABILITADO = 1;
                                db.RolPagina.InsertOnSubmit(rolPagina);
                            }
                            respuesta = 1;
                            db.SubmitChanges();
                            transaccion.Complete();
                        }
                        else 
                        {
                            //Modificamos
                            Rol rol = db.Rol.Where(p => p.IIDROL == oRolCLS.IIDROL).First();
                            rol.NOMBRE = oRolCLS.NOMBRE;
                            rol.DESCRIPCION = oRolCLS.DESCRIPCION;
                            //Deshabilitar todo
                            var lista = db.RolPagina.Where(p => p.IIDROL == oRolCLS.IIDROL);
                            foreach (RolPagina oRolPagina in lista) 
                            {
                                oRolPagina.BHABILITADO = 0;
                            }
                            String[] codigo = dataAEnviar.Split('$');
                            for (int i = 0; i < codigo.Length; i++)
                            {
                                int cantidad = db.RolPagina.Where(p => p.IIDROL == oRolCLS.IIDROL && p.IIDPAGINA == int.Parse(codigo[i])).Count();
                                if (cantidad == 0)
                                {
                                    RolPagina rolPagina = new RolPagina();
                                    rolPagina.IIDROL = rol.IIDROL;
                                    rolPagina.IIDPAGINA = int.Parse(codigo[i]);
                                    rolPagina.BHABILITADO = 1;
                                    db.RolPagina.InsertOnSubmit(rolPagina);
                                }
                                else 
                                {
                                    RolPagina rolPagina = db.RolPagina.Where(p => p.IIDROL == oRolCLS.IIDROL 
                                    && p.IIDPAGINA == int.Parse(codigo[i])).First();
                                    rolPagina.BHABILITADO = 1;
                                }
                            }
                            respuesta = 1;
                            db.SubmitChanges();
                            transaccion.Complete();
                        }
                    }
                }
            }
            catch 
            {
                respuesta = 0;
            }
            return respuesta;
        }

        public JsonResult ListarRolPagina(int idRol) 
        {
            using (PruebaDataContext db = new PruebaDataContext()) 
            {
                var lista = db.RolPagina.Where(p => p.IIDROL == idRol && p.BHABILITADO == 1)
                    .Select(x => new
                    {
                        x.IIDROL,
                        x.IIDPAGINA,
                        x.BHABILITADO
                    }).ToList(); ;
                return Json(lista, JsonRequestBehavior.AllowGet);
            }
        }
    }
}