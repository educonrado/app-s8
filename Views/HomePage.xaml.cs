using app_s8.GoogleAuth;
using app_s8.Services;

namespace app_s8.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();

        var user = UserPreferencesService.GetUser();

        if (user != null)
        {
            welcomeLabel.Text = $"Bienvenido, {user.FullName}";
            emailLabel.Text = $"Correo: {user.Email}";
        }
        else
        {
            // Si no hay usuario guardado, regresar a LoginPage
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }

    private async void LogoutBtn_Clicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlert("Cerrar sesión", "¿Estás seguro?", "Sí", "No");

        if (!confirm) return;

        // Limpia preferencias y vuelve al login
        UserPreferencesService.ClearUser();
        await new GoogleAuthService().LogoutAsync();

        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}
