using geocode.services;

namespace geocode
{
    public partial class App : Application
    {
        public App(
            DatabaseService databaseService,
            DatabaseSeedService seedService)
        {
            InitializeComponent();

            MainPage = new AppShell();

            InicializarBaseDatos(
                databaseService,
                seedService
            );
        }

        private async void InicializarBaseDatos(
            DatabaseService databaseService,
            DatabaseSeedService seedService)
        {
            try
            {
                await databaseService.InitializeAsync();

                await seedService.SeedAsync();

                System.Diagnostics.Debug.WriteLine(
                    "Base de datos inicializada."
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error inicializando base de datos: {ex}"
                );
            }
        }
    }
}