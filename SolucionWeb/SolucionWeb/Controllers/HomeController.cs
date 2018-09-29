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
            using (DatabaseRPEntities2 obj = new DatabaseRPEntities2())
            {
                var x = from usr in obj.usuario
                        where usr.dni == 58986728
                        select usr.nombre;


                    string variable = x.ToList().ElementAt(0);
                    ViewBag.nombre = variable;
    
                    ViewBag.nombre = "No se encontro";

                return View();
            }
                
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            ViewBag.Respuesta = "";
            return View();
        }

        [HttpPost]
        public ActionResult VerificarUsuario(string reportName)
        {
            ViewBag.Message = "Your application description page.";
            ViewBag.Respuesta = "muere " + reportName;


            return View("About");
        }


        [HttpPost]
        public ActionResult About(string reportName)
        {
            ViewBag.Message = "Your application description page.";
            ViewBag.Respuesta = "hola " + reportName;

            return View();
        }

        public ActionResult SignUp()
        {


            return View();
        }

    }
}