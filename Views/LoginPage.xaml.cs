using System.Diagnostics;
using app_s8.Services;
using app_s8.GoogleAuth;

namespace app_s8.Views;

public partial class LoginPage : ContentPage
{
    private readonly UserService userService;
    public LoginPage()
    {
        InitializeComponent();
        userService = UserService.Instancia;
        NavigationPage.SetHasBackButton(this, false);
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    private async void OnLogin(object sender, EventArgs e)
    {
        var googleAuthService = new GoogleAuthService();
        var user = await googleAuthService.AuthenticateAsync();

        if (user != null)
        {
            UserPreferencesService.SaveUser(user); // tarea 2
            userService.SetUserId(user.Uid);       // tarea 1

            Debug.WriteLine("Login exitoso!");

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

}