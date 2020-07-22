using MiPrimeraAplicacionWeb.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiPrimeraAplicacionWeb.Controllers
{
    [Seguridad]
    public class AlumnoController : Controller
    {
        // GET: Alumno
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult listarSexo()
        {
            PruebaDataContext db = new PruebaDataContext();
            var lista = db.Sexo.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new {IID = p.IIDSEXO, p.NOMBRE });
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult listarAlumnos()
        {
            PruebaDataContext db = new PruebaDataContext();
            var lista=(db.Alumno.Where(p=>p.BHABILITADO.Equals(1))
                .Select(p=>new 
                { 
                    p.IIDALUMNO,
                    p.NOMBRE,
                    p.APPATERNO,
                    p.APMATERNO,
                    p.TELEFONOPADRE
                })).ToList();
            return Json(lista,JsonRequestBehavior.AllowGet);
        }

        public JsonResult filtrarAlumnoPorSexo(int iidsexo) {
            PruebaDataContext db = new PruebaDataContext();
            var lista = db.Alumno.Where(p => p.BHABILITADO.Equals(1)
            && p.IIDSEXO.Equals(iidsexo)).Select(p => new
            {
                p.IIDALUMNO,
                p.NOMBRE,
                p.APPATERNO,
                p.APMATERNO,
                p.TELEFONOPADRE
            }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int eliminar(int id) {
            int nunRegistrosAfectados = 0;

            PruebaDataContext db = new PruebaDataContext();

            try {
                Alumno alumno = db.Alumno.Where(p => p.IIDALUMNO.Equals(id)).First();
                alumno.BHABILITADO = 0;
                db.SubmitChanges();
                nunRegistrosAfectados = 1;
            } catch (Exception ex) {
                nunRegistrosAfectados = 0;
            }

            return nunRegistrosAfectados;
        }

        public JsonResult recuperarInformacion(int id) {            

            PruebaDataContext db = new PruebaDataContext();
            var consulta = db.Alumno.Where(p => p.IIDALUMNO.Equals(id)).
                Select(
                p => new
                {
                    p.IIDALUMNO,
                    p.NOMBRE,
                    p.APPATERNO,
                    p.APMATERNO,
                    FECHANAC = ((DateTime)p.FECHANACIMIENTO).ToShortDateString(),
                    p.IIDSEXO,
                    p.NUMEROHERMANOS,
                    p.TELEFONOMADRE,
                    p.TELEFONOPADRE
                });

            return Json(consulta, JsonRequestBehavior.AllowGet); 
        }

        public int guardarDatos(Alumno alumno) {

            PruebaDataContext db = new PruebaDataContext();
            int numRegistrosAfectados = 0; 
            try 
            {
                int idAlumno = alumno.IIDALUMNO;
                if (idAlumno == 0) 
                {
                    int nveces = db.Alumno.Where(p => p.NOMBRE.Equals(alumno.NOMBRE)
                    &&p.APPATERNO.Equals(alumno.APPATERNO)&&p.APMATERNO.Equals(alumno.APPATERNO)).Count();

                    if (nveces == 0)
                    {
                        alumno.IIDTIPOUSUARIO = 'A';
                        alumno.bTieneUsuario = 0;
                        //nuevo agregar
                        db.Alumno.InsertOnSubmit(alumno);
                        db.SubmitChanges();
                        numRegistrosAfectados = 1;
                    }
                    else 
                    {
                        numRegistrosAfectados = -1;
                    }

                }
                else{
                    int nveces = db.Alumno.Where(p => p.NOMBRE.Equals(alumno.NOMBRE)
                    && p.APPATERNO.Equals(alumno.APPATERNO) && p.APMATERNO.Equals(alumno.APPATERNO)
                    &alumno.IIDALUMNO.Equals(alumno.IIDALUMNO)).Count();

                    if (nveces==0)
                    {
                        Alumno obj = db.Alumno.Where(p => p.IIDALUMNO.Equals(idAlumno)).First();
                        obj.NOMBRE = alumno.NOMBRE;
                        obj.APPATERNO = alumno.APPATERNO;
                        obj.APMATERNO = alumno.APMATERNO;
                        obj.IIDSEXO = alumno.IIDSEXO;
                        obj.TELEFONOPADRE = alumno.TELEFONOPADRE;
                        obj.TELEFONOMADRE = alumno.TELEFONOMADRE;
                        obj.FECHANACIMIENTO = alumno.FECHANACIMIENTO;
                        obj.NUMEROHERMANOS = alumno.NUMEROHERMANOS;
                        db.SubmitChanges();
                        numRegistrosAfectados = 1;
                    }
                    else 
                    {
                        numRegistrosAfectados = -1;
                    }
                }
            }
            catch{
                numRegistrosAfectados = 0;
            }

            return numRegistrosAfectados;
        }

    }
}