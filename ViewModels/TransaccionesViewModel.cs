using app_s8.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_s8.ViewModels
{
    public class TransaccionesViewModel
    {
        public List<ResumenTransacciones> Transacciones { get; set; }
        public TransaccionesViewModel(List<ResumenTransacciones> transacciones = null)
        {
            Transacciones = transacciones ?? new List<ResumenTransacciones>();
        }
    }
}
