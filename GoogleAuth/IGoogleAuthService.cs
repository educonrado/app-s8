using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_s8.GoogleAuth
{
    public interface IGoogleAuthService
    {
        public Task<GoogleUserDTO> AuthenticateAsync();
        public Task<GoogleUserDTO> GetCurrentUserAsync();
        public Task LogoutAsync();

    }
}
