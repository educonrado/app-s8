using app_s8.Models;
using app_s8.Services;
using app_s8.ViewModels;
using Google.Cloud.Firestore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace app_s8.Views;

public partial class DashboardPage : ContentPage, INotifyPropertyChanged
{
   
    private readonly FinanzasService _finanzasService;

    private double _balanceTotal;
    private double _ingresosDelMes;
    private double _gastosDelMes;
    

    private ResultadosViewModel _resultadosViewModel;
    private CuentasViewModel _cuentasViewModel;
    private TransaccionesViewModel _transaccionesViewModel;

    public DashboardPage()
    {
        InitializeComponent();
        _finanzasService = new FinanzasService();
        BindingContext = this;
        InicializarDatosVacios();
        _ = CargarDatosDashboardAsync();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = CargarDatosDashboardAsync();
    }
    public double BalanceTotal
    {
        get => _balanceTotal;
        set
        {
            _balanceTotal = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(BalanceTotalFormateado));
        }
    }

    public double IngresosDelMes
    {
        get => _ingresosDelMes;
        set
        {
            _ingresosDelMes = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IngresosDelMesFormateado));
        }
    }

    public double GastosDelMes
    {
        get => _gastosDelMes;
        set
        {
            _gastosDelMes = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(GastosDelMesFormateado));
        }
    }

    public string BalanceTotalFormateado => $"${BalanceTotal:N2}";
    public string IngresosDelMesFormateado => $"${IngresosDelMes:N2}";
    public string GastosDelMesFormateado => $"${GastosDelMes:N2}";

    public ResultadosViewModel ResultadosViewModel
    {
        get => _resultadosViewModel;
        set
        {
            _resultadosViewModel = value;
            OnPropertyChanged();
        }
    }

    public CuentasViewModel CuentasViewModel
    {
        get => _cuentasViewModel;
        set
        {
            _cuentasViewModel = value;
            OnPropertyChanged();
        }
    }

    public TransaccionesViewModel TransaccionesViewModel
    {
        get => _transaccionesViewModel;
        set
        {
            _transaccionesViewModel = value;
            OnPropertyChanged();
        }
    }

    private void InicializarDatosVacios()
    {
        BalanceTotal = 0;
        IngresosDelMes = 0;
        GastosDelMes = 0;

        _resultadosViewModel = new ResultadosViewModel
        {
            
        };

        _cuentasViewModel = new CuentasViewModel
        {
            
        };
    }

    private async Task CargarDatosDashboardAsync()
    {
        try
        {

            if (string.IsNullOrEmpty(UserService.Instancia.CurrentUserId))
            {
                await DisplayAlert("Error", "No hay usuario logueado", "OK");
                return;
            }

            Usuario usuario = await _finanzasService.CargarOCrearDatosUsuarioAsync();

            await CalcularMetricasDashboard(usuario);

            await CargarDatosGraficasAsync();

            Debug.WriteLine($"Dashboard cargado - Balance: {BalanceTotal}, Ingresos: {IngresosDelMes}, Gastos: {GastosDelMes}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error cargando dashboard: {ex.Message}");
            await DisplayAlert("Error", "Error cargando datos del dashboard", "OK");
        }
    }
    private async Task CalcularMetricasDashboard(Usuario usuario)
    {
        BalanceTotal = usuario.CalcularSaldo();
        var mesActual = DateTime.Now;
        var inicioMes = new DateTime(mesActual.Year, mesActual.Month, 1);
        var finMes = inicioMes.AddMonths(1).AddDays(-1);

        IngresosDelMes = usuario.Ingresos?
            .Where(i => i.Fecha.ToDateTime() >= inicioMes && i.Fecha.ToDateTime() <= finMes)
            .Sum(i => i.Monto) ?? 0.0;

        GastosDelMes = usuario.Gastos?
            .Where(g => g.Fecha.ToDateTime() >= inicioMes && g.Fecha.ToDateTime() <= finMes)
            .Sum(g => g.Monto) ?? 0.0;
    }

    private async Task CargarDatosGraficasAsync()
    {
        try
        {
            var datosGraficos = await _finanzasService.ObtenerDatosGraficosAsync();
            ResultadosViewModel = new ResultadosViewModel(datosGraficos);

            var datosCuentas = await _finanzasService.ObtenerDatosCuentasAync();
            CuentasViewModel = new CuentasViewModel(datosCuentas);

            var datosTransacciones = await _finanzasService.ObtenerUltimas50TransaccionesAsync();
            TransaccionesViewModel = new TransaccionesViewModel(datosTransacciones);

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error cargando datos de gr�ficas: {ex.Message}");
            ResultadosViewModel = new ResultadosViewModel();
            CuentasViewModel = new CuentasViewModel();
            TransaccionesViewModel = new TransaccionesViewModel();
        }
    }

    public async Task RefrescarDatosAsync()
    {
        await CargarDatosDashboardAsync();
    }
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    
}