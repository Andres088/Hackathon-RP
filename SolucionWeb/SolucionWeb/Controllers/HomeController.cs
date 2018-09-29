using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestSharp;
using RestSharp.Deserializers;
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

        public ActionResult Rubro(string mall)
        {
            ViewBag.mall = mall;
            return View();
        }

        public ActionResult Mall(string departamento)
        {
            ViewBag.departamento = departamento;
            List<string> consulta = (List<string>)Session["Consulta"];
            consulta.Add(departamento);
            Session["Consulta"] = consulta;

            // Consulta API de Departamentos y genera Lista
            RestSharp.Deserializers.JsonDeserializer deserial = new JsonDeserializer();
            var client = new RestClient("https://api.devrealplazaonline.com/v1/departments");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Postman-Token", "50ce8858-368a-4c32-80b1-a819b4197b93");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("x-api-key", "TDy86NqDhGkZcdbkGeJ45sFL55o69954KjVIaU6h");
            IRestResponse response = client.Execute(request);
            var lista = deserial.Deserialize<List<Departamento>>(response);
            Dictionary<string, string> dep_to_code = new Dictionary<string, string>();
            foreach (Departamento departa in lista) dep_to_code.Add(departa.depa_c_vnomb, departa.depa_c_ccod);

            // Consulta API de Busqueda de Mall por Departamento
            string codigo_depa = dep_to_code[departamento];
            var client2 = new RestClient("https://api.devrealplazaonline.com/v1/real-estate?ps_depa_c_ccod=15");
            var request2 = new RestRequest(Method.GET);
            request2.AddHeader("Postman-Token", "b08fa9af-9f4d-429f-a2d2-e84557223e29");
            request2.AddHeader("Cache-Control", "no-cache");
            request2.AddHeader("x-api-key", "TDy86NqDhGkZcdbkGeJ45sFL55o69954KjVIaU6h");
            IRestResponse response2 = client2.Execute(request2);
            var lista2 = deserial.Deserialize<List<Mall>>(response2);
            //List<string> lista_malls = new List<string>();
            //foreach (Mall mimall in lista2) lista_malls.Add(mimall.inm_c_vnomb);
            List<Mall> lista_malls = lista2;

            ViewBag.lista_malls = lista_malls;
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

    public class Departamento
    {
        public string depa_c_ccod { get; set; }
        public string depa_c_vnomb { get; set; }
    }

    public class Mall
    {
        public string inm_c_icod { get; set; }
        public string inm_c_vnomb { get; set; }
        public float inm_c_vlatitud { get; set; }
        public float inm_c_vlongitud { get; set; }
    }
}