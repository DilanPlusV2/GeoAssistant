using geocode.viewmodels;

namespace geocode.Pages;

public partial class ConsultaEntregasPage
    : ContentPage
{
    private readonly
        ConsultaEntregasViewModel _viewModel;


    public ConsultaEntregasPage(
        ConsultaEntregasViewModel viewModel)
    {
        InitializeComponent();

        _viewModel =
            viewModel;

        BindingContext =
            _viewModel;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel
            .InicializarAsync();
    }


    private async void OnVolver(
        object? sender,
        EventArgs e)
    {
        await Shell.Current
            .GoToAsync("..");
    }
}