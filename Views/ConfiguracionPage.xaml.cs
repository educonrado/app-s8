using System.Diagnostics;
using app_s8.Models;
using app_s8.Services;
using Google.Cloud.Firestore;

namespace app_s8.Views;

public partial class ConfiguracionPage : ContentPage
{

    private readonly FinanzasService _finanzasService;
    public ConfiguracionPage()
	{
		InitializeComponent();
        _finanzasService = new FinanzasService();
    }

    private void btnCerrarSesion_Clicked(object sender, EventArgs e)
    {
        UserService.Instancia.ClearUserId();

        Application.Current.MainPage = new NavigationPage(new Views.LoginPage());
    }

    private async void btnIngreso_Clicked(object sender, EventArgs e)
    {
        Ingreso ingreso = new Ingreso
        {
            Categoria = "Venta",
            Cuenta = "Efectivo",
            Monto = 202.30,
            Descripcion = "Ejemplo de ingreso",
            Nota = "Cliente Caro",
            Fecha = Timestamp.GetCurrentTimestamp()
        };
        _ = _finanzasService.AgregarIngresoAsync(ingreso);
    }

    private async void btnGasto_Clicked(object sender, EventArgs e)
    {
        Gasto gasto = new()
        {
            Categoria = "Insumos",
            Cuenta = "Efectivo",
            Descripcion = "Ejemplo de gasto",
            Monto = 22.30,
            Fecha = Timestamp.GetCurrentTimestamp(),
            Nota = "Compra de insumos"
        };
        _ = _finanzasService.AgregarGastoAsync(gasto);
    }

    private async void btnGuardar_Clicked(object sender, EventArgs e)
    {
        try
        {
            var nuevaCuenta = new Cuenta
            {
                NombreCuenta = "Cuenta de Prueba",
                Monto = new Random().NextDouble() * 100
            };

            await _finanzasService.AgregarCuentaAsync(nuevaCuenta);

            await DisplayAlert("Éxito", "Cuenta agregada correctamente", "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error agregando cuenta: {ex.Message}");
            await DisplayAlert("Error", "Error agregando cuenta", "OK");
        }
    }
}