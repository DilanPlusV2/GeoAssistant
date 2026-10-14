using geocode.Pages;

namespace geocode
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(
            nameof(EntregaPage),
            typeof(EntregaPage));

            Routing.RegisterRoute(
            nameof(JornadaPage),
            typeof(JornadaPage));

            Routing.RegisterRoute(
            nameof(ConsultaEntregasPage),
            typeof(ConsultaEntregasPage));
        }
    }
}
