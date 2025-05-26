namespace app_s8.Models
{

    public class CuentasModel
    {

        public string NombreCuenta {  get; set; }
        public double Monto { get; set;}

        public CuentasModel(string nombreCuenta, double monto)
        {
            NombreCuenta = nombreCuenta;
            Monto = monto;
        }
    }

}
