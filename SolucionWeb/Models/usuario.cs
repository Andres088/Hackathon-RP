//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SolucionWeb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class usuario
    {
        public int user_id { get; set; }
        public Nullable<int> dni { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string email { get; set; }
        public Nullable<int> telefono { get; set; }
        public string departamento { get; set; }
    }
}
