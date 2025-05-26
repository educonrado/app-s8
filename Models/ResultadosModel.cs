namespace app_s8.Models
{
    public class ResultadosModel
    {    
        public string Mes { get; set; }
        public double MontoIngresos { get; set; }
        public double MontoGastos { get; set; }

        public ResultadosModel(string mes, double ingresos, double gastos)
        {
            Mes = mes;
            MontoIngresos = ingresos;
            MontoGastos = gastos;
        }
    }
}
