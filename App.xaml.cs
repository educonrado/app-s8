using app_s8.Services;

namespace app_s8
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var savedUser = UserPreferencesService.GetUser();

            if (savedUser != null)
            {
                // Usuario ya logueado, redirigir directamente
                return new Window(new NavigationPage(new Views.DashboardPage()));
            }
            else
            {
                return new Window(new NavigationPage(new Views.LoginPage()));
            }

        }
    }
}