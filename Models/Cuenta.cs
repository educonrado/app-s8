using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace app_s8.Models
{
    [FirestoreData]
    public class Cuenta
    {
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty("nombre_cuenta")]
        public string NombreCuenta { get; set; }
        [FirestoreProperty("monto")]
        public double Monto { get; set; }

        public Cuenta() { }
    }
}
