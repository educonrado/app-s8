using System.Diagnostics;

namespace app_s8.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private void OnLogin(object sender, EventArgs e)
    {
		Debug.WriteLine("Login exitoso!");
		Application.Current.MainPage = new AppShell();
    }
}