namespace app_s8.Views;

public partial class DashboardPage : ContentPage
{
	public DashboardPage()
	{
		InitializeComponent();
	}

    private void OnIngresos(object sender, EventArgs e)
    {
        
        Navigation.PushAsync(new IngresoPage());
    }

    private void OnGastos(object sender, EventArgs e)
    {
        
        Navigation.PushAsync(new GastoPage());
    }

    private void OnConfiguraciones(object sender, EventArgs e)
    {
        
        Navigation.PushAsync(new ConfiguracionPage());
    }
}