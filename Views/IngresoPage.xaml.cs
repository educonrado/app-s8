using app_s8.Models;
using app_s8.Services;
using Google.Cloud.Firestore;
using System.Diagnostics;
using Microcharts;
using SkiaSharp;

namespace app_s8.Views;

public partial class IngresoPage : ContentPage
{
    private readonly FinanzasService _finanzasService;
    public IngresoPage()
	{
		InitializeComponent();
        _finanzasService = new FinanzasService();
        fechaDatePicker.Date = DateTime.Now;
        cuentaPicker.SelectedIndex = 0;

        CargarGraficoPorFecha();
    }

    public IngresoPage(double total)
    {
        InitializeComponent();
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        try
        {
            if (!ValidarCampos())
                return;

            var ingreso = new Ingreso
            {
                Monto = double.Parse(montoEntry.Text),
                Categoria = categoriaPicker.SelectedItem?.ToString(),
                Fecha = Timestamp.FromDateTime(fechaDatePicker.Date.ToUniversalTime()),
                Descripcion = descripcionEntry.Text,
                Cuenta = cuentaPicker.SelectedItem?.ToString(),
                Nota = notaEditor.Text
            };

            await _finanzasService.AgregarIngresoAsync(ingreso);
            var ingresos = await _finanzasService.ObtenerIngresosUsuarioAsync();
            //CargarGraficoIngresos(ingresos);
            LimpiarCampos();
            await DisplayAlert("Éxito", "Ingreso guardado correctamente", "OK");
            CargarGraficoPorFecha();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al guardar: {ex.Message}", "OK");
        }
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Cancelar",
            "¿Está seguro de que desea cancelar? Se perderán los datos ingresados.",
            "Sí", "No");

        if (confirmar)
        {
            LimpiarCampos();

        }
    }

    private void LimpiarCampos()
    {
        montoEntry.Text = null;
        categoriaPicker.SelectedIndex = -1;
        fechaDatePicker.Date = DateTime.Now;
        descripcionEntry.Text = null;
        cuentaPicker.SelectedIndex = 0;
        notaEditor.Text = null;
        montoEntry.Focus();
    }

    private bool ValidarCampos()
    {

        if (string.IsNullOrWhiteSpace(montoEntry.Text))
        {
            DisplayAlert("Error", "El monto es requerido", "OK");
            return false;
        }

        if (!double.TryParse(montoEntry.Text, out double monto) || monto <= 0)
        {
            DisplayAlert("Error", "Ingrese un monto válido mayor a 0", "OK");
            return false;
        }

        if (categoriaPicker.SelectedItem == null)
        {
            DisplayAlert("Error", "Seleccione una categoría", "OK");
            return false;
        }

        if (string.IsNullOrWhiteSpace(descripcionEntry.Text))
        {
            DisplayAlert("Error", "La descripción es requerida", "OK");
            return false;
        }

        if (cuentaPicker.SelectedItem == null)
        {
            DisplayAlert("Error", "Seleccione una cuenta", "OK");
            return false;
        }

        return true;
    }

    private async void CargarGraficoPorFecha()
    {
        try
        {
            var ingresos = await _finanzasService.ObtenerIngresosUsuarioAsync();

            var entradas = ingresos
                .GroupBy(i => i.Fecha.ToDateTime().ToString("dd MMM"))
                .Select(g => new ChartEntry((float)g.Sum(i => i.Monto))
                {
                    Label = g.Key,
                    ValueLabel = g.Sum(i => i.Monto).ToString("F2"),
                    Color = SKColor.Parse("#3498db")
                })
                .ToList();

            ingresosChart.Chart = new LineChart
            {
                Entries = entradas,
                LineMode = LineMode.Straight,
                LineSize = 4,
                PointMode = PointMode.Circle,
                PointSize = 6,
                BackgroundColor = SKColors.Transparent
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error al cargar gráfico: {ex.Message}");
        }
    }

    private async void CargarGraficoPorCategoria()
    {
        try
        {
            var ingresos = await _finanzasService.ObtenerIngresosUsuarioAsync();

            var entradas = ingresos
                .GroupBy(i => i.Categoria)
                .Select(g => new ChartEntry((float)g.Sum(i => i.Monto))
                {
                    Label = g.Key,
                    ValueLabel = g.Sum(i => i.Monto).ToString("F2"),
                    Color = SKColor.Parse("#2ecc71")
                })
                .ToList();

            ingresosChart.Chart = new DonutChart
            {
                Entries = entradas,
                BackgroundColor = SKColors.Transparent
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error al cargar gráfico: {ex.Message}");
        }
    }

    // Métodos para enlazar con los botones en la UI
    private void OnPag1Clicked(object sender, EventArgs e)
    {
        CargarGraficoPorFecha();
    }

    private void OnPag2Clicked(object sender, EventArgs e)
    {
        CargarGraficoPorCategoria();
    }

    private void OnPag3Clicked(object sender, EventArgs e)
    {
        // Si deseas implementar más adelante otra vista o gráfico.
        DisplayAlert("Info", "Gráfico adicional no implementado todavía", "OK");
    }

}