using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;
using MiPrimeraAplicacionWeb.Models;

namespace MiPrimeraAplicacionWeb.Controllers
{
    public class MatriculaController : Controller
    {
        // GET: Matricula
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult Listar()
        {
            PruebaDataContext db = new PruebaDataContext();
            var lista = (from Matricula in db.Matricula
                         join periodo in db.Periodo
                         on Matricula.IIDPERIODO equals periodo.IIDPERIODO
                         join grad in db.Grado
                         on Matricula.IIDGRADO equals grad.IIDGRADO
                         join seccion in db.Seccion
                         on Matricula.IIDSECCION equals seccion.IIDSECCION
                         join alumno in db.Alumno
                         on Matricula.IIDALUMNO equals alumno.IIDALUMNO                         
                         where Matricula.BHABILITADO==1
                         select new
                         {
                             IID = Matricula.IIDMATRICULA,
                             NOMBREPERIODO = periodo.NOMBRE,
                             NOMBREGRADO = grad.NOMBRE,
                             NOMBRESECCION = seccion.NOMBRE,                                                          
                             NOMBREALUMNO = alumno.NOMBRE + " " + alumno.APPATERNO + " " + alumno.APMATERNO
                         }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerMatricula(int id)
        {            
            using (PruebaDataContext db = new PruebaDataContext()) 
            {
                Matricula matricula = db.Matricula.Where(p => p.IIDMATRICULA.Equals(id)).First();
                int idGrado = (int) matricula.IIDGRADO;
                int idSeccion =(int) matricula.IIDSECCION;

                int iid = db.GradoSeccion.Where(p => p.IIDGRADO == idGrado && p.IIDSECCION == idSeccion).First().IID;

                var oMatricula = db.Matricula.Where(p => p.IIDMATRICULA.Equals(id)).Select(
                    p => new
                    {
                        IIDMATRICULA = (int)p.IIDMATRICULA,
                        IIDPERIODO = (int)p.IIDPERIODO,
                        IIDSECCION = iid,
                        IIDALUMNO = (int)p.IIDALUMNO
                    }).First();
                return Json(oMatricula, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ListarCursos(int id)
        {
            using (PruebaDataContext db = new PruebaDataContext()) {
                int iidGrado = (int) db.Matricula.Where(p => p.IIDMATRICULA == id).First().IIDGRADO;
                List<int?> lista= db.PeriodoGradoCurso.Where(p => p.IIDGRADO == iidGrado).Select(p=>p.IIDCURSO).ToList();
                var listaCurso = (from detalle in db.DetalleMatricula
                                  join curso in db.Curso
                                  on detalle.IIDCURSO equals curso.IIDCURSO
                                  where detalle.IIDMATRICULA.Equals(id)
                                  && lista.Contains(detalle.IIDCURSO)
                                  //where detalle.bhabilitado.Equals(1)
                                  select new 
                                  { 
                                      detalle.IIDMATRICULA,
                                      curso.IIDCURSO,
                                      curso.NOMBRE,
                                      detalle.bhabilitado
                                  }).ToList();
                return Json(listaCurso, JsonRequestBehavior.AllowGet);
            }
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
            return Json(list, JsonRequestBehavior.AllowGet);
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

        public JsonResult ListarAlumnos()
        {
            PruebaDataContext db = new PruebaDataContext();
            var list = db.Alumno.Where(p => p.BHABILITADO.Equals(1)).
                Select(p => new
                {
                    IID = p.IIDALUMNO,
                    NOMBRE = p.NOMBRE + " " + p.APPATERNO + " " + p.APMATERNO
                });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public int GuardarDatos(Matricula oMatricula, int IIDGRADOSECCION, string valorAEnviar, string valorADeshabilitar) 
        {

            int nregistrosAfectados = 0;
            //nregistrosAfectados = -1;
            PruebaDataContext db = new PruebaDataContext();
            int iidMatricula = oMatricula.IIDMATRICULA;
            GradoSeccion oGradoseccion = db.GradoSeccion.Where(p => p.IID.Equals(IIDGRADOSECCION)).First();
            int iidgrado = (int) oGradoseccion.IIDGRADO;
            int iidseccion = (int) oGradoseccion.IIDSECCION;
            oMatricula.IIDGRADO = iidgrado;
            oMatricula.IIDSECCION = iidseccion;
            oMatricula.FECHA = DateTime.Now;

            try
            {
                using (var transaccion = new TransactionScope()) 
                {
                    if (oMatricula.IIDMATRICULA.Equals(0))
                    {
                        int cantidad = db.Matricula.Where(p => p.IIDALUMNO == oMatricula.IIDALUMNO
                         && p.IIDPERIODO == oMatricula.IIDPERIODO).Count();
                        if (cantidad > 1) 
                        {
                            return -1;
                        }
                        int nveces = db.Matricula.Where(
                            p => p.IIDALUMNO.Equals(oMatricula.IIDALUMNO)
                            && p.IIDPERIODO.Equals(oMatricula.IIDPERIODO)
                            && p.IIDGRADO.Equals(oMatricula.IIDGRADO)).Count();

                        if (nveces == 0)
                        {
                            db.Matricula.InsertOnSubmit(oMatricula);
                            db.SubmitChanges();
                            int idMatriculaGenerada = oMatricula.IIDMATRICULA;
                            /*var lista = db.PeriodoGradoCurso.Where(p => p.IIDPERIODO.Equals(oMatricula.IIDPERIODO)
                                && p.IIDGRADO.Equals(iidgrado) && p.BHABILITADO.Equals(1)).Select(p => p.IIDCURSO);*/

                            if (valorAEnviar != "" && valorAEnviar != null)
                            {
                                string[] cursos = valorAEnviar.Split('$');
                                foreach (var curso in cursos)
                                {
                                    DetalleMatricula dm = new DetalleMatricula();
                                    dm.IIDMATRICULA = idMatriculaGenerada;
                                    dm.IIDCURSO = int.Parse(curso);
                                    dm.NOTA1 = 0;
                                    dm.NOTA2 = 0;
                                    dm.NOTA3 = 0;
                                    dm.NOTA4 = 0;
                                    dm.PROMEDIO = 0;
                                    dm.bhabilitado = 1;
                                    db.DetalleMatricula.InsertOnSubmit(dm);
                                }
                            }

                            if (valorADeshabilitar != "" && valorADeshabilitar != null)
                            {
                                string[] cursos = valorAEnviar.Split('$');
                                foreach (var curso in cursos)
                                {
                                    DetalleMatricula dm = new DetalleMatricula();
                                    dm.IIDMATRICULA = idMatriculaGenerada;
                                    dm.IIDCURSO = int.Parse(curso);
                                    dm.NOTA1 = 0;
                                    dm.NOTA2 = 0;
                                    dm.NOTA3 = 0;
                                    dm.NOTA4 = 0;
                                    dm.PROMEDIO = 0;
                                    dm.bhabilitado = 0;
                                    db.DetalleMatricula.InsertOnSubmit(dm);
                                }
                            }


                            db.SubmitChanges();
                            transaccion.Complete();
                            nregistrosAfectados = 1;
                        }
                        else 
                        {
                            nregistrosAfectados = -1;
                        }
                    }
                    else 
                    {
                        int cantidad = db.Matricula.Where(p => p.IIDALUMNO == oMatricula.IIDALUMNO
                        && p.IIDPERIODO == oMatricula.IIDPERIODO&&p.IIDMATRICULA!=oMatricula.IIDMATRICULA).Count();
                        if (cantidad > 1)
                        {
                            return -1;
                        }
                        int nveces = db.Matricula.Where(p => p.IIDALUMNO.Equals(oMatricula.IIDALUMNO)
                        && p.IIDPERIODO.Equals(oMatricula.IIDPERIODO)
                        && p.IIDGRADO.Equals(oMatricula.IIDGRADO)
                        && !p.IIDMATRICULA.Equals(oMatricula.IIDMATRICULA)).Count();

                        if (nveces == 0)
                        {
                            Matricula oMatriculaObjeto = db.Matricula.Where(p => p.IIDMATRICULA == oMatricula.IIDMATRICULA).First();
                            oMatriculaObjeto.IIDPERIODO = oMatricula.IIDPERIODO;
                            oMatriculaObjeto.IIDGRADO = iidgrado;
                            oMatriculaObjeto.IIDSECCION = iidseccion;
                            oMatriculaObjeto.IIDALUMNO = oMatriculaObjeto.IIDALUMNO;

                            var lista = db.DetalleMatricula.Where(p => p.IIDMATRICULA == oMatricula.IIDMATRICULA);
                            foreach (DetalleMatricula odetalle in lista)
                            {
                                odetalle.bhabilitado = 0;
                            }
                            //4$3$5 [4,3,5]
                            string[] valores = valorAEnviar.Split('$');

                            if (valorAEnviar != "") 
                            {
                                int nVeces = 0;
                                for (int i = 0; i < valores.Length; i++)
                                {
                                    nVeces = db.DetalleMatricula.Where(p => p.IIDMATRICULA == oMatricula.IIDMATRICULA
                                    && p.IIDCURSO == int.Parse(valores[i])).Count();


                                    if (nVeces == 1)
                                    {
                                        DetalleMatricula odet = db.DetalleMatricula.Where(p => p.IIDMATRICULA == oMatricula.IIDMATRICULA
                                        && p.IIDCURSO == int.Parse(valores[i])).First();
                                        odet.bhabilitado = 1;
                                    }
                                    else
                                    {                                        
                                        DetalleMatricula dm = new DetalleMatricula();
                                        dm.IIDMATRICULA = oMatricula.IIDMATRICULA;
                                        dm.IIDCURSO = int.Parse(valores[i]);
                                        dm.NOTA1 = 0;
                                        dm.NOTA2 = 0;
                                        dm.NOTA3 = 0;
                                        dm.NOTA4 = 0;
                                        dm.PROMEDIO = 0;
                                        dm.bhabilitado = 1;
                                        db.DetalleMatricula.InsertOnSubmit(dm);
                                    }
                                }
                            }
                            db.SubmitChanges();
                            transaccion.Complete();
                            nregistrosAfectados = 1;
                        }
                    }
                }
            }
            catch 
            {
                nregistrosAfectados = 0;
            }
            return nregistrosAfectados;
        }

        public int Eliminar(int iidMatricula) 
        {
            int respuesta = 0;
            PruebaDataContext db = new PruebaDataContext();
            try
            {
                using (var transaccion = new TransactionScope()) 
                {
                    Matricula oMatricula = db.Matricula.Where(p => p.IIDMATRICULA == iidMatricula).First();
                    oMatricula.BHABILITADO = 0;
                    var listaDetalleMatricula = db.DetalleMatricula.Where(p => p.IIDMATRICULA == iidMatricula);

                    foreach (DetalleMatricula oDetalleMatricula in listaDetalleMatricula) 
                    {
                        oDetalleMatricula.bhabilitado = 0;
                    }
                    db.SubmitChanges();
                    transaccion.Complete();
                    respuesta = 1;
                }
            }
            catch (Exception ex) 
            {
                respuesta = 0;
            }
            return respuesta;
        }

        public JsonResult ListarCursosPorPeriodoYGrado(int iidPeriodo, int iidGradoSeccion) 
        {
            
            using (PruebaDataContext db = new PruebaDataContext()) 
            {
                int iidGrado = (int)db.GradoSeccion.Where(p => p.IID == iidGradoSeccion).First().IIDGRADO;   
                var lista = (from periodoGradoCurso in db.PeriodoGradoCurso
                             join curso in db.Curso
                             on periodoGradoCurso.IIDCURSO equals curso.IIDCURSO
                             where periodoGradoCurso.BHABILITADO == 1
                             && periodoGradoCurso.IIDPERIODO == iidPeriodo
                             && periodoGradoCurso.IIDGRADO == iidGrado
                             select new Curso
                             {
                                 IIDCURSO = curso.IIDCURSO,
                                 NOMBRE = curso.NOMBRE
                             }).ToList();

                return Json(lista, JsonRequestBehavior.AllowGet);
            }
            
        }

    }


}