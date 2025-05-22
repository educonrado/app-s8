using app_s8.Constantes;
using Google.Api;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace app_s8.Services
{
    public static class FirestoreService
    {
        public static FirestoreDb FirestoreDb { get; private set; }
        //public static FirebaseAuthProvider AuthProvider { get; private set; } // Para Firebase Authentication

        public static void Initialize(string projectId, string apiKey)
        {
            if (FirestoreDb == null)
            {
                FirestoreDb = FirestoreDb.Create(projectId);
            }

            /*if (AuthProvider == null)
            {
                AuthProvider = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
            }*/
        }

        /*public static void Initialize()
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
                FirestoreDb = FirestoreDb.Create(Constantes.Conexion.FirebaseProjectId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inicializando Firestore: {ex.Message}");
            }

        }*/

        //public static FirestoreDb GetFirestoreDb() => FirestoreDb;
    }
}
