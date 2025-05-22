using System.Collections.ObjectModel;
using app_s8.Models;
using app_s8.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace app_s8.ViewModels
{
    public partial class ChartDataItem : ObservableObject
    {
        [ObservableProperty]
        private string _label;
        [ObservableProperty]
        private double _value;

    }

    public partial class DashboardViewModel : ObservableObject
    {
        private readonly UserDataService _userDataService;
        [ObservableProperty]
        private double _saldoActual;
        [ObservableProperty]
        private ObservableCollection<ChartDataItem> _cuentasParaGrafico = new();
        [ObservableProperty]
        private ObservableCollection<Gasto> _movimientosGastosRecientes = new();
        [ObservableProperty]
        private ObservableCollection<Ingreso> _movimientosIngresosRecientes = new();

        public IAsyncRelayCommand CargarDatosDashboardCommand { get; }

        public DashboardViewModel(UserDataService userDataService)
        {
            _userDataService = userDataService;
            CargarDatosDashboardCommand = new AsyncRelayCommand(CargarDatosDashboardAsync);
            WeakReferenceMessenger.Default.Register<UserDataUpdatedMessage>(this, async (r, m) =>
            {
                await r.CargarDatosDashboardAsync();
            });
        }

        private async Task CargarDatosDashboardAsync()
        {
           
        }
    }


    public class UserDataUpdatedMessage : ValueChangedMessage<Usuario>
    {
        public UserDataUpdatedMessage(Usuario value) : base(value) { }
    }
}
