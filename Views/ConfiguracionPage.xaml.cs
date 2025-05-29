using System.Collections.ObjectModel;
using System.Diagnostics;
using app_s8.Models;
using app_s8.Services;
using Google.Cloud.Firestore;

namespace app_s8.Views;

public partial class ConfiguracionPage : ContentPage
{

    public ObservableCollection<string> IngresosCategorias { get; set; } = new() { "Ventas", "Servicios" };
    public ObservableCollection<string> EgresosCategorias { get; set; } = new() { "Alquiler", "Marketing" };
    public ObservableCollection<string> Cuentas { get; set; } = new() { "Cuenta Principal", "Cuenta Ahorros" };
    private byte[] _fotoTemporal;

    public ConfiguracionPage()

    {
        InitializeComponent();
        BindingContext = this;
        CargarPreferencias();
    }

    private void CargarPreferencias()
    {
        string monedaGuardada = Preferences.Get("MonedaPrincipal", "USD");
        int index = MonedaPicker.Items.IndexOf(monedaGuardada);
        if (index >= 0) MonedaPicker.SelectedIndex = index;

        NotificacionesSwitch.IsToggled = Preferences.Get("ActivarNotificaciones", false);

        var ingresos = Preferences.Get("IngresosCategorias", null);
        if (!string.IsNullOrEmpty(ingresos))
            IngresosCategorias = new(ingresos.Split(','));

        IngresosCategoriasCollectionView.ItemsSource = IngresosCategorias;

        var egresos = Preferences.Get("EgresosCategorias", null);
        if (!string.IsNullOrEmpty(egresos))
            EgresosCategorias = new(egresos.Split(','));

        EgresosCategoriasCollectionView.ItemsSource = EgresosCategorias;

        var cuentas = Preferences.Get("Cuentas", null);
        if (!string.IsNullOrEmpty(cuentas))
            Cuentas = new(cuentas.Split(','));

        CuentasCollectionView.ItemsSource = Cuentas;

        string fotoBase64 = Preferences.Get("FotoPerfil", null);
        if (!string.IsNullOrEmpty(fotoBase64))
        {
            _fotoTemporal = Convert.FromBase64String(fotoBase64);
            ImagenCapturada.Source = ImageSource.FromStream(() => new MemoryStream(_fotoTemporal));
        }
    }

    private void GuardarPreferencias()
    {
        if (MonedaPicker.SelectedItem != null)
            Preferences.Set("MonedaPrincipal", MonedaPicker.SelectedItem.ToString());

        Preferences.Set("ActivarNotificaciones", NotificacionesSwitch.IsToggled);
        Preferences.Set("IngresosCategorias", string.Join(",", IngresosCategorias));
        Preferences.Set("EgresosCategorias", string.Join(",", EgresosCategorias));
        Preferences.Set("Cuentas", string.Join(",", Cuentas));

        if (_fotoTemporal != null)
            Preferences.Set("FotoPerfil", Convert.ToBase64String(_fotoTemporal));
        else
            Preferences.Remove("FotoPerfil");
    }

    private void MonedaPicker_SelectedIndexChanged(object sender, EventArgs e) => GuardarPreferencias();
    private void NotificacionesSwitch_Toggled(object sender, ToggledEventArgs e) => GuardarPreferencias();

    private void AgregarIngresoCategoria_Clicked(object sender, EventArgs e)
    {
        var nueva = NuevoIngresoCategoriaEntry.Text?.Trim();
        if (!string.IsNullOrWhiteSpace(nueva) && !IngresosCategorias.Contains(nueva))
        {
            IngresosCategorias.Add(nueva);
            NuevoIngresoCategoriaEntry.Text = "";
            GuardarPreferencias();
        }
    }

    private void AgregarEgresoCategoria_Clicked(object sender, EventArgs e)
    {
        var nueva = NuevoEgresoCategoriaEntry.Text?.Trim();
        if (!string.IsNullOrWhiteSpace(nueva) && !EgresosCategorias.Contains(nueva))
        {
            EgresosCategorias.Add(nueva);
            NuevoEgresoCategoriaEntry.Text = "";
            GuardarPreferencias();
        }
    }

    private async void EliminarIngresoCategoria_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string cat)
        {
            bool confirmar = await DisplayAlert("Eliminar", $"�Eliminar '{cat}'?", "S�", "No");
            if (confirmar)
            {
                IngresosCategorias.Remove(cat);
                GuardarPreferencias();
            }
        }
    }

    private async void EliminarEgresoCategoria_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string cat)
        {
            bool confirmar = await DisplayAlert("Eliminar", $"�Eliminar '{cat}'?", "S�", "No");
            if (confirmar)
            {
                EgresosCategorias.Remove(cat);
                GuardarPreferencias();
            }
        }
    }

    private async void EditarIngresoCategoria_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string cat)
        {
            string nuevo = await DisplayPromptAsync("Editar Categor�a", "Nuevo nombre:", initialValue: cat);
            if (!string.IsNullOrWhiteSpace(nuevo))
            {
                int index = IngresosCategorias.IndexOf(cat);
                if (index >= 0)
                {
                    IngresosCategorias[index] = nuevo;
                    GuardarPreferencias();
                }
            }
        }
    }

    private async void EditarEgresoCategoria_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string cat)
        {
            string nuevo = await DisplayPromptAsync("Editar Categor�a", "Nuevo nombre:", initialValue: cat);
            if (!string.IsNullOrWhiteSpace(nuevo))
            {
                int index = EgresosCategorias.IndexOf(cat);
                if (index >= 0)
                {
                    EgresosCategorias[index] = nuevo;
                    GuardarPreferencias();
                }
            }
        }
    }

    private void AgregarCuenta_Clicked(object sender, EventArgs e)
    {
        var nueva = NuevaCuentaEntry.Text?.Trim();
        if (!string.IsNullOrWhiteSpace(nueva) && !Cuentas.Contains(nueva))
        {
            Cuentas.Add(nueva);
            NuevaCuentaEntry.Text = "";
            GuardarPreferencias();
        }
    }

    private async void EliminarCuenta_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string cuenta)
        {
            bool confirmar = await DisplayAlert("Eliminar", $"�Eliminar '{cuenta}'?", "S�", "No");
            if (confirmar)
            {
                Cuentas.Remove(cuenta);
                GuardarPreferencias();
            }
        }
    }

    private async void EditarCuenta_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string cuenta)
        {
            string nuevo = await DisplayPromptAsync("Editar Cuenta", "Nuevo nombre:", initialValue: cuenta);
            if (!string.IsNullOrWhiteSpace(nuevo))
            {
                int index = Cuentas.IndexOf(cuenta);
                if (index >= 0)
                {
                    Cuentas[index] = nuevo;
                    GuardarPreferencias();
                }
            }
        }
    }

    private async void OnTomarFotoClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await MediaPicker.Default.CapturePhotoAsync();

            if (result != null)
            {
                var stream = await result.OpenReadAsync();
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    _fotoTemporal = memoryStream.ToArray();

                    await DisplayAlert("Factura Capturada", $"Tama�o: {_fotoTemporal.Length} bytes.", "OK");

                    ImagenCapturada.Source = ImageSource.FromStream(() => new MemoryStream(_fotoTemporal));

                    GuardarPreferencias();
                }
            }
            else
            {
                await DisplayAlert("Cancelado", "La captura fue cancelada.", "OK");
            }
        }
        catch (PermissionException ex)
        {
            await DisplayAlert("Permiso Denegado", $"Otorga permisos de c�mara: {ex.Message}", "OK");
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("No compatible", "Captura de fotos no compatible en este dispositivo.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurri� un error: {ex.Message}", "OK");
        }
    }
}
