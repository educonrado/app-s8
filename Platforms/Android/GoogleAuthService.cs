using Android.App;
using Android.Gms.Auth.Api.SignIn;
using app_s8.Platforms.Android;




namespace app_s8.GoogleAuth
{
    public partial class GoogleAuthService
    {
        public static Activity _activity;
        public static GoogleSignInOptions _gso;
        public static GoogleSignInClient _googleSignInClient;

        // Campo para rastrear el TaskCompletionSource actual
        private TaskCompletionSource<GoogleUserDTO> _currentTaskCompletionSource;

        public GoogleAuthService()
        {
            _activity = Platform.CurrentActivity;

            // Google Auth Option
            _gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                            .RequestIdToken(WebApiKey)
                            .RequestEmail()
                            .RequestId()
                            .RequestProfile()
                            .Build();

            _googleSignInClient = GoogleSignIn.GetClient(_activity, _gso);
            MainActivity.ResultGoogleAuth += MainActivity_ResultGoogleAuth;
        }

        public Task<GoogleUserDTO> AuthenticateAsync()
        {
            // Crear un nuevo TaskCompletionSource para cada autenticación
            _currentTaskCompletionSource = new TaskCompletionSource<GoogleUserDTO>();

            _activity.StartActivityForResult(_googleSignInClient.SignInIntent, 9001);

            // Retornar la Task del TaskCompletionSource actual
            return _currentTaskCompletionSource.Task;
        }

        private void MainActivity_ResultGoogleAuth(object sender, (bool Success, GoogleSignInAccount Account) e)
        {
            // Verificar que hay un TaskCompletionSource pendiente
            if (_currentTaskCompletionSource == null)
                return;

            // Verificar que el TaskCompletionSource no ha sido completado ya
            if (_currentTaskCompletionSource.Task.IsCompleted)
                return;

            if (e.Success)
            {
                try
                {
                    var currentAccount = e.Account;
                    _currentTaskCompletionSource.SetResult(
                        new GoogleUserDTO
                        {
                            Uid = currentAccount.Id,
                            Email = currentAccount.Email,
                            FullName = currentAccount.DisplayName,
                            TokenId = currentAccount.IdToken,
                            UserName = currentAccount.GivenName,
                        });
                }
                catch (Exception ex)
                {
                    _currentTaskCompletionSource.SetException(ex);
                }
            }
            else
            {
                // Manejar el caso de autenticación cancelada/fallida
                _currentTaskCompletionSource.SetException(
                    new OperationCanceledException("Google authentication was cancelled or failed"));
            }

            // Limpiar la referencia
            _currentTaskCompletionSource = null;
        }

        public async Task<GoogleUserDTO> GetCurrentUserAsync()
        {
            try
            {
                var user = await _googleSignInClient.SilentSignInAsync();
                return new GoogleUserDTO
                {
                    Uid = user.Id,
                    Email = user.Email,
                    FullName = user.DisplayName,
                    TokenId = user.IdToken,
                    UserName = user.GivenName,
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            await _googleSignInClient.SignOutAsync();

            // Limpiar cualquier TaskCompletionSource pendiente
            if (_currentTaskCompletionSource != null && !_currentTaskCompletionSource.Task.IsCompleted)
            {
                _currentTaskCompletionSource.SetException(
                    new OperationCanceledException("User logged out"));
                _currentTaskCompletionSource = null;
            }
        }
    }
}