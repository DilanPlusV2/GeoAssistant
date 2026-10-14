using geocode.viewmodels;

namespace geocode.Pages;

public partial class EntregaPage : ContentPage
{
    private readonly EntregaViewModel _viewModel;

    public EntregaPage(
        EntregaViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.InicializarAsync();
    }

    private async void OnVolver(
        object? sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    // =========================================================
    // CONSULTAR ENTREGAS
    // =========================================================

    private async void OnConsultarEntregas(object? sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync(nameof(ConsultaEntregasPage));
        }
        catch (Exception ex)
        {
            string errorDetalle = ex.InnerException != null
                ? ex.InnerException.Message
                : ex.Message;

            System.Diagnostics.Debug.WriteLine($"Error al navegar a ConsultaEntregasPage: {ex}");

            await Shell.Current.DisplayAlertAsync(
                "Detalle del Error",
                $"No se pudo abrir la pantalla de consulta de entregas:\n\n{errorDetalle}",
                "OK");
        }
    }
}