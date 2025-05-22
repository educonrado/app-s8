using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace app_s8.Models
{
    [FirestoreData]
    public class Usuario
    {
        [FirestoreProperty("saldo_actual")]
        public double SaldoActual {  get; set; }
        [FirestoreProperty("gastos")]

        public List<Gasto> Gastos { get; set; } = new List<Gasto>();
        [FirestoreProperty("cuentas")]
        public List<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
        [FirestoreProperty("ingresos")]
        public List<Ingreso> Ingresos { get; set; } = new List<Ingreso>();
        [FirestoreProperty("resumen_mensual")]
        public Dictionary<string, ResumenMes> ResumenMensual { get; set; } = new Dictionary<string, ResumenMes>();
        [FirestoreProperty("ultima_actualizacion")]
        public Timestamp UltimaActualizacion { get; set; }
        public Usuario() { }
                
    }
}
