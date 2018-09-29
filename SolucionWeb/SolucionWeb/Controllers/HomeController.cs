using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SolucionWeb.Models;

namespace SolucionWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SignIn()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }

        public ActionResult Departamento()
        {
            return View();
        }

        public ActionResult Mall(string departamento)
        {
            ViewBag.departamento = departamento;
            List<string> consulta = (List<string>)Session["Consulta"];
            consulta.Add(departamento);
            Session["Consulta"] = consulta;

            return View();
        }

        [HttpPost]
        public ActionResult SignIn(string dni)
        {
            if (dni.Length != 8)
            {
                ViewBag.mensaje = "Ingrese un DNI válido.";
                return View();
            }
            try
            {
                int n_dni = Int32.Parse(dni);

                using (DatabaseRPEntities2 obj = new DatabaseRPEntities2())
                {
                    var x = from usr in obj.usuario
                            where usr.dni == n_dni
                            select usr.nombre;

                    string variable = x.ToList().ElementAt(0);
                    ViewBag.nombre = variable;

                    Session["Usuario"] = n_dni;
                    List<string> consulta = new List<string>();
                    Session["Consulta"] = consulta;

                    return View("Departamento");
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                ViewBag.mensaje = "DNI no registrado";
                return View();
            }
            catch (FormatException e)
            {
                ViewBag.mensaje = "Ingrese un DNI valido" ;
                return View();
            }
        }

        [HttpPost]
        public ActionResult SignUp(
            string dni, string nombre, string apellidoPaterno, string apellidoMaterno, string email, string telefono, string departamento)
        {

            int n_dni = 0;
            int n_telefono = 0;
            try { n_dni = Int32.Parse(dni); }
            catch { ViewBag.mensaje = "Ingrese un DNI valido"; return View(); }
            try { n_telefono = Int32.Parse(dni); }
            catch { ViewBag.mensaje = "Ingrese un telefono valido"; return View(); }

            try
            {
                n_dni = Int32.Parse(dni);
                n_telefono = Int32.Parse(telefono);
                using (DatabaseRPEntities2 obj = new DatabaseRPEntities2())
                {
                    var x = from usr in obj.usuario
                            select usr.user_id;

                    int new_id = 0;
                    foreach (int elemento in x)
                    {
                        new_id = elemento;
                    }
                    new_id += 1;

                    usuario persona = new usuario();
                    persona.user_id = new_id;
                    persona.dni = n_dni;
                    persona.nombre = nombre;
                    persona.apellidoPaterno = apellidoPaterno;
                    persona.apellidoMaterno = apellidoMaterno;
                    persona.email = email;
                    persona.telefono = n_telefono;
                    persona.departamento = departamento;

                    obj.usuario.Add(persona);
                    obj.SaveChanges();

                   
                    ViewBag.mensaje = "Registro de usuario exitoso.";
                    return View();
                }
            }
            catch (FormatException e)
            {
                ViewBag.mensaje = "Ingrese un DNI valido";
                return View();
            }
            catch (Exception e)
            {
                ViewBag.mensaje = e;
                return View();
            }


        }

    }
}