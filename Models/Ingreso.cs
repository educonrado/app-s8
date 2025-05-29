using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace app_s8.Models
{
    [FirestoreData]
    public class Ingreso
    {
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty("monto")]
        public double Monto { get; set; }
        [FirestoreProperty("categoria")]
        public string Categoria { get; set; }
        [FirestoreProperty("fecha")]
        public Timestamp Fecha { get; set; }
        [FirestoreProperty("descripcion")]
        public string Descripcion { get; set; }

        [FirestoreProperty("cuenta")]
        public string Cuenta { get; set; }
        [FirestoreProperty("nota")]
        public string Nota { get; set; }
        public string MontoFormateado => Monto.ToString("C2");
        public string FechaFormateada => Fecha.ToDateTime().ToString("MM/yyyy");
    }
}
