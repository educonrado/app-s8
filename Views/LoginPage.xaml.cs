using app_s8.GoogleAuth;

using app_s8.GoogleAuth;
using app_s8.Services;

namespace app_s8.Views;

public partial class LoginPage : ContentPage
{
    private readonly IGoogleAuthService _googleAuthService;

    public LoginPage()
    {
        InitializeComponent();
        _googleAuthService = new GoogleAuthService(); // o usa inyección de dependencias
    }
    private async void loginBtn_Clicked(object sender, EventArgs e)
    {

        var loggedUser = await _googleAuthService.GetCurrentUserAsync();

        if (loggedUser == null)
        {
            loggedUser = await _googleAuthService.AuthenticateAsync();
        }

        if (loggedUser != null)
        {
            UserPreferencesService.SaveUser(loggedUser);
            await Application.Current.MainPage.DisplayAlert("Login Message", "Welcome " + loggedUser.FullName, "Ok");

            // Aquí puedes redirigir al HomePage si deseas
             Application.Current.MainPage = new NavigationPage(new HomePage());
        }


    }

   
}