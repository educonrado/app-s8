using Google.Cloud.Firestore;
using System.Reflection;

namespace app_s8.Services
{
    public static class FirestoreService
    {
 
        public static FirestoreDb FirestoreDb { get; private set; }

        public static DocumentReference ObtenerDocumentReferenceUsuario(string userId)
        {
            return FirestoreDb.Collection(Constantes.FirestoreCollections.Usuarios).Document(userId);
        }

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
                FirestoreDb = FirestoreDb.Create(Constantes.Conexion.FirebaseProjectId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inicializando Firestore: {ex.Message}");
            }

        }

        public static FirestoreDb GetFirestoreDb() => FirestoreDb;
    }
}
