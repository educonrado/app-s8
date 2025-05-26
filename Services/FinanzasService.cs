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

            if (!forzarRecarga && _usuarioCache != null && (DateTime.Now))
            {
                
            }

            DocumentReference docRef = _db.Collection(Constantes.FirestoreCollections.Usuarios).Document(uid);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<Usuario>();
            }
            else
            {
                Usuario nuevoUsuario = new Usuario
                {
                    SaldoActual = 0.0,
                    Gastos = new List<Gasto>(),
                    Ingresos = new List<Ingreso>(),
                    Cuentas = new List<Cuenta>
                    {
                        new Cuenta{NombreCuenta = "Efectivo", Monto = 0.0 }
                    },
                    ResumenMensual = new Dictionary<string, ResumenMes>(),
                    UltimaActualizacion = Timestamp.GetCurrentTimestamp()

                };
                await docRef.SetAsync(nuevoUsuario);
                return nuevoUsuario;
            }

        }

        public async Task ActualizarDatosUsuarioAsync(Usuario usuario)
        {
            string uid = _userService.CurrentUserId;
            if (string.IsNullOrEmpty(uid))
            {
                throw new InvalidOperationException("UID del usuario no disponible.");
            }

            DocumentReference docRef = _db.Collection(Constantes.FirestoreCollections.Usuarios).Document(uid);
            await docRef.SetAsync(usuario);
        }

        public async Task AgregarGastoAsync(Gasto nuevoGasto)
        {
            Usuario usuario = await CargarOCrearDatosUsuarioAsync();
            if (usuario == null) return;

            nuevoGasto.Id = Guid.NewGuid().ToString();
            if (nuevoGasto.Fecha == null) nuevoGasto.Fecha = Timestamp.GetCurrentTimestamp();

            usuario.Gastos.Add(nuevoGasto);
            usuario.SaldoActual -= nuevoGasto.Monto;

            string mesKey = nuevoGasto.Fecha.ToDateTime().ToString("yyyy-MM");
            if (!usuario.ResumenMensual.ContainsKey(mesKey))
            {
                usuario.ResumenMensual[mesKey] = new ResumenMes();
            }

            usuario.ResumenMensual[mesKey].GastosTotal += nuevoGasto.Monto;
            usuario.UltimaActualizacion = Timestamp.GetCurrentTimestamp();

            await ActualizarDatosUsuarioAsync(usuario);

        }

        public async Task AgregarIngresoAsync(Ingreso nuevoIngreso)
        {
            Usuario usuario = await CargarOCrearDatosUsuarioAsync();
            if (usuario == null) return;

            nuevoIngreso.Id = Guid.NewGuid().ToString();
            if (nuevoIngreso.Fecha == null) nuevoIngreso.Fecha = Timestamp.GetCurrentTimestamp();

            usuario.Ingresos.Add(nuevoIngreso);
            usuario.SaldoActual += nuevoIngreso.Monto;

            string mesKey = nuevoIngreso.Fecha.ToDateTime().ToString("yyyy-MM");
            if (!usuario.ResumenMensual.ContainsKey(mesKey))
            {
                usuario.ResumenMensual[mesKey] = new ResumenMes();
            }
            usuario.ResumenMensual[mesKey].IngresosTotal += nuevoIngreso.Monto;
            usuario.UltimaActualizacion = Timestamp.GetCurrentTimestamp();

            await ActualizarDatosUsuarioAsync(usuario);
        }

        public async Task AgregarCuentaAsync(Cuenta nuevaCuenta)
        {
            Usuario usuario = await CargarOCrearDatosUsuarioAsync();
            if (usuario == null) return;

            nuevaCuenta.Id = Guid.NewGuid().ToString();

            usuario.Cuentas.Add(nuevaCuenta);
            usuario.UltimaActualizacion = Timestamp.GetCurrentTimestamp();

            await ActualizarDatosUsuarioAsync(usuario);
        }

        public async Task EliminarGastosAsync(string gastoId)
        {
            Usuario usuario = await CargarOCrearDatosUsuarioAsync();
            if (usuario == null) return;

            var gastoAEliminar = usuario.Gastos.FirstOrDefault(g => g.Id == gastoId);
            if (gastoAEliminar != null)
            {
                usuario.Gastos.Remove(gastoAEliminar);
                usuario.SaldoActual += gastoAEliminar.Monto;

                string mesKey = gastoAEliminar.Fecha.ToDateTime().ToString("yyyy-MM");
                if (usuario.ResumenMensual.ContainsKey(mesKey))
                {
                    usuario.ResumenMensual[mesKey].GastosTotal -= gastoAEliminar.Monto;
                }

                usuario.UltimaActualizacion = Timestamp.GetCurrentTimestamp();
                await ActualizarDatosUsuarioAsync(usuario);

            }
        }
    }
}
