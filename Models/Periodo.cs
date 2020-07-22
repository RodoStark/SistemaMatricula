using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiPrimeraAplicacionWeb.Models
{
    public class Periodo
    {
        public int IIDperiodo { get; set; }
        public string NOMBRE { set; get; }
        public DateTime FECHAINICIO { set; get; }
        public DateTime FECHAFIN { set; get; }
        public int BHABILITADO{ get; set; }

    }
}