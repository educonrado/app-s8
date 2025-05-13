namespace app_s8.Views;

public partial class DashboardPage : ContentPage
{
	public DashboardPage()
	{
		InitializeComponent();
        CargarDatosPanel();
	}

    private void CargarDatosPanel()
    {
        float balance = 105263.03f;
        float ventas = 2569.87f;
        float gastos = 1047.36f;
        lblBalance.Text = $"${balance}";
        lblVentasMes.Text = $"${ventas}";
        lblGastosMes.Text = $"${gastos}";
    }

    /*private void OnIngresos(object sender, EventArgs e)
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
    }*/
}