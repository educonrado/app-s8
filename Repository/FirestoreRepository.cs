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
            var coleccion = db.Collection(FirestoreCollections.Emprendimientos);
            var documento = await coleccion.AddAsync(cuenta);
            return documento.Id;
        }
    }
}
