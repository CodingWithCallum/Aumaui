using AumauiCL.Interfaces;
namespace Aumaui
{
    public partial class App : Application
    {
        private readonly IAuthService _authService;

        public App(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage(_authService)) { Title = "Aumaui" };
        }
    }
}
