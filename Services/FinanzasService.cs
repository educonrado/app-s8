using app_s8.Models;
using Google.Cloud.Firestore;

namespace app_s8.Services
{
    public class FinanzasService
    {
        private readonly FirestoreDb _db;
        private readonly UserService _userService;

        private Usuario _usuarioCache;
        private DateTime _ultimaActualizacionCache;

        public FinanzasService()
        {
            FirestoreService.Initialize();
            _db = FirestoreService.FirestoreDb;
            _userService = UserService.Instancia;
        }

        public async Task<Usuario> CargarOCrearDatosUsuarioAsync(bool forzarRecarga = false)
        {
            string uid = _userService.CurrentUserId;
            if (string.IsNullOrEmpty(uid))
            {
                throw new InvalidOperationException("UID del usuario no disponible.");
            }

            if (!forzarRecarga && _usuarioCache != null && (DateTime.Now - _ultimaActualizacionCache).TotalMinutes < 5)
            {
                return _usuarioCache;
            }

            DocumentReference docRef = FirestoreService.ObtenerDocumentReferenceUsuario(uid);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            Usuario usuario;
            if (snapshot.Exists)
            {
                usuario = snapshot.ConvertTo<Usuario>();
            }
            else
            {
                usuario = new Usuario
                {
                    Gastos = new List<Gasto>(),
                    Ingresos = new List<Ingreso>(),
                    Cuentas = new List<Cuenta>
                    {
                        new Cuenta{Id = Guid.NewGuid().ToString(), NombreCuenta = "Efectivo", Monto = 0.0 }
                    },
                    UltimaActualizacion = Timestamp.GetCurrentTimestamp()

                };
                await docRef.SetAsync(usuario);
            }

            _usuarioCache = usuario;
            _ultimaActualizacionCache = DateTime.Now;

            return usuario;

        }

        public async Task ActualizarUsuarioAsync(Usuario usuario)
        {
            string uid = _userService.CurrentUserId;
            if (string.IsNullOrEmpty(uid))
            {
                throw new InvalidOperationException("UID del usuario no disponible.");
            }

            usuario.UltimaActualizacion = Timestamp.GetCurrentTimestamp();

            DocumentReference docRef = FirestoreService.ObtenerDocumentReferenceUsuario(uid);
            await docRef.SetAsync(usuario);

            _usuarioCache = usuario;
            _ultimaActualizacionCache = DateTime.Now;
        }

        public async Task AgregarGastoAsync(Gasto nuevoGasto)
        {
            Usuario usuario = await CargarOCrearDatosUsuarioAsync();
            if (usuario == null) return;

            nuevoGasto.Id = Guid.NewGuid().ToString();
            if (nuevoGasto.Fecha == null) nuevoGasto.Fecha = Timestamp.GetCurrentTimestamp();

            usuario.Gastos.Add(nuevoGasto);

            ActualizarMontoCuenta(usuario, nuevoGasto.Cuenta, -nuevoGasto.Monto);

            await ActualizarUsuarioAsync(usuario);

        }

        

        public async Task AgregarIngresoAsync(Ingreso nuevoIngreso)
        {
            Usuario usuario = await CargarOCrearDatosUsuarioAsync();
            if (usuario == null) return;

            nuevoIngreso.Id = Guid.NewGuid().ToString();
            if (nuevoIngreso.Fecha == null) nuevoIngreso.Fecha = Timestamp.GetCurrentTimestamp();

            usuario.Ingresos.Add(nuevoIngreso);
            ActualizarMontoCuenta(usuario, nuevoIngreso.Cuenta, nuevoIngreso.Monto);

            await ActualizarUsuarioAsync(usuario);
        }

        public async Task AgregarCuentaAsync(Cuenta nuevaCuenta)
        {
            Usuario usuario = await CargarOCrearDatosUsuarioAsync();
            if (usuario == null) return;

            nuevaCuenta.Id = Guid.NewGuid().ToString();

            usuario.Cuentas.Add(nuevaCuenta);

            await ActualizarUsuarioAsync(usuario);
        }

        public async Task EliminarGastosAsync(string gastoId)
        {
            Usuario usuario = await CargarOCrearDatosUsuarioAsync();
            if (usuario == null) return;

            var gastoAEliminar = usuario.Gastos.FirstOrDefault(gasto => gasto.Id == gastoId);
            if (gastoAEliminar != null)
            {
                usuario.Gastos.Remove(gastoAEliminar);

                ActualizarMontoCuenta(usuario, gastoAEliminar.Cuenta, gastoAEliminar.Monto); 

                await ActualizarUsuarioAsync(usuario);

            }
        }

        public async Task EliminarIngresoAsync(string ingresoId)
        {
            Usuario usuario = await CargarOCrearDatosUsuarioAsync();
            if (usuario == null) return;

            var ingresoAEliminar = usuario.Ingresos.FirstOrDefault(ingreso => ingreso.Id == ingresoId);

            if (ingresoAEliminar != null)
            {
                usuario.Ingresos.Remove(ingresoAEliminar);

                ActualizarMontoCuenta(usuario, ingresoAEliminar.Cuenta, -ingresoAEliminar.Monto);
                await ActualizarUsuarioAsync(usuario);
            }
        }

        private void ActualizarMontoCuenta(Usuario usuario, string nombreCuenta, double monto)
        {
            var cuenta = usuario.Cuentas.FirstOrDefault(cuenta => cuenta.NombreCuenta == nombreCuenta);
            if (cuenta != null)
            {
                cuenta.Monto += monto;
            }
        }

        //Métodos para dashboard
        public async Task<List<CuentasModel>> ObtenerDatosCuentasAync()
        {
            Usuario usuario = await CargarOCrearDatosUsuarioAsync();
            return usuario.Cuentas.Select(cuenta => new CuentasModel(cuenta.NombreCuenta, cuenta.Monto)).ToList();
        }

        public async Task<List<ResultadosModel>> ObtenerDatosGraficosAsync()
        {
            Usuario usuario = await CargarOCrearDatosUsuarioAsync();
            return usuario.ObtenerResumenUltimos6Meses();
        }

        public async Task<List<ResumenTransacciones>> ObtenerUltimas50TransaccionesAsync()
        {
            Usuario usuario = await CargarOCrearDatosUsuarioAsync();
            return usuario.ObtenerUltimasTransacciones();
        }

        public void LimpiarCache()
        {
            _usuarioCache = null;
            _ultimaActualizacionCache = DateTime.MinValue;
        }
    }
}
