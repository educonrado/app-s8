using app_s8.Models;
using app_s8.Services;
using Google.Cloud.Firestore;
using System.Diagnostics;

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
            LimpiarCampos();
            await DisplayAlert("Éxito", "Ingreso guardado correctamente", "OK");

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


}