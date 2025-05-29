using app_s8.Models;
using app_s8.Services;
using Google.Cloud.Firestore;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace app_s8.Views;

public partial class GastoPage : ContentPage
{
    private double total;
    private ObservableCollection<Gasto> gastos;
    private Gasto gastoSeleccionado;
    private FirestoreDb db;
    private readonly FinanzasService _finanzasService;

    public GastoPage()
    {
        InitializeComponent();
        //InicializarFirestore();
        FirestoreService.Initialize();
        db = FirestoreService.GetFirestoreDb();

        gastos = new ObservableCollection<Gasto>();
        CollectionViewGastos.ItemsSource = gastos;
        

    }



    public GastoPage(double total)
    {
        InitializeComponent();
        _finanzasService = new FinanzasService();
        CargarValoresPorDefecto(total);
    }

    private void CargarValoresPorDefecto(double total)
    {
        Debug.WriteLine("Prueba " + total);
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
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

            DocumentReference docRef = await db.Collection("gastos").AddAsync(gasto);
            gasto.Id = docRef.Id;

            gastos.Add(gasto);
            LimpiarFormulario();

            await DisplayAlert("Éxito", "Gasto guardado correctamente", "OK");
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

            DocumentReference docRef = db.Collection("gastos").Document(gastoSeleccionado.Id);
            await docRef.SetAsync(gastoSeleccionado);

            // Actualizar la colección observable
            int index = gastos.IndexOf(gastoSeleccionado);
            if (index >= 0)
            {
                gastos[index] = gastoSeleccionado;
            }

            LimpiarFormulario();
            CambiarModoEdicion(false);

            await DisplayAlert("Éxito", "Gasto actualizado correctamente", "OK");
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

    private async void OnCargarGastosClicked(object sender, EventArgs e)
    {
        try
        {
            gastos.Clear();

            Query gastosQuery = db.Collection("gastos").OrderByDescending("fecha");
            QuerySnapshot querySnapshot = await gastosQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot document in querySnapshot.Documents)
            {
                if (document.Exists)
                {
                    var gasto = document.ConvertTo<Gasto>();
                    gasto.Id = document.Id;
                    gastos.Add(gasto);
                }
            }

            await DisplayAlert("Éxito", $"Se cargaron {gastos.Count} gastos", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar gastos: {ex.Message}", "OK");
        }
    }

    private void OnGastoSeleccionado(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Gasto gasto)
        {
            gastoSeleccionado = gasto;
            CargarDatosEnFormulario(gasto);
            CambiarModoEdicion(true);
        }
    }

    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is Button button && button.CommandParameter is Gasto gasto)
            {
                bool confirmar = await DisplayAlert("Confirmar",
                    $"¿Está seguro de eliminar el gasto de {gasto.Categoria}?",
                    "Sí", "No");

                if (confirmar)
                {
                    DocumentReference docRef = db.Collection("gastos").Document(gasto.Id);
                    await docRef.DeleteAsync();

                    gastos.Remove(gasto);

                    if (gastoSeleccionado?.Id == gasto.Id)
                    {
                        LimpiarFormulario();
                        CambiarModoEdicion(false);
                    }

                    await DisplayAlert("Éxito", "Gasto eliminado correctamente", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al eliminar: {ex.Message}", "OK");
        }
    }
    private bool ValidarCampos()
    {
        if (string.IsNullOrWhiteSpace(EntryMonto.Text) ||
            !double.TryParse(EntryMonto.Text, out double monto) || monto <= 0)
        {
            DisplayAlert("Error", "Ingrese un monto válido", "OK");
            return false;
        }

        if (categoriaPicker.SelectedItem == null)
        {
            DisplayAlert("Error", "Seleccione una categoría", "OK");
            return false;
        }

        if (string.IsNullOrWhiteSpace(EntryDescripcion.Text))
        {
            DisplayAlert("Error", "La descripción es obligatoria", "OK");
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
}