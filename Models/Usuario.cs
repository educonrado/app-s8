using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace app_s8.Models
{
    [FirestoreData]
    public class Usuario
    {
        
        [FirestoreProperty("gastos")]

        public List<Gasto> Gastos { get; set; } = new List<Gasto>();
        [FirestoreProperty("cuentas")]
        public List<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
        [FirestoreProperty("ingresos")]
        public List<Ingreso> Ingresos { get; set; } = new List<Ingreso>();
        
        [FirestoreProperty("ultima_actualizacion")]
        public Timestamp UltimaActualizacion { get; set; }
        public Usuario() { }

        public double CalcularSaldo()
        {
            return Cuentas?.Sum(cuenta => cuenta.Monto) ?? 0.0;
        }

        //Devuelve los últimas 50 transacciones
        public List<ResumenTransacciones> ObtenerUltimasTransacciones()
        {
            var transacciones = new List<ResumenTransacciones>();
            if (Gastos != null)
            {
                transacciones.AddRange(Gastos.Select(gasto => new ResumenTransacciones
                {
                    Id = gasto.Id,
                    Tipo = "Gasto",
                    Monto = gasto.Monto,
                    Descripcion = gasto.Descripcion,
                    Categoria = gasto.Categoria,
                    Fecha = gasto.Fecha,
                    Cuenta = gasto.Cuenta
                }));
            }
            if (Ingresos != null)
            {
                transacciones.AddRange(Ingresos.Select(ingreso => new ResumenTransacciones
                {
                    Id = ingreso.Id,
                    Tipo = "Ingreso",
                    Monto = ingreso.Monto,
                    Descripcion = ingreso.Descripcion,
                    Categoria = ingreso.Categoria,
                    Fecha = ingreso.Fecha,
                    Cuenta = ingreso.Cuenta
                }));
            }
            // Similar a JS para devolver una lista
            return [.. transacciones
                .OrderByDescending(transaccion => transaccion.Fecha.ToDateTime())
                .Take(50)];
        }

        public List<ResumenTransacciones> ObtenerUltimosIngresos()
        {
            var transacciones = new List<ResumenTransacciones>();
            
            if (Ingresos != null)
            {
                transacciones.AddRange(Ingresos.Select(ingreso => new ResumenTransacciones
                {
                    Id = ingreso.Id,
                    Tipo = "Ingreso",
                    Monto = ingreso.Monto,
                    Descripcion = ingreso.Descripcion,
                    Categoria = ingreso.Categoria,
                    Fecha = ingreso.Fecha,
                    Cuenta = ingreso.Cuenta
                }));
            }
            // Similar a JS para devolver una lista
            return [.. transacciones
                .OrderByDescending(transaccion => transaccion.Fecha.ToDateTime())
                .Take(50)];
        }

        public List<ResumenTransacciones> ObtenerUltimosGastos()
        {
            var transacciones = new List<ResumenTransacciones>();
            if (Gastos != null)
            {
                transacciones.AddRange(Gastos.Select(gasto => new ResumenTransacciones
                {
                    Id = gasto.Id,
                    Tipo = "Gasto",
                    Monto = gasto.Monto,
                    Descripcion = gasto.Descripcion,
                    Categoria = gasto.Categoria,
                    Fecha = gasto.Fecha,
                    Cuenta = gasto.Cuenta
                }));
            }
            
            return [.. transacciones
                .OrderByDescending(transaccion => transaccion.Fecha.ToDateTime())
                .Take(50)];
        }

        public List<ResultadosModel> ObtenerResumenUltimos6Meses()
        {
            var fechaInicio = DateTime.Now.AddMonths(-5).Date;
            var resumen = new List<ResultadosModel>();

            for (int i = 5; i >=0 ; i--)
            {
                var mes = DateTime.Now.AddMonths(-i);
                var nombresMes = mes.ToString("MMM", System.Globalization.CultureInfo.GetCultureInfo("es-ES"));

                var gastosDelMes = Gastos?
                    .Where(gasto => gasto.Fecha.ToDateTime().Year == mes.Year 
                    && gasto.Fecha.ToDateTime().Month == mes.Month)
                    .Sum(gasto => gasto.Monto) ?? 0.0;
                
                var ingresoDelMes = Ingresos?
                    .Where(ingreso => ingreso.Fecha.ToDateTime().Year == mes.Year 
                    && ingreso.Fecha.ToDateTime().Month == mes.Month)
                    .Sum(ingreso => ingreso.Monto) ?? 0.0;

                resumen.Add(new ResultadosModel(nombresMes, ingresoDelMes, gastosDelMes));
            }

            return resumen;
        }
                
    }
}
