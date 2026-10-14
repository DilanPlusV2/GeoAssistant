using geocode.viewmodels;

namespace geocode.Pages;

public partial class JornadaPage : ContentPage
{
    private readonly JornadaViewModel _viewModel;

    public JornadaPage(
        JornadaViewModel viewModel)
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
}