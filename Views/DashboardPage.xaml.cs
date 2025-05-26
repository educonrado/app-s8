using app_s8.Models;
using app_s8.Services;
using app_s8.ViewModels;
using System.ComponentModel;
using System.Diagnostics;

namespace app_s8.Views;

public partial class DashboardPage : ContentPage, INotifyPropertyChanged
{
   
    private readonly FinanzasService _finanzasService;

    private double _balanceTotal;
    private double _ingresosDelMes;
    private double _gastosDelMes;
    private bool _isLoading;

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

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    // AGREGADO: Propiedades formateadas para mostrar en UI
    public string BalanceTotalFormateado => $"${BalanceTotal:N2}";
    public string IngresosDelMesFormateado => $"${IngresosDelMes:N2}";
    public string GastosDelMesFormateado => $"${GastosDelMes:N2}";


    private async void ObtenerDatos(string currentUserId)
    {
        //Usuario usuario = await repository.ObtenerUsuarioPorUid(currentUserId);
        
    }

    // AGREGADO: ViewModels como propiedades
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

        // CAMBIO: Inicializar FinanzasService
        _finanzasService = new FinanzasService();

        // AGREGADO: Establecer BindingContext
        BindingContext = this;

        // CAMBIO: Cargar datos reales en lugar de hardcodeados
        _ = CargarDatosDashboardAsync();
    }

    // M�TODO PRINCIPAL - REEMPLAZA CargarDatosPanel() y ObtenerDatos()
    private async Task CargarDatosDashboardAsync()
    {
        try
        {
            IsLoading = true;

            // Verificar que hay usuario logueado
            if (string.IsNullOrEmpty(UserService.Instancia.CurrentUserId))
            {
                await DisplayAlert("Error", "No hay usuario logueado", "OK");
                return;
            }

            // AGREGADO: Cargar datos del usuario
            Usuario usuario = await _finanzasService.CargarOCrearDatosUsuarioAsync();

            // AGREGADO: Calcular m�tricas del dashboard
            await CalcularMetricasDashboard(usuario);

            // AGREGADO: Cargar datos para las gr�ficas
            await CargarDatosGraficasAsync();

            Debug.WriteLine($"Dashboard cargado - Balance: {BalanceTotal}, Ingresos: {IngresosDelMes}, Gastos: {GastosDelMes}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error cargando dashboard: {ex.Message}");
            await DisplayAlert("Error", "Error cargando datos del dashboard", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    // AGREGADO: M�todo para calcular m�tricas
    private async Task CalcularMetricasDashboard(Usuario usuario)
    {
        // Balance total (suma de todas las cuentas)
        BalanceTotal = usuario.CalcularSaldo();

        // Calcular ingresos y gastos del mes actual
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

    // AGREGADO: M�todo para cargar datos de gr�ficas
    private async Task CargarDatosGraficasAsync()
    {
        try
        {
            // Cargar datos para gr�fica de l�neas (�ltimos 6 meses)
            var datosGraficos = await _finanzasService.ObtenerDatosGraficosAsync();
            ResultadosViewModel = new ResultadosViewModel(datosGraficos);

            // Cargar datos para gr�fica de pie (cuentas)
            var datosCuentas = await _finanzasService.ObtenerDatosCuentasAync();
            CuentasViewModel = new CuentasViewModel(datosCuentas);

            // Cargar datos para tabla de transacciones
            var datosTransacciones = await _finanzasService.ObtenerUltimas50TransaccionesAsync();
            TransaccionesViewModel = new TransaccionesViewModel(datosTransacciones);

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error cargando datos de gr�ficas: {ex.Message}");
            // En caso de error, usar datos de ejemplo
            ResultadosViewModel = new ResultadosViewModel();
            CuentasViewModel = new CuentasViewModel();
            TransaccionesViewModel = new TransaccionesViewModel();
        }
    }

    // MODIFICADO: Bot�n de prueba mejorado
    private async void btnGuardar_Clicked(object sender, EventArgs e)
    {
        try
        {
            IsLoading = true;

            // CAMBIO: Usar FinanzasService en lugar de repository
            var nuevaCuenta = new Cuenta
            {
                NombreCuenta = "Cuenta de Prueba",
                Monto = 1250.26
            };

            await _finanzasService.AgregarCuentaAsync(nuevaCuenta);

            // AGREGADO: Recargar dashboard despu�s de agregar
            await CargarDatosDashboardAsync();

            await DisplayAlert("�xito", "Cuenta agregada correctamente", "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error agregando cuenta: {ex.Message}");
            await DisplayAlert("Error", "Error agregando cuenta", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }
    // AGREGADO: M�todo para refrescar datos (�til para pull-to-refresh)
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