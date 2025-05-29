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

            return new Window(new NavigationPage(new Views.LoginPage()));


        }
    }
}