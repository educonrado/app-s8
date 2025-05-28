
using app_s8.Models;

namespace app_s8.ViewModels
{
    public class ResultadosViewModel
    {
        public List<ResultadosModel> Resultados { get; set; }
        public ResultadosViewModel(List<ResultadosModel> resultados = null) 
        {
            Resultados = resultados ?? new List<ResultadosModel>()
            {
                new ResultadosModel("Ene", 1, 0),
                new ResultadosModel("Feb", 0, 1),
                new ResultadosModel("Mar", 1, 0),
                new ResultadosModel("Abr", 0, 1),
                new ResultadosModel("May", 1, 0),
                new ResultadosModel("Jun", 0, 1),
            };
        }
    }
}
