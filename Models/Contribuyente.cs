using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F29Downloader.Models
{
    public class Contribuyente
    {
        public string Rut { get; set; } = "";

        public string Clave { get; set; } = "";

        public string Periodo { get; set; } = "";
        
        public string Estado { get; set; } = "Pendiente";

        public string Mensaje { get; set; } = "";
        public string FechaProceso { get; set; } = "";

        public string RutaPdf { get; set; } = "";

        public int Intentos { get; set; } = 0;
    }
}
