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

    public DashboardPage()
    {
        InitializeComponent();
        _finanzasService = new FinanzasService();
        BindingContext = this;
        InicializarDatosVacios();
        _ = CargarDatosDashboardAsync();
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
            Debug.WriteLine($"Error cargando datos de gráficas: {ex.Message}");
            ResultadosViewModel = new ResultadosViewModel();
            CuentasViewModel = new CuentasViewModel();
            TransaccionesViewModel = new TransaccionesViewModel();
        }
    }

    private async void btnGuardar_Clicked(object sender, EventArgs e)
    {
        try
        {
            var nuevaCuenta = new Cuenta
            {
                NombreCuenta = "Cuenta de Prueba",
                Monto = 1250.26
            };

            await _finanzasService.AgregarCuentaAsync(nuevaCuenta);

            await CargarDatosDashboardAsync();

            await DisplayAlert("Éxito", "Cuenta agregada correctamente", "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error agregando cuenta: {ex.Message}");
            await DisplayAlert("Error", "Error agregando cuenta", "OK");
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

    private async void btnIngreso_Clicked(object sender, EventArgs e)
    {
        Ingreso ingreso = new Ingreso
        {
            Categoria = "Venta",
            Cuenta = "Efectivo",
            Monto = 202.30,
            Nota = "Cliente Caro",
            Fecha = Timestamp.GetCurrentTimestamp()
        };
        _ = _finanzasService.AgregarIngresoAsync(ingreso);
        await RefrescarDatosAsync();
    }

    private async void btnGasto_Clicked(object sender, EventArgs e)
    {
        Gasto gasto = new()
        {
            Categoria = "Insumos",
            Cuenta = "Efectivo",
            Monto = 22.30,
            Fecha = Timestamp.GetCurrentTimestamp(),
            Nota = "Compra de insumos"
        };
        _ = _finanzasService.AgregarGastoAsync(gasto);
        await RefrescarDatosAsync();
    }
}