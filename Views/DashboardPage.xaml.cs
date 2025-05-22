using app_s8.Models;
using app_s8.Repository;
using app_s8.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace app_s8.Views;

public partial class DashboardPage : ContentPage
{
    private readonly FirestoreRepository repository;
    public DashboardPage()
	{
		InitializeComponent();
        repository = new FirestoreRepository();
        CargarDatosPanel();
        ObtenerDatos(UserService.Instancia.CurrentUserId);
        
    }

    private async void ObtenerDatos(string currentUserId)
    {
        Usuario usuario = await repository.ObtenerUsuarioPorUid(currentUserId);
        Debug.WriteLine(usuario.SaldoActual);
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
        await repository.AddAsync(nuevoItem);
    }
}