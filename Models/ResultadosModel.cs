using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_s8.Models
{
    public class ResultadosModel
    {    
        public string Mes { get; set; }
        public double MontoVentas { get; set; }
        public double MontoGastos { get; set; }

        public ResultadosModel(string mes, double montoVentas, double montoGastos)
        {
            Mes = mes;
            MontoVentas = montoVentas;
            MontoGastos = montoGastos;
        }
    }
}
