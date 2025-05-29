using app_s8.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_s8.ViewModels
{
    public class IngresosViewModel
    {
        public List<Ingreso> Ingresos { get; set; }
        public IngresosViewModel(List<Ingreso> ingresos = null)
        {
            Ingresos = ingresos ?? new List<Ingreso>();
        }
    }
}
