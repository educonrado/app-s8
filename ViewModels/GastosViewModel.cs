using app_s8.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_s8.ViewModels
{
    public class GastosViewModel
    {
        public List<Gasto> Gastos { get; set; }
        public GastosViewModel(List<Gasto> gastos = null)
        {
            Gastos = gastos ?? new List<Gasto>();
        }
    }
}
