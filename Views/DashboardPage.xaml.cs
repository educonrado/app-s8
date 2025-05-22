using app_s8.Models;
using app_s8.Repository;
using System.Threading.Tasks;

namespace app_s8.Views;

public partial class DashboardPage : ContentPage
{
    private readonly FirestoreRepository _repository;
    public DashboardPage()
	{
		InitializeComponent();
        CargarDatosPanel();
        _repository = new FirestoreRepository();
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

    private async void btnGuardar_Clicked(object sender, EventArgs e)
    {
        var nuevoItem = new CuentasModel
        {
            NombreCuenta = "Efectivo",
            Monto = 1250.26
        };
        await _repository.AddAsync(nuevoItem);
    }
}