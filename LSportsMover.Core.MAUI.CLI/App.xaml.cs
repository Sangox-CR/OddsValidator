namespace LSportsMover.Core.MAUI.CLI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "LSportsMover.Core.MAUI.CLI" };
        }
    }
}
