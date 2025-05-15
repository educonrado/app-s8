using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_s8.Models
{
    public class CuentasViewModel
    {
        public List<CuentasModel> Cuentas { get; set; }
        
        public CuentasViewModel() 
        { 
            Cuentas = new List<CuentasModel>()
            {
                new CuentasModel("Efectivo", 12000),
                new CuentasModel("Banco", 14000),
                new CuentasModel("Poliza", 20000),
            };
        }



    }
}
