using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace app_s8.Models
{
    public class ResumenMes
    {
        [FirestoreProperty("ingresos_total")]
        public double IngresosTotal { get; set; }

        [FirestoreProperty("gastos_total")]
        public double GastosTotal { get; set; }

        public ResumenMes() { }
    }
}
