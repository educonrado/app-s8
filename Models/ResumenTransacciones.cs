using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_s8.Models
{
    public class ResumenTransacciones
    {
        public string Id { get; set; }
        public string Tipo { get; set; } // "Gasto" o "Ingreso"
        public double Monto { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public Timestamp Fecha { get; set; }
        public string Cuenta { get; set; }

        public string MontoFormateado => Monto.ToString("C2");
        public string FechaFormateada => Fecha.ToDateTime().ToString("MM/yyyy");
        public string TipoColor => Tipo == "Ingreso" ? "Green" : "Red";
    }
}
