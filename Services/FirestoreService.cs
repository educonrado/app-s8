using app_s8.Constantes;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace app_s8.Services
{
    public class FirestoreService
    {
        private static FirestoreDb firestoreDb;

        public static void Initialize()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "app_s8.Resources.adminsdk.json";

                string tempPath = Path.Combine(FileSystem.CacheDirectory, Constantes.Conexion.NombreArchivoFirestoreConnect);

                using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream == null)
                        throw new Exception("No se encontró el archivo de credenciales embebido");

                    using (var fileStream = File.Create(tempPath))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }

                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", tempPath);
                firestoreDb = FirestoreDb.Create(Constantes.Conexion.FirebaseProjectId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inicializando Firestore: {ex.Message}");
            }

        }

        public static FirestoreDb GetFirestoreDb() => firestoreDb;
    }
}
