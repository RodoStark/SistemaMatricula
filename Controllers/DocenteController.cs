using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiPrimeraAplicacionWeb.Controllers
{
    public class DocenteController : Controller
    {
        // GET: Docente
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult listarDocente()
        {
            PruebaDataContext db = new PruebaDataContext();

            var lista = db.Docente.Where(p => p.BHABILITADO.Equals(1)).
                Select(p => new
                {
                            p.IIDDOCENTE,
                            p.NOMBRE,
                            p.APPATERNO,
                            p.APMATERNO,
                            p.EMAIL
                }).ToList();
                
            return Json(lista,JsonRequestBehavior.AllowGet);
        }


        public JsonResult filtrarDocentePorModalidad(int iidmodalidad)
        {
            PruebaDataContext db = new PruebaDataContext();

            var lista = db.Docente.Where(p => p.BHABILITADO.Equals(1)&&p.IIDMODALIDADCONTRATO.Equals(iidmodalidad)).
                Select(p => new
                {
                    p.IIDDOCENTE,
                    p.NOMBRE,
                    p.APPATERNO,
                    p.APMATERNO,
                    p.EMAIL
                }).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }


        public JsonResult listarModalidadContratato()
        {
            PruebaDataContext db = new PruebaDataContext();
            var lista = db.ModalidadContrato.Where(p => p.BHABILITADO.Equals(1)).
                Select(p=>new
                {   
                    IID = p.IIDMODALIDADCONTRATO,
                    p.NOMBRE                    
                }).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int eliminar(int id) {

            PruebaDataContext db = new PruebaDataContext();
            int nregistrosAfectados = 0;

            try 
            {
                Docente docente = db.Docente.Where(p => p.IIDDOCENTE.Equals(id)).First();
                docente.BHABILITADO = 0;
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
            var lista = db.Docente.Where(p => p.IIDDOCENTE.Equals(id)).Select(
                p => new
                {
                    p.IIDDOCENTE,
                    p.NOMBRE,
                    p.APPATERNO,
                    p.APMATERNO,                    
                    p.DIRECCION,
                    p.TELEFONOCELULAR,
                    p.TELEFONOFIJO,
                    p.EMAIL,
                    p.IIDSEXO,
                    FECHACONTRACT = ((DateTime)p.FECHACONTRATO).ToShortDateString(),
                    p.IIDMODALIDADCONTRATO,
                    FOTOMOSTRAR = Convert.ToBase64String(p.FOTO.ToArray())
                }
                );
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int guardarDatos(Docente oDocente, string cadenaFoto) 
        {
            PruebaDataContext db = new PruebaDataContext();
            int nregistrosAfectados = 0;
            try
            {
                int iddocente = oDocente.IIDDOCENTE;
                if (iddocente.Equals(0))
                {
                    int nveces = db.Docente.Where(p => p.NOMBRE.Equals(oDocente.NOMBRE)
                    && p.APPATERNO.Equals(oDocente.APPATERNO) && p.APMATERNO.Equals(oDocente.APPATERNO)).Count();

                    if (nveces == 0)
                    {
                        oDocente.IIDTIPOUSUARIO = 'D';
                        oDocente.bTieneUsuario = 0;
                        oDocente.FOTO = Convert.FromBase64String(cadenaFoto);
                        db.Docente.InsertOnSubmit(oDocente);
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
                    int nveces = db.Docente.Where(p => p.NOMBRE.Equals(oDocente.NOMBRE)
                    && p.APPATERNO.Equals(oDocente.APPATERNO) && p.APMATERNO.Equals(oDocente.APPATERNO)
                    && !p.IIDDOCENTE.Equals(oDocente.IIDDOCENTE)).Count();

                    if(nveces == 0) { 
                        Docente obj = db.Docente.Where(p => p.IIDDOCENTE.Equals(iddocente)).First();
                        obj.NOMBRE = oDocente.NOMBRE;
                        obj.APPATERNO= oDocente.APPATERNO;
                        obj.APMATERNO = oDocente.APMATERNO;
                        obj.DIRECCION = oDocente.DIRECCION;
                        obj.TELEFONOCELULAR = oDocente.TELEFONOCELULAR;
                        obj.TELEFONOFIJO = oDocente.TELEFONOFIJO;
                        obj.EMAIL= oDocente.EMAIL;
                        obj.IIDSEXO= oDocente.IIDSEXO;
                        obj.FECHACONTRATO= oDocente.FECHACONTRATO;
                        obj.IIDMODALIDADCONTRATO= oDocente.IIDMODALIDADCONTRATO;
                        obj.FOTO = Convert.FromBase64String(cadenaFoto);
                        db.SubmitChanges();
                        nregistrosAfectados = 1;
                    }
                    else {
                        nregistrosAfectados = -1;
                    }                 
                }
            }
            catch 
            {
            }
                nregistrosAfectados = 0;
            return nregistrosAfectados;
        }


    }
}