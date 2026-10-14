using geocode.Pages;
using geocode.viewmodels;
using System.Collections.Specialized;

namespace geocode
{
    public partial class MainPage : ContentPage
    {
        private MainViewModel? vm;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Evitar crear múltiples ViewModels
            if (vm != null)
                return;

            vm = new MainViewModel();

            BindingContext = vm;

            // Auto scroll del chat
            vm.Mensajes.CollectionChanged += Mensajes_CollectionChanged;
        }

        private void Mensajes_CollectionChanged(
            object? sender,
            NotifyCollectionChangedEventArgs e)
        {
            if (vm == null || vm.Mensajes.Count == 0)
                return;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100);

                ChatCollection.ScrollTo(
                    vm.Mensajes.Last(),
                    position: ScrollToPosition.End,
                    animate: true
                );
            });
        }

        private void OnEnviarMensaje(
            object? sender,
            EventArgs e)
        {
            vm?.EnviarMensaje();
        }
        private async void OnAbrirEntregas(object? sender, EventArgs e)
        {
            try
            {
                var pagina = Handler?.MauiContext?.Services
                    .GetService<Pages.EntregaPage>();

                if (pagina == null)
                {
                    await Shell.Current.DisplayAlertAsync(
                        "Error",
                        "No se pudo resolver el servicio de EntregaPage.",
                        "OK");
                    return;
                }

                await Navigation.PushAsync(pagina);
            }
            catch (Exception ex)
            {
                string errorDetalle = ex.InnerException != null
                    ? ex.InnerException.Message
                    : ex.Message;

                System.Diagnostics.Debug.WriteLine($"Error navegación: {ex}");

                await Shell.Current.DisplayAlertAsync(
                    "Detalle del Error",
                    $"No se pudo abrir la página:\n\n{errorDetalle}",
                    "OK");
            }
        }
        private async void OnAbrirJornadas(object? sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(JornadaPage));
            }
            catch (Exception ex)
            {
                // Obtener el mensaje detallado o la excepción interna real
                string errorDetalle = ex.InnerException != null
                    ? ex.InnerException.Message
                    : ex.Message;

                System.Diagnostics.Debug.WriteLine($"Error navegación: {ex}");

                await Shell.Current.DisplayAlertAsync(
                    "Detalle del Error",
                    $"No se pudo abrir la página:\n\n{errorDetalle}",
                    "OK");
            }
        }
    }
}
