using MiPrimeraAplicacionWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace MiPrimeraAplicacionWeb.Controllers
{
	public class UsuarioController : Controller
	{
		// GET: Usuario
		public ActionResult Index()
		{
			return View();
		}

		public JsonResult ListarRol()
		{
			using (PruebaDataContext db = new PruebaDataContext())
			{
				var lista = db.Rol.Where(p => p.BHABILITADO == 1)
				.Select(x => new
				{
					IID = x.IIDROL,
					x.NOMBRE
				}
				).ToList();
				return Json(lista, JsonRequestBehavior.AllowGet);
			}
		}

		public JsonResult ListarPersonas()
		{
			List<PersonaCLS> listaPersona = new List<PersonaCLS>();
			//ListaAlumnos
			using (var db = new PruebaDataContext())
			{
				List<PersonaCLS> listaAlumno = (from item in db.Alumno
												where item.bTieneUsuario == 0
												select new PersonaCLS
												{
													IID = item.IIDALUMNO,
													NOMBRE = item.NOMBRE + " " + item.APPATERNO + " " + item.APMATERNO + " (A)"
												}).ToList();
				listaPersona.AddRange(listaAlumno);
				List<PersonaCLS> listaDocente = (from item in db.Docente
												 where item.bTieneUsuario == 0
												 select new PersonaCLS
												 {
													 IID = item.IIDDOCENTE,
													 NOMBRE = item.NOMBRE + " " + item.APPATERNO + " " + item.APMATERNO + " (D)"
												 }).ToList(); ;
				listaPersona.AddRange(listaDocente);
				listaPersona = listaPersona.OrderBy(p => p.NOMBRE).ToList();

				return Json(listaPersona, JsonRequestBehavior.AllowGet);
			}
		}

		public int GuardarDatos(Usuario usuario, string nombreCompleto)
		{
			int respuesta = 0;
			try
			{				
				int idUsuario = usuario.IIDUSUARIO;
				using (PruebaDataContext db = new PruebaDataContext())
				{
					using (var transaction = new TransactionScope())
					{
						if (idUsuario == 0)
						{
							string clave = usuario.CONTRA;
							SHA256Managed sha = new SHA256Managed();
							byte[] dataNoCifrada = Encoding.Default.GetBytes(clave);
							byte[] dataCifrada = sha.ComputeHash(dataNoCifrada);
							//contrasena
							usuario.CONTRA = BitConverter.ToString(dataCifrada).Replace("-", "");
							char tipo = char.Parse(nombreCompleto.Substring(nombreCompleto.Length - 2, 1));
							usuario.TIPOUSUARIO = tipo;
							db.Usuario.InsertOnSubmit(usuario);

							if (tipo.Equals('A'))
							{
								Alumno alumno = db.Alumno.Where(p => p.IIDALUMNO == usuario.IID).First();
								alumno.bTieneUsuario = 1;
							}
							else
							{
								Docente docente = db.Docente.Where(p => p.IIDDOCENTE == usuario.IID).First();
								docente.bTieneUsuario = 1;
							}
							db.SubmitChanges();
							transaction.Complete();
							respuesta = 1;
						}
						else 
						{
							Usuario oUsuarioCLS = db.Usuario.Where(p => p.IIDUSUARIO == idUsuario).First();
							oUsuarioCLS.IIDROL = oUsuarioCLS.IIDROL;
							oUsuarioCLS.NOMBREUSUARIO = oUsuarioCLS.NOMBREUSUARIO;
							db.SubmitChanges();
							transaction.Complete();
							respuesta = 1;
						}
					}
				}
			}
			catch (Exception ex)
			{
				respuesta = 0;

			}		
			return respuesta;
		}

		public JsonResult ListarUsuarios() 
		{
			List<UsuarioCLS> listaUsuario = new List<UsuarioCLS>();
			using (PruebaDataContext db = new PruebaDataContext()) 
			{
				List<UsuarioCLS> listaAlumno = (from usuario in db.Usuario
												join alumno in db.Alumno
												on usuario.IID equals alumno.IIDALUMNO
												join rol in db.Rol
												on usuario.IIDROL equals rol.IIDROL
												where usuario.BHABILITADO == 1 && usuario.TIPOUSUARIO == 'A'
												select new UsuarioCLS
												{
													IdUsuario = usuario.IIDUSUARIO,
													NombrePersona = alumno.NOMBRE + " " + alumno.APPATERNO + " "+alumno.APMATERNO,
													NombreUsuario = usuario.NOMBREUSUARIO,
													NombreRol = rol.NOMBRE,
													NombreTipoEmpleado = "Alumno"


												}).ToList();
				listaUsuario.AddRange(listaAlumno);

				List<UsuarioCLS> listaDocente = (from usuario in db.Usuario
												join docente in db.Docente
												on usuario.IID equals docente.IIDDOCENTE
												join rol in db.Rol
												on usuario.IIDROL equals rol.IIDROL
												where usuario.BHABILITADO == 1 && usuario.TIPOUSUARIO == 'A'
												select new UsuarioCLS
												{
													IdUsuario = usuario.IIDUSUARIO,
													NombrePersona = docente.NOMBRE + " " + docente.APPATERNO + " "+docente.APMATERNO,
													NombreUsuario = usuario.NOMBREUSUARIO,
													NombreRol = rol.NOMBRE,
													NombreTipoEmpleado = "Docente"


												}).ToList();
				listaUsuario.AddRange(listaDocente);
				listaUsuario = listaUsuario.OrderBy(p => p.IdUsuario).ToList();
			}
			return Json(listaUsuario, JsonRequestBehavior.AllowGet);
		}

		public JsonResult RecuperarInformacion(int idUsuario) 
		{
			using (PruebaDataContext db = new PruebaDataContext()) 
			{
				var oUsuario = db.Usuario.Where(p => p.IIDUSUARIO == idUsuario).
								   Select(
									p => new
									{
										p.IIDUSUARIO,
										p.NOMBREUSUARIO,
										p.IIDROL
									}).First();
				return Json(oUsuario, JsonRequestBehavior.AllowGet);
			}			
		}
	}
}