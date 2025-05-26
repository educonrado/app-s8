using System.Diagnostics;
using app_s8.Services;

namespace app_s8.Views;

public partial class LoginPage : ContentPage
{
	private readonly UserService userService = UserService.Instancia;
	public LoginPage()
	{
		InitializeComponent();
	}

    private async void OnLogin(object sender, EventArgs e)
    {
		try
		{
            string uid = "uid-001-001";
            userService.SetUserId(uid);
			Debug.WriteLine("Login exitoso!");
			Application.Current.MainPage = new AppShell();
        }
        catch (Exception ex)
		{
            Debug.WriteLine($"Error en login: {ex.Message}");
            await DisplayAlert("Error", "No se pudo obtener el UID del usuario.", "OK");
            
		}

    }
}