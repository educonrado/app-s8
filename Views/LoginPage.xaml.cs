using System.Diagnostics;
using app_s8.Services;
using app_s8.GoogleAuth;

namespace app_s8.Views;

public partial class LoginPage : ContentPage
{
    private readonly UserService userService;
    private readonly GoogleAuthService googleAuth;
    public LoginPage()
    {
        InitializeComponent();
        userService = UserService.Instancia;
        NavigationPage.SetHasBackButton(this, false);
        googleAuth = new GoogleAuthService();
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    private async void OnLogin(object sender, EventArgs e)
    {
        var user = await googleAuth.AuthenticateAsync();

        if (user != null)
        {
            userService.SetUserId(user.Uid);

            Debug.WriteLine("Login exitoso!"+ user.FullName);

            var loadingPage = new ContentPage
            {
                Content = new StackLayout
                {
                    Children =
                {
                    new ActivityIndicator { IsRunning = true, Color = Colors.BlueViolet },
                    new Label { Text = "Cargando...", HorizontalOptions = LayoutOptions.Center }
                },
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                }
            };

            Application.Current.MainPage = loadingPage;

            await Task.Delay(100);
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("Error", "Autenticación fallida", "OK");
        }
    }
    private async void OnTermsTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Términos y Condiciones",
            "Aquí iría toda la información sobre los términos, condiciones y la política de privacidad que el usuario debería aceptar.",
            "Cerrar");
    }


}