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
    }

    private void GuardarPreferencias()
    {
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

    private void btnCerrarSesion_Clicked(object sender, EventArgs e)
    {
        UserService.Instancia.ClearUserId();

        Application.Current.MainPage = new NavigationPage(new Views.LoginPage());
    }
}
