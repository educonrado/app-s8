using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_s8.Models
{
    [FirestoreData]
    public class CuentasModel
    {
        [FirestoreDocumentId]
        public string id { get; set; }
        [FirestoreProperty]
        public string NombreCuenta {  get; set; }
        [FirestoreProperty]
        public double Monto { get; set;}

        public CuentasModel()
        {

        }
        public CuentasModel(string nombreCuenta, double monto)
        {
            NombreCuenta = nombreCuenta;
            Monto = monto;
        }
    }

}
