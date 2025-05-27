using app_s8.GoogleAuth;
using app_s8.Services;

namespace app_s8.Views;

public partial class LoginPage : ContentPage
{
    private readonly IGoogleAuthService _googleAuthService;
    private GoogleUserDTO _loggedUser;

    public LoginPage()
    {
        InitializeComponent();
        _googleAuthService = new GoogleAuthService();

        _loggedUser = UserPreferencesService.GetUser();

        if (_loggedUser != null)
        {
            loginBtn.Text = "Continuar";
        }
    }

    private async void loginBtn_Clicked(object sender, EventArgs e)
    {
        if (_loggedUser == null)
        {
            // Intentar sesión silenciosa
            _loggedUser = await _googleAuthService.GetCurrentUserAsync();

            // Si no hay sesión previa, autenticar
            if (_loggedUser == null)
            {
                _loggedUser = await _googleAuthService.AuthenticateAsync();
            }

            // Si se logró loguear, cambiar botón a "Continuar"
            if (_loggedUser != null)
            {
                UserPreferencesService.SaveUser(_loggedUser);
                await DisplayAlert("Login", "Autenticado: " + _loggedUser.FullName, "OK");
                loginBtn.Text = "Continuar";
            }
        }
        else
        {
            // Ya estaba autenticado, ir al HomePage
            await Application.Current.MainPage.DisplayAlert("Bienvenido", _loggedUser.FullName, "Entrar");
            Application.Current.MainPage = new NavigationPage(new HomePage());
        }
    }
}
