using System.Diagnostics;
using app_s8.Services;
using app_s8.GoogleAuth;

namespace app_s8.Views;

public partial class LoginPage : ContentPage
{
    private readonly UserService userService;
    private readonly GoogleAuthService googleAuth;
    public LoginPage()
    {
        InitializeComponent();
        userService = UserService.Instancia;
        NavigationPage.SetHasBackButton(this, false);
        googleAuth = new GoogleAuthService();
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    private async void OnLogin(object sender, EventArgs e)
    {
        var user = await googleAuth.AuthenticateAsync();

        if (user != null)
        {
            userService.SetUserId(user.Uid);

            Debug.WriteLine("Login exitoso!"+ user.FullName);

            var loadingPage = new ContentPage
            {
                Content = new StackLayout
                {
                    Children =
                {
                    new ActivityIndicator { IsRunning = true, Color = Colors.BlueViolet },
                    new Label { Text = "Cargando...", HorizontalOptions = LayoutOptions.Center }
                },
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                }
            };

            Application.Current.MainPage = loadingPage;

            await Task.Delay(100);
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("Error", "Autenticación fallida", "OK");
        }
    }
    private async void OnTermsTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Términos y Condiciones",
            "Fecha de última actualización: 29 de mayo de 2025\r\n\r\n1. Términos y Condiciones de Uso\r\n1.1. Introducción\r\nBienvenido a CushquiApp (“la App”). Al descargar, acceder y/o usar esta aplicación, usted acepta cumplir y estar sujeto a los siguientes Términos y Condiciones. Si no está de acuerdo con alguno de estos términos, por favor no utilice la App.\r\n\r\n1.2. Descripción del Servicio\r\nCushquiApp es una plataforma digital destinada a [describir función principal de la app, ej: facilitar la gestión de tareas, comercio electrónico, etc.]. Nos esforzamos para ofrecer un servicio seguro y fiable.\r\n\r\n1.3. Uso Aceptable\r\nUsted se compromete a utilizar la App conforme a la ley y buenas prácticas.\r\n\r\nNo debe usar la App para actividades ilegales, fraudulentas o dañinas.\r\n\r\nEstá prohibido el uso para difundir contenido ofensivo, difamatorio, discriminatorio o que viole derechos de terceros.\r\n\r\n1.4. Registro y Seguridad\r\nPara usar ciertas funciones, deberá crear una cuenta con información verídica.\r\n\r\nUsted es responsable de mantener la confidencialidad de sus datos de acceso.\r\n\r\nNotifique de inmediato cualquier uso no autorizado.\r\n\r\n1.5. Propiedad Intelectual\r\nTodos los derechos sobre el diseño, logos, textos, imágenes y software pertenecen a CushquiApp o sus licenciantes.\r\n\r\nNo puede copiar, modificar, distribuir ni explotar el contenido sin autorización expresa.\r\n\r\n1.6. Limitación de Responsabilidad\r\nCushquiApp no garantiza la disponibilidad continua, ni que la App esté libre de errores o vulnerabilidades.\r\n\r\nNo nos responsabilizamos por pérdidas o daños derivados del uso o mal uso de la App.\r\n\r\n1.7. Cambios en los Términos\r\nPodemos actualizar estos términos en cualquier momento.\r\n\r\nSe informará a los usuarios mediante notificaciones o correo electrónico.\r\n\r\n1.8. Terminación del Servicio\r\nPodemos suspender o cancelar su acceso si incumple estos términos.\r\n\r\nUsted puede dar por terminada su cuenta cuando lo desee.\r\n\r\n1.9. Ley Aplicable y Jurisdicción\r\nEstos términos se rigen por las leyes de [país o estado].\r\n\r\nCualquier disputa se someterá a los tribunales competentes de [lugar].\r\n\r\n2. Política de Privacidad\r\n2.1. Introducción\r\nEn CushquiApp valoramos su privacidad y nos comprometemos a proteger sus datos personales conforme a la legislación vigente, incluyendo el Reglamento General de Protección de Datos (RGPD) y otras leyes aplicables.\r\n\r\n2.2. Datos que Recopilamos\r\nInformación de registro: nombre, correo electrónico, teléfono.\r\n\r\nDatos técnicos: dirección IP, tipo de dispositivo, sistema operativo.\r\n\r\nInformación de uso: interacciones dentro de la App, tiempos y acciones.\r\n\r\nDatos de ubicación, si usted autoriza su acceso.\r\n\r\nInformación sensible: sólo si es absolutamente necesaria y con su consentimiento explícito.\r\n\r\n2.3. Finalidades del Tratamiento\r\nProveer y mejorar el servicio.\r\n\r\nGestionar su cuenta y comunicaciones.\r\n\r\nEnviar notificaciones relevantes o promociones con su consentimiento.\r\n\r\nCumplir con obligaciones legales y de seguridad.\r\n\r\n2.4. Base Legal para el Tratamiento\r\nConsentimiento previo y explícito.\r\n\r\nEjecución de contrato o relación.\r\n\r\nCumplimiento de obligaciones legales.\r\n\r\nInterés legítimo para fines estadísticos o mejora del servicio.\r\n\r\n2.5. Compartición de Datos\r\nNo vendemos sus datos a terceros.\r\n\r\nCompartimos con proveedores autorizados que ayudan en la prestación del servicio (hosting, analítica, soporte).\r\n\r\nPodemos compartir con autoridades legales en caso de requerimiento formal.\r\n\r\n2.6. Retención de Datos\r\nConservamos sus datos sólo durante el tiempo necesario para cumplir con las finalidades indicadas o cuando la ley lo exija.\r\n\r\n2.7. Derechos del Usuario\r\nUsted puede, en cualquier momento:\r\n\r\nSolicitar acceso a sus datos.\r\n\r\nRectificar datos inexactos.\r\n\r\nSolicitar la supresión de sus datos.\r\n\r\nOponerse al tratamiento o solicitar la limitación.\r\n\r\nRetirar el consentimiento otorgado.\r\n\r\nSolicitar portabilidad de datos.\r\n\r\n2.8. Seguridad de los Datos\r\nAdoptamos medidas técnicas y organizativas para proteger sus datos contra acceso no autorizado, pérdida, alteración o divulgación.\r\n\r\n2.9. Contacto para Protección de Datos\r\nPara ejercer sus derechos o consultas, puede contactarnos en: privacidad@cushquiapp.com\r\n\r\n3. Aviso Legal\r\n3.1. Propósito de la App\r\nCushquiApp es una aplicación destinada a [descripción breve del objetivo principal].\r\n\r\n3.2. Limitaciones\r\nNo garantizamos que la App estará siempre libre de interrupciones o errores.\r\n\r\nNo somos responsables por daños derivados del uso o mal uso de la App.\r\n\r\nLa información proporcionada no sustituye asesoría profesional.\r\n\r\n3.3. Enlaces a Terceros\r\nLa App puede contener enlaces a sitios externos. No nos responsabilizamos por sus contenidos o políticas.\r\n\r\n4. Consentimiento Informado para Tratamiento de Datos Personales\r\nAntes de iniciar el uso de la App y recolectar cualquier dato personal, se presenta la siguiente declaración para aceptación:\r\n\r\n“Con mi aceptación, autorizo a CushquiApp a tratar mis datos personales conforme a la Política de Privacidad, para los fines descritos. Entiendo que puedo retirar mi consentimiento en cualquier momento, así como ejercer mis derechos ARCO (Acceso, Rectificación, Cancelación y Oposición).”\r\n\r\n5. Permisos Solicitados por la App y su Justificación\r\nPermiso\tPropósito\r\nAcceso a la cámara\tPara tomar fotos o escanear documentos.\r\nAcceso a ubicación\tPara funciones basadas en ubicación GPS.\r\nAcceso a almacenamiento\tPara guardar y cargar archivos.\r\nNotificaciones\tPara enviar alertas, promociones o avisos.\r\nAcceso a contactos\tPara funcionalidades sociales o invitaciones.\r\n\r\n6. Seguridad y Buenas Prácticas en el Uso de CushquiApp\r\nMantenga su dispositivo actualizado y protegido.\r\n\r\nNo comparta sus datos de acceso.\r\n\r\nRevise periódicamente los permisos concedidos.\r\n\r\nContacte a soporte ante cualquier actividad sospechosa.\r\n\r\n7. Preguntas Frecuentes (FAQs)\r\n¿Puedo eliminar mi cuenta?\r\nSí, en cualquier momento desde la configuración o contactando soporte.\r\n\r\n¿Qué datos recopila CushquiApp?\r\nDatos personales básicos, técnicos, de uso y ubicación (si autoriza).\r\n\r\n¿Mis datos se venden a terceros?\r\nNo, sus datos no se venden ni alquilan.\r\n\r\n¿Cómo puedo ejercer mis derechos de privacidad?\r\nEnviando un correo a privacidad@cushquiapp.com con su solicitud.\r\n\r\n8. Contacto y Soporte\r\nPara cualquier duda legal o técnica, puede escribirnos a:\r\nsoporte@cushquiapp.com\r\nTeléfono: +[código país] [número]\r\nHorario: Lunes a viernes, 9:00 a 18:00\r\n\r\n",
            "Cerrar");
    }


}