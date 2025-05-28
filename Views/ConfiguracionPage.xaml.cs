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

    }

    private void GuardarPreferencias()
    {
        Preferences.Set("ActivarNotificaciones", NotificacionesSwitch.IsToggled);
        Preferences.Set("IngresosCategorias", string.Join(",", IngresosCategorias));
        Preferences.Set("EgresosCategorias", string.Join(",", EgresosCategorias));
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
            bool confirmar = await DisplayAlert("Eliminar", $"¿Eliminar '{cat}'?", "Sí", "No");
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
            bool confirmar = await DisplayAlert("Eliminar", $"¿Eliminar '{cat}'?", "Sí", "No");
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
            string nuevo = await DisplayPromptAsync("Editar Categoría", "Nuevo nombre:", initialValue: cat);
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
            string nuevo = await DisplayPromptAsync("Editar Categoría", "Nuevo nombre:", initialValue: cat);
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

    private void btnCerrarSesion_Clicked(object sender, EventArgs e)
    {
        UserService.Instancia.ClearUserId();

        Application.Current.MainPage = new NavigationPage(new Views.LoginPage());
    }

   

    
}