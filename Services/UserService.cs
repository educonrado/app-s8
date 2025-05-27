using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_s8.Services
{
    public class UserService
    {
        private static UserService instanciaLocal;
        public static UserService Instancia
        {
            get
            {
                if (instanciaLocal == null)
                {
                    instanciaLocal = new UserService();
                }
                return instanciaLocal;
            }
        }

        public string CurrentUserId { get; private set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(CurrentUserId);

        public void SetUserId(string uid)
        {
            CurrentUserId = uid;
            Debug.WriteLine($"UserService: UID establecido a {CurrentUserId}");
        }

        public void ClearUserId()
        {
            CurrentUserId = null;

        }
    }
}
