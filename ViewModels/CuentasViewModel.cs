using app_s8.Models;

namespace app_s8.ViewModels
{
    public class CuentasViewModel
    {
        public List<CuentasModel> Cuentas { get; set; }
        
        public CuentasViewModel(List<CuentasModel> cuentas = null) 
        { 
            Cuentas = cuentas ?? new List<CuentasModel>()
            {
                new CuentasModel("Efectivo", 10),
                new CuentasModel("Banco", 20),
                new CuentasModel("Poliza", 5),
            };
        }



    }
}
