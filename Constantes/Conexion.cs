using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_s8.Constantes
{
    public static class Conexion
    {
        public const string FirebaseProjectId = "cushquiapp-bd4f8";
        public const string NombreArchivoFirestoreConnect = "adminsdk.json";

        public static string FirebaseCredentialsPath
        {
            get
            {
                return Path.Combine(FileSystem.AppDataDirectory, "adminsdk.json"); 
            }
        }
    }
}
