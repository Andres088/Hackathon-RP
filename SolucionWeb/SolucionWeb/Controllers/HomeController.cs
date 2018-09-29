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

        public ActionResult Reportes()
        {
            using (DatabaseRPEntities2 obj = new DatabaseRPEntities2())
            {
                var x = from cons in obj.consulta
                        select cons;

                List<consulta> lista_consultas = x.ToList();
                ViewBag.lista_consultas = lista_consultas;

                return View();
            }
            
        }


        public ActionResult DetalleTienda(Tienda tienda)
        {
            Mall mall = (Mall)Session["Mall"];
            var client = new RestClient("https://api.devrealplazaonline.com/v1/coordinates?pi_inm_c_icod="+mall.inm_c_icod+"&ps_loc_c_ccod="+tienda.loc_c_ccod);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Postman-Token", "31bcf8d9-f3b2-4d49-b996-4f991e642505");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("x-api-key", "TDy86NqDhGkZcdbkGeJ45sFL55o69954KjVIaU6h");
            IRestResponse response = client.Execute(request);
            RestSharp.Deserializers.JsonDeserializer deserial = new JsonDeserializer();
            var lista = deserial.Deserialize<List<Ubicacion>>(response);

            ViewBag.ubicaciones = lista;
            ViewBag.tienda = tienda;
            Session["Tienda"] = tienda;

            using (DatabaseRPEntities2 obj = new DatabaseRPEntities2())
            {
                var x = from cons in obj.consulta
                        select cons.query_id;

                int new_id = 0;
                foreach (int elemento in x)
                {
                    new_id = elemento;
                }
                new_id += 1;

                Mall mimall = (Mall)Session["Mall"];
                Rubro mirubro = (Rubro)Session["Rubro"];
                Tienda mitienda = (Tienda)Session["Tienda"];

                consulta consulta = new consulta();
                consulta.query_id = new_id;
                consulta.user_id = (int)Session["Usuario"];
                consulta.departamento = (string)Session["Departamento"];
                consulta.mall = mimall.inm_c_vnomb;
                consulta.rubro = mirubro.rubro_c_vnomb;
                consulta.tienda = mitienda.nomb_com_c_vnomb;
                consulta.fecha = DateTime.Now;
                consulta.hora = DateTime.Now.TimeOfDay;

                obj.consulta.Add(consulta);
                obj.SaveChanges();


                ViewBag.mensaje = "Registro de usuario exitoso.";
                return View();
            }

        }

        public ActionResult Tienda(Rubro rubro)
        {
            
            Mall mall = (Mall)Session["Mall"];
            string codigo_mall = mall.inm_c_icod;
            string codigo_rubro = rubro.rubro_c_yid.ToString();
            List<Rubro> lista_rubros = (List<Rubro>)Session["ListaRubros"];

            RestSharp.Deserializers.JsonDeserializer deserial = new JsonDeserializer();
            var client2 = new RestClient("https://api.devrealplazaonline.com/v1/local?pi_inm_c_icod=" + mall.inm_c_icod + "&pi_rubro_c_yid=" + rubro.rubro_c_yid);
            var request2 = new RestRequest(Method.GET);
            request2.AddHeader("Postman-Token", "19cb3c19-d472-461d-be49-a7777430042c");
            request2.AddHeader("Cache-Control", "no-cache");
            request2.AddHeader("x-api-key", "TDy86NqDhGkZcdbkGeJ45sFL55o69954KjVIaU6h");
            IRestResponse response2 = client2.Execute(request2);
            var lista_tien = deserial.Deserialize<List<Tienda>>(response2);

            ViewBag.lista_tiendas = lista_tien;
            ViewBag.Rubro = rubro;
            Session["Rubro"] = rubro;
            return View();
        }

        public ActionResult Rubro(Mall mall)
        {
            
            RestSharp.Deserializers.JsonDeserializer deserial = new JsonDeserializer();
            var client = new RestClient("https://api.devrealplazaonline.com/v1/items?pi_inm_c_icod="+ mall.inm_c_icod.ToString());
            var request = new RestRequest(Method.GET);
            request.AddHeader("Postman-Token", "52625167-ff0e-4e67-9313-739727ab384e");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("x-api-key", "TDy86NqDhGkZcdbkGeJ45sFL55o69954KjVIaU6h");
            IRestResponse response = client.Execute(request);
            var lista_rub = deserial.Deserialize<List<Rubro>>(response);

            List<Rubro> lista_rubros = new List<Rubro>();
            List<Tienda> lista_tiendas = new List<Tienda>();

            //foreach (Rubro rubro_ in lista_rub)
            //{
            //    var client2 = new RestClient("https://api.devrealplazaonline.com/v1/local?pi_inm_c_icod=" + mall.inm_c_icod + "&pi_rubro_c_yid=" + rubro_.rubro_c_yid);
            //    var request2 = new RestRequest(Method.GET);
            //    request2.AddHeader("Postman-Token", "19cb3c19-d472-461d-be49-a7777430042c");
            //    request2.AddHeader("Cache-Control", "no-cache");
            //    request2.AddHeader("x-api-key", "TDy86NqDhGkZcdbkGeJ45sFL55o69954KjVIaU6h");
            //    IRestResponse response2 = client2.Execute(request2);
            //    var lista_tien = deserial.Deserialize<List<Tienda>>(response2);
            //    if (lista_tien.Count > 0)
            //    {
            //        lista_rubros.Add(rubro_);
            //    }    
            //}

            lista_rubros = lista_rub;
            ViewBag.lista_rubros = lista_rubros;
            ViewBag.mall = mall;
            Session["Mall"] = mall;
            Session["ListaRubros"] = lista_rubros;
            return View();
        }

        public ActionResult Mall(string departamento)
        {
            ViewBag.departamento = departamento;

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
            var client2 = new RestClient("https://api.devrealplazaonline.com/v1/real-estate?ps_depa_c_ccod="+codigo_depa);
            var request2 = new RestRequest(Method.GET);
            request2.AddHeader("Postman-Token", "b08fa9af-9f4d-429f-a2d2-e84557223e29");
            request2.AddHeader("Cache-Control", "no-cache");
            request2.AddHeader("x-api-key", "TDy86NqDhGkZcdbkGeJ45sFL55o69954KjVIaU6h");
            IRestResponse response2 = client2.Execute(request2);
            var lista2 = deserial.Deserialize<List<Mall>>(response2);
            //List<string> lista_malls = new List<string>();
            //foreach (Mall mimall in lista2) lista_malls.Add(mimall.inm_c_vnomb);
            List<Mall> lista_malls = lista2;

            Session["Departamento"] = departamento;
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
                if (n_dni == 06310996) return View("Reportes");
                using (DatabaseRPEntities2 obj = new DatabaseRPEntities2())
                {
                    var x = from usr in obj.usuario
                            where usr.dni == n_dni
                            select usr.nombre;

                    string variable = x.ToList().ElementAt(0);
                    ViewBag.nombre = variable;

                    Session["Usuario"] = n_dni;
 
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

    public class Rubro
    {
        public int rubro_c_yid { get; set; }
        public string rubro_c_vnomb { get; set; }
        public string loc_tipo_c_vnomb { get; set; }
    }

    public class Tienda
    {
        public int inm_c_icod { get; set; }
        public string loc_c_ccod { get; set; }
        public string nomb_com_c_vnomb { get; set; }
    }

    public class Ubicacion
    {
        public int orden { get; set; }
        public int x1 { get; set; }
        public int y1 { get; set; }
        public int x2 { get; set; }
        public int y2 { get; set; }
    }
}