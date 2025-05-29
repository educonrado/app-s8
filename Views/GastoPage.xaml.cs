using app_s8.Models;
using app_s8.Services;
using app_s8.ViewModels;
using Google.Cloud.Firestore;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace app_s8.Views;

public partial class GastoPage : ContentPage
{
    private double total;
    private Gasto gastoSeleccionado;
    private readonly FinanzasService _finanzasService;

    public GastoPage()
    {
        InitializeComponent();
        FirestoreService.Initialize();
        _finanzasService = new FinanzasService();
        CargarGastosHistoricos();
        
    }

    private async void CargarGastosHistoricos()
    {
        var gastos = await _finanzasService.ObtenerGastosUsuarioAsync();
        var viewModel = new GastosViewModel(gastos);
        this.BindingContext = viewModel;
    }

    public GastoPage(double total)
    {
        InitializeComponent();
        _finanzasService = new FinanzasService();
        CargarValoresPorDefecto(total);
        CargarGastosHistoricos();
    }

    private void CargarValoresPorDefecto(double total)
    {
        EntryMonto.Text = total.ToString();
        categoriaPicker.SelectedIndex = 0;
        EntryDescripcion.Text = "Compra";
        cuentaPicker.SelectedIndex = 0;
        EditorNota.Text = "Valor cargado automáticamente desde comprobante";
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        /**
         * try
        {
            

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
         * */
        try
        {
            if (!ValidarCampos())
                return;

            var gasto = new Gasto
            {
                Monto = double.Parse(EntryMonto.Text),
                Categoria = categoriaPicker.SelectedItem.ToString(),
                Fecha = Timestamp.FromDateTime(DatePickerFecha.Date.ToUniversalTime()),
                Descripcion = EntryDescripcion.Text,
                Cuenta = cuentaPicker.SelectedItem.ToString(),
                Nota = EditorNota.Text
            };

            await _finanzasService.AgregarGastoAsync(gasto);
            var gastos = await _finanzasService.ObtenerGastosUsuarioAsync();

            LimpiarFormulario();

            await DisplayAlert("Gastos", "Gasto guardado correctamente", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al guardar: {ex.Message}", "OK");
        }
    }

    private async void OnActualizarClicked(object sender, EventArgs e)
    {
        try
        {
            if (gastoSeleccionado == null || !ValidarCampos())
                return;

            gastoSeleccionado.Monto = double.Parse(EntryMonto.Text);
            gastoSeleccionado.Categoria = categoriaPicker.SelectedItem.ToString();
            gastoSeleccionado.Fecha = Timestamp.FromDateTime(DatePickerFecha.Date.ToUniversalTime());
            gastoSeleccionado.Descripcion = EntryDescripcion.Text;
            gastoSeleccionado.Cuenta = cuentaPicker.SelectedItem.ToString();
            gastoSeleccionado.Nota = EditorNota.Text;

            await _finanzasService.ActualizarGastoAsync(gastoSeleccionado);
            var gastos = await _finanzasService.ObtenerGastosUsuarioAsync();
            // Actualizar la colecci�n observable
            int index = gastos.IndexOf(gastoSeleccionado);
            if (index >= 0)
            {
                gastos[index] = gastoSeleccionado;
            }

            LimpiarFormulario();
            CambiarModoEdicion(false);

            await DisplayAlert("Gastos", "Gasto actualizado correctamente", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al actualizar: {ex.Message}", "OK");
        }
    }

    private void OnCancelarClicked(object sender, EventArgs e)
    {
        LimpiarFormulario();
        CambiarModoEdicion(false);
    }
    
    private bool ValidarCampos()
    {
        if (string.IsNullOrWhiteSpace(EntryMonto.Text) ||
            !double.TryParse(EntryMonto.Text, out double monto) || monto <= 0)
        {
            DisplayAlert("Error", "Ingrese un monto v�lido", "OK");
            return false;
        }

        if (categoriaPicker.SelectedItem == null)
        {
            DisplayAlert("Error", "Seleccione una categor�a", "OK");
            return false;
        }

        if (string.IsNullOrWhiteSpace(EntryDescripcion.Text))
        {
            DisplayAlert("Error", "La descripci�n es obligatoria", "OK");
            return false;
        }

        if (cuentaPicker.SelectedItem == null)
        {
            DisplayAlert("Error", "La cuenta es obligatoria", "OK");
            return false;
        }

        return true;
    }

    private void LimpiarFormulario()
    {
        EntryMonto.Text = string.Empty;
        categoriaPicker.SelectedIndex = -1;
        DatePickerFecha.Date = DateTime.Now;
        EntryDescripcion.Text = string.Empty;
        cuentaPicker.SelectedIndex = -1;
        EditorNota.Text = string.Empty;
        gastoSeleccionado = null;
        EntryMonto.Focus();
        CargarGastosHistoricos();
    }

    private void CargarDatosEnFormulario(Gasto gasto)
    {
        EntryMonto.Text = gasto.Monto.ToString();
        categoriaPicker.SelectedItem = gasto.Categoria;
        DatePickerFecha.Date = gasto.Fecha.ToDateTime();
        EntryDescripcion.Text = gasto.Descripcion;
        cuentaPicker.SelectedItem = gasto.Cuenta;
        EditorNota.Text = gasto.Nota;
    }

    private void CambiarModoEdicion(bool esEdicion)
    {
        BtnGuardar.IsVisible = !esEdicion;
        BtnActualizar.IsVisible = esEdicion;
        gastoSeleccionado = esEdicion ? gastoSeleccionado : null;
    }

    private void listadoGastos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var gasto = (Gasto) e.SelectedItem;
        if (gasto != null)
        {
            CargarDatosEnFormulario(gasto);
            gastoSeleccionado = gasto;
            CambiarModoEdicion(true);
        }
        else
        {
            LimpiarFormulario();
            CambiarModoEdicion(false);
        }
    }

    private async void btnEliminar_Clicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            var gasto = button?.BindingContext as Gasto;

            if (gasto == null) return;

            bool confirmar = await DisplayAlert("Confirmar", "¿Está seguro de eliminar el gasto?", "Si", "No");

            if (confirmar)
            {
                await _finanzasService.EliminarGastosAsync(gasto.Id);

                if (gastoSeleccionado?.Id == gasto.Id)
                {
                    LimpiarFormulario();
                    CambiarModoEdicion(false);
                }
                CargarGastosHistoricos();
                await DisplayAlert("Eliminar", "Gasto eliminado correctamente", "Aceptar");
            }
            
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al eliminar: {ex.Message}", "Aceptar");
        }
    }
}