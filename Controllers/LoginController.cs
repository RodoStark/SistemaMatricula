using Microsoft.Ajax.Utilities;
using Microsoft.SqlServer.Server;
using MiPrimeraAplicacionWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MiPrimeraAplicacionWeb.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public int ValidarUsuario(string usuario, string contra) 
        {
            int respuesta = 0;
            try
            {
                using (PruebaDataContext db = new PruebaDataContext())
                {
                    SHA256Managed sha = new SHA256Managed();
                    byte[] dataNoCifrada = Encoding.Default.GetBytes(contra);
                    byte[] dataCifrada = sha.ComputeHash(dataNoCifrada);
                    string contraCifrada = BitConverter.ToString(dataCifrada).Replace("-", "");
                    
                    respuesta = db.Usuario.Where(p => p.NOMBREUSUARIO.Equals(usuario)&&p.CONTRA.Equals(contraCifrada)).Count();
                    if (respuesta == 1)
                    {
                        int idUsuario = db.Usuario.Where(p => p.NOMBREUSUARIO == usuario && p.CONTRA == contraCifrada).First().IIDUSUARIO;
                        Session["idUsuario"] = idUsuario;
                        //Session["tipoUsuario"] = db.Usuario.Where(p => p.NOMBREUSUARIO == usuario && p.CONTRA == contraCifrada).First().TIPOUSUARIO;
                        var roles = from usu in db.Usuario
                                    join rol in db.Rol
                                    on usu.IIDROL equals rol.IIDROL
                                    join rolPagina in db.RolPagina
                                    on usu.IIDROL equals rolPagina.IIDROL
                                    join pagina in db.Pagina
                                    on rolPagina.IIDPAGINA equals pagina.IIDPAGINA
                                    where usu.BHABILITADO == 1 && rolPagina.BHABILITADO == 1
                                    && usu.IIDUSUARIO == idUsuario
                                    select new
                                    {
                                        accions = pagina.ACCION,
                                        controladores = pagina.CONTROLADOR,
                                        mensaje = pagina.MENSAJE
                                    };

                        Variable.Acciones = new List<string>();
                        Variable.Controladores = new List<string>();
                        Variable.Mensaje = new List<string>();

                        foreach (var item in roles) 
                        {
                            Variable.Acciones.Add(item.accions);
                            Variable.Controladores.Add(item.controladores);
                            Variable.Mensaje.Add(item.mensaje);


                        }
                    }
                }
                return respuesta;
                
            }
            catch 
            {
                respuesta = 0;
            }
            return respuesta;
        }

        public ActionResult Cerrar() 
        {
            return RedirectToAction("Index");
        }
    }
}