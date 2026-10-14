using Microsoft.Extensions.Logging;
using geocode.services;
using geocode.Pages;
using geocode.viewmodels;

namespace geocode
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont(
                        "OpenSans-Regular.ttf",
                        "OpenSansRegular");

                    fonts.AddFont(
                        "OpenSans-Semibold.ttf",
                        "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // =====================================================
            // BASE DE DATOS
            // =====================================================

            builder.Services.AddSingleton<DatabaseService>();


            // =====================================================
            // SERVICIOS
            // =====================================================

            builder.Services.AddSingleton<JornadaService>();
            builder.Services.AddSingleton<JornadaCajeroService>();
            builder.Services.AddSingleton<CajeroService>();

            builder.Services.AddSingleton<ConfiguracionFacturacionService>();
            builder.Services.AddSingleton<DomiciliarioService>();
            builder.Services.AddSingleton<EntregaParserService>();
            builder.Services.AddSingleton<FacturaService>();
            builder.Services.AddSingleton<EntregaService>();


            // =====================================================
            // BEBIDAS
            // =====================================================

            builder.Services.AddSingleton<BebidaService>();
            builder.Services.AddSingleton<InventarioService>();
            builder.Services.AddSingleton<EntregaBebidaService>();


            // =====================================================
            // ENTREGAS
            // =====================================================

            builder.Services.AddSingleton<ProcesadorEntregaService>();
            builder.Services.AddTransient<ConsultaEntregaService>();


            // =====================================================
            // SEED
            // =====================================================

            builder.Services.AddSingleton<DatabaseSeedService>();


            // =====================================================
            // VIEWMODELS
            // =====================================================

            builder.Services.AddTransient<EntregaViewModel>();
            builder.Services.AddTransient<JornadaViewModel>();
            builder.Services.AddTransient<ConsultaEntregasViewModel>();


            // =====================================================
            // PAGES
            // =====================================================

            builder.Services.AddTransient<EntregaPage>();
            builder.Services.AddTransient<JornadaPage>();
            builder.Services.AddTransient<ConsultaEntregasPage>();

            return builder.Build();
        }
    }
}