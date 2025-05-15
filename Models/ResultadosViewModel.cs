using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_s8.Models
{
    public class ResultadosViewModel
    {
        public List<ResultadosModel> Resultados { get; set; }
        public ResultadosViewModel() 
        {
            Resultados = new List<ResultadosModel>()
            {
                new ResultadosModel("Ene", 2500.95, 1235.36),
                new ResultadosModel("Feb", 2100.95, 1685.36),
                new ResultadosModel("Mar", 2000.95, 1895.36),
                new ResultadosModel("Abr", 2800.95, 2016.93),
                new ResultadosModel("May", 2897.95, 3015.86),
                new ResultadosModel("Jun", 1500.95, 2063.09),
            };
        }
    }
}
