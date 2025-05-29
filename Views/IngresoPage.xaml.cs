using app_s8.Models;
using app_s8.Services;
using Google.Cloud.Firestore;
using System.Diagnostics;
using Microcharts;
using SkiaSharp;
using app_s8.ViewModels;
using System.ComponentModel;
using static Android.Provider.ContactsContract.CommonDataKinds;
using static Android.Util.EventLogTags;
using System.Threading.Tasks;

namespace app_s8.Views;

public partial class IngresoPage : ContentPage, INotifyPropertyChanged
{
    private readonly FinanzasService _finanzasService;
    private Ingreso ingresoSeleccionado;
    public IngresoPage()
	{
		InitializeComponent();
        _finanzasService = new FinanzasService();
        fechaDatePicker.Date = DateTime.Now;
        cuentaPicker.SelectedIndex = 0;

        CargarGraficoPorFecha();
        CargarDatosHistoricos();
    }

    private async void CargarDatosHistoricos()
    {
        var ingresos = await _finanzasService.ObtenerIngresosUsuarioAsync();
        var viewModel = new IngresosViewModel(ingresos);
        this.BindingContext = viewModel;
    }

    public IngresoPage(double total)
    {
        InitializeComponent();
        _finanzasService = new FinanzasService();
        CargarValoresPorDefecto(total);
        CargarDatosHistoricos();
    }

    private void CargarValoresPorDefecto(double total)
    {
        montoEntry.Text = total.ToString();
        categoriaPicker.SelectedIndex = 0;
        descripcionEntry.Text = "Venta";
        cuentaPicker.SelectedIndex = 0;
        notaEditor.Text = "Valor cargado automáticamente desde comprobante";
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
            CargarDatosHistoricos();
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
            CambiarModoEdicion(false);
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

        CargarDatosHistoricos();
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

            chartPorFecha.Chart = new LineChart
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

            chartPorCategoria.Chart = new DonutChart
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
        chartPorFecha.IsVisible = true;
        chartPorCategoria.IsVisible = false;
        chartResumen.IsVisible = false;

        // Puedes cargar los datos si aún no están
        CargarGraficoPorFecha();
    }

    private void OnPag2Clicked(object sender, EventArgs e)
    {
        chartPorFecha.IsVisible = false;
        chartPorCategoria.IsVisible = true;
        chartResumen.IsVisible = false;

        CargarGraficoPorCategoria();
    }

    private void OnPag3Clicked(object sender, EventArgs e)
    {
        // Si deseas implementar más adelante otra vista o gráfico.
        DisplayAlert("Info", "Gráfico adicional no implementado todavía", "OK");
    }

    private void listadoIngresos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var ingreso = (Ingreso) e.SelectedItem;
        if (ingreso != null)
        {
            CargarDatosEnFormulario(ingreso);
            ingresoSeleccionado = ingreso;
            CambiarModoEdicion(true);
        }
        else
        {
            LimpiarCampos();
            CambiarModoEdicion(false);
        }
    }

    private void CambiarModoEdicion(bool esEdicion)
    {
        guardarButton.IsVisible = !esEdicion;
        BtnActualizar.IsVisible = esEdicion;
        ingresoSeleccionado = esEdicion ? ingresoSeleccionado : null;
    }

    private void CargarDatosEnFormulario(Ingreso ingreso)
    {
        montoEntry.Text = ingreso.Monto.ToString();
        categoriaPicker.SelectedItem = ingreso.Categoria;
        descripcionEntry.Text = ingreso.Descripcion;
        cuentaPicker.SelectedItem = ingreso.Cuenta;
        notaEditor.Text = ingreso.Nota;
    }

    private async void btnEliminar_Clicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            var ingreso = button?.BindingContext as Ingreso;
            if (ingreso == null) return;
            bool confirmar = await DisplayAlert("Confirmar", "¿Está seguro de eliminar el ingreso?", "Sí", "No");

            if (confirmar)
            {
                await _finanzasService.EliminarIngresoAsync(ingreso.Id);
                LimpiarCampos();
                await DisplayAlert("Éxito", "Ingreso eliminado correctamente", "Aceptar");
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Error al eliminar", "Aceptar");
        }
        
    }

    private async void BtnActualizar_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (ingresoSeleccionado == null || !ValidarCampos()) return;

            ingresoSeleccionado.Monto = double.Parse(montoEntry.Text);
            ingresoSeleccionado.Categoria = categoriaPicker.SelectedItem?.ToString();
            ingresoSeleccionado.Fecha = Timestamp.FromDateTime(fechaDatePicker.Date.ToUniversalTime());
            ingresoSeleccionado.Descripcion = descripcionEntry.Text;
            ingresoSeleccionado.Cuenta = cuentaPicker.SelectedItem?.ToString();
            ingresoSeleccionado.Nota = notaEditor.Text;

            await _finanzasService.ActualizarIngresoAsync(ingresoSeleccionado);
            var ingresos = await _finanzasService.ObtenerIngresosUsuarioAsync();

            CambiarModoEdicion(false);
            LimpiarCampos();
            await DisplayAlert("Ingresos", "Ingreso actualizado correctamente", "Aceptar");

        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Error al actualizar", "Aceptar");
        }
    }
}