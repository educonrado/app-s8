using app_s8.Models;

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

}