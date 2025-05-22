using System.Diagnostics;
using app_s8.Services;

namespace app_s8.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private async void OnLogin(object sender, EventArgs e)
    {
		string uid = "uid-001-001";
		if (!string.IsNullOrEmpty(uid))
		{
			UserService.Instancia.SetUserId(uid);
			Debug.WriteLine("Login exitoso!");
			Application.Current.MainPage = new AppShell();
		}
        else
        {
            await DisplayAlert("Error", "No se pudo obtener el UID del usuario.", "OK");
        }
    }
}