using app_s8.Constantes;
using app_s8.Models;
using app_s8.Services;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_s8.Repository
{
    public class FirestoreRepository
    {
        private readonly FirestoreDb db;

        public FirestoreRepository()
        {
            db = FirestoreService.GetFirestoreDb();
        }

        public async Task<string> AddAsync(CuentasModel cuenta)
        {
            var coleccion = db.Collection(FirestoreCollections.Usuarios);
            var documento = await coleccion.AddAsync(cuenta);
            return documento.Id;
        }

        public async Task<Usuario> ObtenerUsuarioPorUid(string uid)
        {
            DocumentReference docRef = db.Collection(FirestoreCollections.Usuarios).Document(uid);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                Usuario usuario = snapshot.ConvertTo<Usuario>();
                return usuario;
            }
            else
            {
                Usuario nuevoUsuario = new Usuario
                {
                    SaldoActual = 0.0,
                    Gastos = new List<Gasto>(),
                    Ingresos = new List<Ingreso>(),
                    Cuentas = new List<Cuenta>
                    {
                        new Cuenta{NombreCuenta = "Efectivo", Monto = 0.0 }
                    },
                    ResumenMensual = new Dictionary<string, ResumenMes>()

                };
                await docRef.SetAsync(nuevoUsuario);
                return nuevoUsuario;
            }

            /*var coleccion = db.Collection(FirestoreCollections.Usuarios);
            var documento = await coleccion.GetSnapshotAsync().;*/
        }
    }
}
